using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public class InteractRequest
    {
        public string text;
        public Vector3 worldPosition;
        public int priority;
        public object source;
    }
    [Header("��ʼ�˵�")]
    public GameObject startPanel;
    public GameObject loadingPanel;

    [Header("��Ϣ��ʾ")]
    public GameObject messagePanel;
    public Text messageText;

    [Header("�Ի���ʾ")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public Text npcNameText;

    [Header("ѡ�ť��ʾ")]
    public Transform optionHolder;
    public GameObject optionButtonPrefab;
    private Action<int> onOptionSelectedCallback;

    [Header("������ʾ")]
    public GameObject interactUI;
    public Text interactText;
    [SerializeField] private int uiUpdateInterval = 5;

    [Header("��ʾ��ʾ")]
    public GameObject tipPanel;
    public Text tipText;

    [Header("����")]
    public GameObject settings;
    public GameObject settingsPanel;
    public Toggle bgmToggle;
    public Slider volumeSlider;
    public GameObject inventoryCanvas;
    public GameObject timer;
    private bool isSettingsOpen = false;


    private List<InteractRequest> activeRequests = new List<InteractRequest>();
    private Action onDialogueCompleteCallback;
    private string[] currentDialogue;
    private int currentLineIndex = 0;
    private PlayerController player;

    private bool isDialogueOn=false;
    private string lastMessage = "";

    #region ��ʼ��
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void ShowMainMenu()
    {
        startPanel.SetActive(true);
        timer.SetActive(false);
        settings.SetActive(false);
        inventoryCanvas.SetActive(false);
        messagePanel.SetActive(false);
        dialoguePanel.SetActive(false);
        interactUI.SetActive(false);
        settingsPanel.SetActive(false);
        tipPanel.SetActive(false);  
    }


    private void Start()
    {
        startPanel.transform.SetAsFirstSibling();
        settingsPanel.transform.SetSiblingIndex(1);
        tipPanel.transform.SetAsLastSibling();
        ShowMainMenu();
        bgmToggle.onValueChanged.AddListener(OnBGMToggle);
        volumeSlider.onValueChanged.AddListener(OnVolumeChange);
    }
    private void Update()
    {
        HandleSettingsToggle();
        HandleOptimizedRefresh();
        if (startPanel.activeSelf || settingsPanel.activeSelf)
        {
            Time.timeScale = 0;
        }
    }
    public void OnStartGame()
    {
        startPanel.SetActive(false);
        GameManager.Instance.StartGame();
    }

    public void ShowSet()
    {
        timer.SetActive(true);
        settings.SetActive(true);
        inventoryCanvas.SetActive(true);
    }

    public void HideSet()
    {
        timer.SetActive(false);
        settings.SetActive(false);
        inventoryCanvas.SetActive(false);
    }
    public void OnBackToMenu()
    {
        Time.timeScale = 0;
        ShowMainMenu();
    }

    public void OnExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void HandleSettingsToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }
    }

    private void HandleOptimizedRefresh()
    {
        if (Time.frameCount % uiUpdateInterval == 0)
        {
            UpdateBestInteract();
        }
    }
    #endregion

    #region ����
    public void ToggleSettings()
    {
        bool isCurrentlyActive = settingsPanel.activeSelf;
        bool newState = !isCurrentlyActive;

        settingsPanel.SetActive(newState);
        isSettingsOpen = newState;
    }


    private void OnBGMToggle(bool isOn)
    {
        AudioManager.Instance.SetBGMState(isOn);
    }

    private void OnVolumeChange(float volume)
    {
        AudioManager.Instance.SetVolume(volume);
    }

    #endregion

    #region ��Ϣ��ʾ
    public void ShowMessage(string message)
    {
        if (message == lastMessage) return;

        lastMessage = message;
        messageText.text = message;
        messagePanel.SetActive(true);
        StartCoroutine(HideMessageAfterDelay());
    }
    private IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(5f);  
        messagePanel.SetActive(false); 
    }

    #endregion

    #region �Ի���ʾ
    public void ShowDialogue(string[] dialogue, Action onDialogueComplete, string npcName = "")
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;
        onDialogueCompleteCallback = onDialogueComplete;

        if (npcNameText != null)
            npcNameText.text = npcName;

        dialoguePanel.SetActive(true);
        ShowNextLine();
        isDialogueOn = true;
    }


    // ��ʾ��һ�仰
    public void OnNextButtonClicked()
    {
        currentLineIndex++;
        if (currentLineIndex < currentDialogue.Length)
        {
            ShowNextLine();
        }
        else
        {
            onDialogueCompleteCallback?.Invoke();   
        }
    }

    private void ShowNextLine()
    {
        dialogueText.text = currentDialogue[currentLineIndex];
    }

    // ��ʾѡ��
    public void ShowOptions(string[] options, Action<int> onOptionSelected)
    {
        ClearOptions();
        onOptionSelectedCallback = onOptionSelected;

        for (int i = 0; i < options.Length; i++)
        {
            GameObject optionButton = Instantiate(optionButtonPrefab, optionHolder);
            Button button = optionButton.GetComponent<Button>();
            button.gameObject.SetActive(true);

            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = options[i];
            }

            int optionIndex = i;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                onOptionSelectedCallback?.Invoke(optionIndex);
                ClearOptions();   // ѡ�������ť
            });
        }

        StartCoroutine(RefreshLayout());
    }
    IEnumerator RefreshLayout()
    {
        // �ȴ�һ֡�ò��������ʼ��
        yield return null;

        // ǿ��ˢ�¸����岼��
        LayoutRebuilder.ForceRebuildLayoutImmediate(optionHolder as RectTransform);

        // ���ʹ��Content Size Fitter
        foreach (var fitter in optionHolder.GetComponentsInChildren<ContentSizeFitter>())
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(fitter.GetComponent<RectTransform>());
        }
    }
    private void ClearOptions()
    {
        foreach (Transform child in optionHolder)
        {
            Destroy(child.gameObject);
        }
    }
    public void HideDialogue()
    {
        isDialogueOn = false;
        dialoguePanel.SetActive(false);
    }
    #endregion

    #region ������ʾ
    public void RegisterInteract(InteractRequest request)
    {
        if (!activeRequests.Contains(request))
        {
            activeRequests.Add(request);
            UpdateBestInteract();
        }
    }

    public void UnregisterInteract(InteractRequest request)
    {
        if (activeRequests.Contains(request))
        {
            activeRequests.Remove(request);
            UpdateBestInteract();
        }
    }

    private void UpdateBestInteract()
    {
        if (isDialogueOn || isSettingsOpen || InventorySystem.Instance.isInventoryOpen || activeRequests.Count == 0)
        {
            interactUI.SetActive(false); 
            return;
        }
            var validRequests = activeRequests.Where(r => {
            if (r.source is CombustibleItem ci)
                return !ci.isBurning;
            return true;
        }).ToList();

        if (validRequests.Count == 0)
        {
            interactUI.SetActive(false);
            return;
        }
        var bestRequest = activeRequests
            .OrderByDescending(r => r.priority)
            .FirstOrDefault();

        if (bestRequest != null)
        {
            UpdateInteractUI(bestRequest);
        }
    }

    private void UpdateInteractUI(InteractRequest request)
    {
        interactText.text = request.text;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(request.worldPosition);
        interactUI.GetComponent<RectTransform>().position = screenPos;
        interactUI.SetActive(true);
    }
    #endregion

    #region ��ʾ��ʾ
    public void ShowTips(string Text)
    {
        tipPanel.SetActive(true);
        tipText.text = Text;    
    }
    #endregion


}

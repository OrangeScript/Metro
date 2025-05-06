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

    [Header("信息显示")]
    public GameObject messagePanel;
    public Text messageText;

    [Header("对话显示")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public Button nextButton;
    public Text npcNameText;

    [Header("选项按钮显示")]
    public Transform optionHolder;
    public GameObject optionButtonPrefab;
    private Action<int> onOptionSelectedCallback;

    [Header("交互显示")]
    public GameObject interactUI;
    public Text interactText;
    [SerializeField] private int uiUpdateInterval = 5;

    [Header("设置")]
    public GameObject settingsPanel;
    private bool isSettingsOpen = false;

    private List<InteractRequest> activeRequests = new List<InteractRequest>();
    private Action onDialogueCompleteCallback;
    private string[] currentDialogue;
    private int currentLineIndex = 0;
    private PlayerController player;

    private bool isDialogueOn=false;

    #region 初始化
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
        InitializeUIState();
    }

    private void InitializeUIState()
    {
        messagePanel.SetActive(false);
        dialoguePanel.SetActive(false);
        interactUI.SetActive(false);
        settingsPanel.SetActive(false);
    }


    private void Start()
    {
        nextButton.onClick.AddListener(OnNextButtonClicked);
        nextButton.gameObject.SetActive(false);
    }
    private void Update()
    {
        HandleUIState();
        HandleSettingsToggle();
        HandleOptimizedRefresh();
    }

    private void HandleUIState()
    {
        if (isDialogueOn || isSettingsOpen||InventorySystem.Instance.isInventoryOpen|| activeRequests.Count == 0)
        {
            interactUI.SetActive(false);
        }
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

    #region 设置
    public void ToggleSettings()
    {
        isSettingsOpen = !isSettingsOpen;

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(isSettingsOpen);
        }
    }
    #endregion

    #region 信息显示
    public void ShowMessage(string message)
    {
        messageText.text = message;
        messagePanel.SetActive(true);
        StartCoroutine(HideMessageAfterDelay());
    }
    private IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(3f);  
        messagePanel.SetActive(false); 
    }

    #endregion

    #region 对话显示
    public void ShowDialogue(string[] dialogue, Action onDialogueComplete, string npcName = "")
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;
        onDialogueCompleteCallback = onDialogueComplete;

        if (npcNameText != null)
            npcNameText.text = npcName;

        dialoguePanel.SetActive(true);
        nextButton.gameObject.SetActive(true);
        ShowNextLine();
        isDialogueOn = true;
    }


    // 显示下一句话
    private void OnNextButtonClicked()
    {
        currentLineIndex++;
        if (currentLineIndex < currentDialogue.Length)
        {
            ShowNextLine();
        }
        else
        {
            nextButton.gameObject.SetActive(false); 
            onDialogueCompleteCallback?.Invoke();   
        }
    }

    private void ShowNextLine()
    {
        dialogueText.text = currentDialogue[currentLineIndex];
    }

    // 显示选项
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
                ClearOptions();   // 选择后清理按钮
            });
        }

        StartCoroutine(RefreshLayout());
    }
    IEnumerator RefreshLayout()
    {
        // 等待一帧让布局组件初始化
        yield return null;

        // 强制刷新父物体布局
        LayoutRebuilder.ForceRebuildLayoutImmediate(optionHolder as RectTransform);

        // 如果使用Content Size Fitter
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

    #region 交互显示
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
}

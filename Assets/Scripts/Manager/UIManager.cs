using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor.Rendering;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
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

    public GameObject interactUI;
    public Text interactText;

    private Action onDialogueCompleteCallback;
    private string[] currentDialogue;
    private int currentLineIndex = 0;
    private PlayerController player;

    private bool isDialogueOn=false;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        Instance = this;
        messagePanel.SetActive(false);
        dialoguePanel.SetActive(false);  
    }

    public void Update()
    {
        if (isDialogueOn) { interactUI.gameObject.SetActive(false); }
    }

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

    private void Start()
    {
        nextButton.onClick.AddListener(OnNextButtonClicked);
        nextButton.gameObject.SetActive(false);
    }

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

    // 隐藏对话框
    public void HideDialogue()
    {
        isDialogueOn = false;
        dialoguePanel.SetActive(false);
    }

    public void ShowInteractUI(bool show,string text)
    {
        if (interactUI != null)
        {
            interactUI.SetActive(show);
        }
        interactText.text = text;
    }

    public void UpdateInteractUIPosition()
    {
        if (interactUI != null && player.nearestInteractable != null)
        {
            Vector3 worldPosition = player.nearestInteractable.transform.position;
            Vector3 offset = new Vector3(0, -0.5f, 0);

            worldPosition += offset;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            interactUI.GetComponent<RectTransform>().position = screenPosition;
        }
    }

    public void UpdateInteractWithNPCUIPosition()
    {
        if (interactUI != null && player.currentNPC != null)
        {
            Vector3 worldPosition = player.currentNPC.transform.position;
            Vector3 offset = new Vector3(0, -0.5f, 0);

            worldPosition += offset;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            interactUI.GetComponent<RectTransform>().position = screenPosition;
        }
    }

}

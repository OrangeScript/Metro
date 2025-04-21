using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private GameObject mazePanel;
    public GameObject messagePanel;
    public Text messageText;
    public GameObject dialogPanel;
    public Text dialogText;

    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private Text interactionText;

    public bool IsDialogueActive = false;

    private void Awake()
    {
        Instance = this;
        HideInteractionPrompt();
        messagePanel.SetActive(false);
        dialogPanel.SetActive(false);  
    }

    public void ShowInteractionPrompt(string message, Vector3 worldPosition)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
            interactionText.text = message;

            // UI 位置转换（世界坐标 -> 屏幕坐标）
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            interactionPrompt.transform.position = screenPosition;
        }
    }

    public void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    public void ShowMazeUI()
    {
        mazePanel.SetActive(true);
    }

    public void HideMazeUI()
    {
        mazePanel.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        messagePanel.SetActive(true);   
    }

    public void ShowDialogue(string[] dialogue)
    {
        dialogText.text = string.Join("\n", dialogue);
        dialogPanel.SetActive(true);
    }

}

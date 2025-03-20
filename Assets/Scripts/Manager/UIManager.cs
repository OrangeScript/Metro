using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private Text interactionText;

    public bool IsDialogueActive = false;

    private void Awake()
    {
        Instance = this;
        HideInteractionPrompt();
    }

    public void ShowInteractionPrompt(string message, Vector3 worldPosition)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
            interactionText.text = message;

            // UI λ��ת������������ -> ��Ļ���꣩
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
}

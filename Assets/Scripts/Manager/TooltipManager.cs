using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    public GameObject tooltipPanel;
    public Text tooltipText;
    public Vector2 fixedPosition;

    void Awake()
    {
        Instance = this;
        tooltipPanel.SetActive(false);

        fixedPosition = new Vector2(0, 0);
    }

    public void Show(string content)
    {
        tooltipText.text = content;
        tooltipPanel.SetActive(true);

        tooltipPanel.GetComponent<RectTransform>().anchoredPosition = fixedPosition;
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}

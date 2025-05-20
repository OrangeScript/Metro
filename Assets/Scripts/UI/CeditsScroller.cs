using UnityEngine;
using System.Collections;

public class CreditsScroller : MonoBehaviour
{
    public static CreditsScroller Instance { get; private set; }

    public RectTransform contentTransform; // 制作人名单内容
    public float scrollSpeed = 30f;
    public float startDelay = 0.5f;
    public float scrollTime = 10f;

    private bool isScrolling = false;

    private void Awake()
    {
        // 单例模式，确保场景中只有一个实例
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gameObject.SetActive(false);
    }

    public void StartScrolling()
    {
        if (!isScrolling)
        {
            isScrolling = true;
            StartCoroutine(ScrollCredits());
        }
    }

    private IEnumerator ScrollCredits()
    {
        yield return new WaitForSecondsRealtime(startDelay);

        float elapsedTime = 0f;
        Vector2 startPos = contentTransform.anchoredPosition;

        while (elapsedTime < scrollTime)
        {
            float deltaY = scrollSpeed * Time.unscaledDeltaTime;
            contentTransform.anchoredPosition += new Vector2(0f, deltaY);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        Debug.Log("制作人名单滚动结束");
        UIManager.Instance.OnBackToMenu();
        gameObject.SetActive(false);
        isScrolling = false;
    }

}

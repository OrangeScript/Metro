using UnityEngine;
using System.Collections;

public class CreditsScroller : MonoBehaviour
{
    public static CreditsScroller Instance { get; private set; }

    public RectTransform contentTransform; // ��������������
    public float scrollSpeed = 30f;
    public float startDelay = 0.5f;
    public float scrollTime = 10f;

    private bool isScrolling = false;

    private void Awake()
    {
        // ����ģʽ��ȷ��������ֻ��һ��ʵ��
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

        Debug.Log("������������������");
        UIManager.Instance.OnBackToMenu();
        gameObject.SetActive(false);
        isScrolling = false;
    }

}

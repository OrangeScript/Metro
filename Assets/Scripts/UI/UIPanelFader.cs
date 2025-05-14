using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelFader : MonoBehaviour
{
    public static UIPanelFader Instance { get; private set; }

    public enum FadeMode
    {
        FadeTextOnly,
        FadePanelAndText
    }

    public FadeMode fadeMode = FadeMode.FadeTextOnly;
    public CanvasGroup canvasGroup;
    public Text text;
    public float fadeDuration = 2;
    public float waitBeforeFadeOut = 2f;
    public float panelFadeDuration = 2f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        gameObject.SetActive(false);
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;     
    }

    private void Start()
    {
        transform.SetSiblingIndex(2);
    }

    public void Setup(FadeMode mode, string message)
    {
        fadeMode = mode;
        if (text != null)
        {
            text.text = message;
        }
    }

    public void Play(Action onComplete = null)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = StartCoroutine(FadeSequence(onComplete));
    }

    private IEnumerator FadeSequence(Action onComplete)
    {
        switch (fadeMode)
        {
            case FadeMode.FadeTextOnly:
                canvasGroup.alpha = 1f;
                SetTextAlpha(0f);
                break;
            case FadeMode.FadePanelAndText:
                canvasGroup.alpha = 0f;
                break;
        }

        if (fadeMode == FadeMode.FadeTextOnly)
        {
            yield return StartCoroutine(FadeTextIn());
        }
        else
        {
            yield return StartCoroutine(FadeCanvasGroupIn());
        }

        yield return new WaitForSeconds(waitBeforeFadeOut);
        yield return StartCoroutine(FadeOutPanel());

        currentRoutine = null;
        onComplete?.Invoke();
    }

    private IEnumerator FadeTextIn()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            SetTextAlpha(alpha);
            yield return null;
        }
        SetTextAlpha(1f);
    }

    private IEnumerator FadeCanvasGroupIn()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            canvasGroup.alpha = alpha;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutPanel()
    {
        float timer = 0f;
        while (timer < panelFadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(timer / panelFadeDuration);
            canvasGroup.alpha = alpha;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    private void SetTextAlpha(float alpha)
    {
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
}

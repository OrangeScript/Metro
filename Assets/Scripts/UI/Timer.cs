using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [Header("计时器设置")]
    [SerializeField] private float totalTime = 370f;
    [SerializeField] private RectTransform fillRect;
    [SerializeField] private Image fillImage;
    [SerializeField] private float maxWidth = 350f;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private Text timeText;
    [SerializeField] private float warningThreshold = 5f;

    private float currentTime;
    private bool isTimerRunning;
    private Color originalColor;

    void Start()
    {
        originalColor = fillImage.color;

        fillRect.sizeDelta = new Vector2(0, fillRect.sizeDelta.y);
        timeText.text = "";
        isTimerRunning = false;
    }

    void Update()
    {
        if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver) return;
        // 只有当游戏开始且计时器未运行时才初始化
        if (GameManager.Instance.isGameStarted && !isTimerRunning)
        {
            InitializeTimer();
        }

        // 游戏运行中更新计时器
        if (isTimerRunning)
        {
            UpdateTimer();
        }
    }

    private void InitializeTimer()
    {
        currentTime = totalTime;
        fillRect.sizeDelta = new Vector2(maxWidth, fillRect.sizeDelta.y);
        isTimerRunning = true;
    }

    private void UpdateTimer()
    {
        currentTime -= Time.deltaTime;
        float progress = currentTime / totalTime;
        float currentWidth = maxWidth * progress;
        fillRect.sizeDelta = new Vector2(currentWidth, fillRect.sizeDelta.y);

        timeText.text = Mathf.CeilToInt(currentTime).ToString() + "s";

        if (currentTime <= warningThreshold)
        {
            float lerpFactor = Mathf.PingPong(Time.time * 2f, 1f);
            fillImage.color = Color.Lerp(originalColor, warningColor, lerpFactor);
        }
        else
        {
            fillImage.color = originalColor;
        }

        if (currentTime <= 0)
        {
            currentTime = 0;
            isTimerRunning = false;
            OnTimeEnd();
        }
    }

    private void OnTimeEnd()
    {
        Debug.Log("时间到！");
        GameManager.Instance.EndGame();
    }

    public void ResetTimer()
    {
        InitializeTimer();
    }
}

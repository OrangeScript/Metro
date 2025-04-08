using UnityEngine;
using static UnityEditor.Progress;

public class SafetyHammer : InteractableObject
{
    [Header("破窗设置")]
    public int hitsRequired = 3; // 砸碎窗户所需的砸击次数
    public float hitCooldown = 0.5f; // 砸击冷却时间

    private int currentHits; // 当前砸击次数
    private float lastHitTime; // 上次砸击时间
    private Window currentTargetWindow; // 当前目标窗户
    private float interactRadius = 5f;

    protected override void Start()
    {
        base.Start();
        destroyOnUse = false;
        useTrigger = UseTrigger.RightClick;
    }

    protected override void HandleUse()
    {
        Debug.Log("使用安全锤敲击玻璃");

        // 检查冷却时间
        if (Time.time - lastHitTime < hitCooldown)
        {
            Debug.Log("冷却时间未到，无法再次使用安全锤！");
            return;
        }

        lastHitTime = Time.time;

        Window targetWindow = GetTargetWindow();
        if (targetWindow != null)
        {
            float distanceToWindow = Vector2.Distance(player.transform.position, targetWindow.transform.position);
            if (distanceToWindow <= interactRadius)
            {
                if (currentTargetWindow != targetWindow)
                {
                    currentHits = 0; 
                    currentTargetWindow = targetWindow; 
                    Debug.Log("目标窗户已改变，砸击次数已重置！");
                }

                currentHits++;
                Debug.Log($"砸击窗户: {currentHits}/{hitsRequired}");

                if (currentHits >= hitsRequired)
                {
                    targetWindow.Break(); // 破碎窗户
                    currentHits = 0; // 重置砸击次数
                    currentTargetWindow = null; // 重置当前目标窗户
                    Debug.Log("窗户已破碎！");
                }
            }
            else
            {
                Debug.Log("距离窗户太远，无法砸击！");
            }
        }
    }

    private Window GetTargetWindow()
    {
        if (player == null) return null;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null && hit.collider.CompareTag("Window"))
        {
            return hit.collider.GetComponent<Window>();
        }
        return null;
    }
}
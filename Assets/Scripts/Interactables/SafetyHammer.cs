using UnityEngine;
using static UnityEditor.Progress;

public class SafetyHammer : InteractableObject
{
    [Header("�ƴ�����")]
    public int hitsRequired = 3; // ���鴰��������һ�����
    public float hitCooldown = 0.5f; // �һ���ȴʱ��

    private int currentHits; // ��ǰ�һ�����
    private float lastHitTime; // �ϴ��һ�ʱ��
    private Window currentTargetWindow; // ��ǰĿ�괰��
    private float interactRadius = 5f;

    protected override void Start()
    {
        base.Start();
        destroyOnUse = false;
        useTrigger = UseTrigger.RightClick;
    }

    protected override void HandleUse()
    {
        Debug.Log("ʹ�ð�ȫ���û�����");

        // �����ȴʱ��
        if (Time.time - lastHitTime < hitCooldown)
        {
            Debug.Log("��ȴʱ��δ�����޷��ٴ�ʹ�ð�ȫ����");
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
                    Debug.Log("Ŀ�괰���Ѹı䣬�һ����������ã�");
                }

                currentHits++;
                Debug.Log($"�һ�����: {currentHits}/{hitsRequired}");

                if (currentHits >= hitsRequired)
                {
                    targetWindow.Break(); // ���鴰��
                    currentHits = 0; // �����һ�����
                    currentTargetWindow = null; // ���õ�ǰĿ�괰��
                    Debug.Log("���������飡");
                }
            }
            else
            {
                Debug.Log("���봰��̫Զ���޷��һ���");
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
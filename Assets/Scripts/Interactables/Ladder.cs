using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class VentTarget
{
    public string tag;
    public Transform targetTransform;
}

public class Ladder : InteractableObject
{
    [Header("����״̬")]
    [SerializeField] private bool isExtended = false;

    [Header("�Ӿ�Ч��")]
    [SerializeField] private Sprite retractedSprite;
    [SerializeField] private Sprite extendedSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("��������")]
    [SerializeField] private float teleportDelay = 0.5f;
    [SerializeField] private float closestDistance = 22f;

    [Header("�ɴ���Ŀ��")]
    [SerializeField] private List<VentTarget> ventTargets = new List<VentTarget>();

    private bool isTeleporting = false;

    protected override void Start()
    {
        base.Start();

        if (ventTargets.Count == 0)
        {
            Debug.LogError("[Ladder] δ�����κδ���Ŀ�꣡");
            enabled = false;
        }
    }

    protected override void HandleUse()
    {
        if (isExtended && !isTeleporting)
        {
            StartCoroutine(TeleportProcess());
        }
        else
        {
            ToggleLadderState();
        }
    }

    private IEnumerator TeleportProcess()
    {
        isTeleporting = true;
        Debug.Log("[Ladder] ��ʼ��������...");
        yield return new WaitForSeconds(teleportDelay);

        Transform nearestVent = FindNearestVent();

        if (nearestVent != null)
        {
            UIManager.Instance.ShowMessage("���ڽ���ͨ��ܵ�...");
            player.transform.position = nearestVent.position;
            player.currentState = PlayerController.PlayerState.Crawling;
            Debug.Log($"[Ladder] ������ɣ�Ŀ��λ�ã�{nearestVent.name}��{nearestVent.position}");
        }
        else
        {
            Debug.LogWarning("[Ladder] δ�ҵ���Ч�Ĵ���Ŀ�꣡");
        }

        ToggleLadderState();
        isTeleporting = false;
        Debug.Log("[Ladder] �������̽�����");
    }

    private Transform FindNearestVent()
    {
        float minDistance = closestDistance;
        Transform resultTransform = null;

        foreach (var vent in ventTargets)
        {
            GameObject[] vents = GameObject.FindGameObjectsWithTag(vent.tag);
            foreach (var ventObj in vents)
            {
                float distance = Vector2.Distance(player.transform.position, ventObj.transform.position);
                Debug.Log($"[Ladder] ����ǩ {vent.tag} ������ {ventObj.name}������ {distance}");

                if (distance < minDistance)
                {
                    minDistance = distance;
                    resultTransform = vent.targetTransform;
                }
            }
        }

        if (resultTransform != null)
        {
            Debug.Log($"[Ladder] �ҵ����ͨ��ڶ�Ӧ�Ĵ���λ�ã�{resultTransform.name}");
        }

        return resultTransform;
    }


    private void ToggleLadderState()
    {
        isExtended = !isExtended;
        UpdateVisuals();
        Debug.Log($"[Ladder] ����״̬�л�Ϊ: {(isExtended ? "չ��" : "����")}");
    }

    private void UpdateVisuals()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isExtended ? extendedSprite : retractedSprite;
        }
        else
        {
            Debug.LogWarning("[Ladder] SpriteRenderer δ�󶨣�");
        }
    }
}

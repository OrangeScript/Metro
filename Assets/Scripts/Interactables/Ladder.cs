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
    [Header("梯子状态")]
    [SerializeField] private bool isExtended = false;

    [Header("视觉效果")]
    [SerializeField] private Sprite retractedSprite;
    [SerializeField] private Sprite extendedSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("传送设置")]
    [SerializeField] private float teleportDelay = 0.5f;
    [SerializeField] private float closestDistance = 22f;

    [Header("可传送目标")]
    [SerializeField] private List<VentTarget> ventTargets = new List<VentTarget>();

    private bool isTeleporting = false;

    protected override void Start()
    {
        base.Start();

        if (ventTargets.Count == 0)
        {
            Debug.LogError("[Ladder] 未设置任何传送目标！");
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
        Debug.Log("[Ladder] 开始传送流程...");
        yield return new WaitForSeconds(teleportDelay);

        Transform nearestVent = FindNearestVent();

        if (nearestVent != null)
        {
            UIManager.Instance.ShowMessage("正在进入通风管道...");
            player.transform.position = nearestVent.position;
            player.currentState = PlayerController.PlayerState.Crawling;
            Debug.Log($"[Ladder] 传送完成，目标位置：{nearestVent.name}，{nearestVent.position}");
        }
        else
        {
            Debug.LogWarning("[Ladder] 未找到有效的传送目标！");
        }

        ToggleLadderState();
        isTeleporting = false;
        Debug.Log("[Ladder] 传送流程结束。");
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
                Debug.Log($"[Ladder] 检查标签 {vent.tag} 的物体 {ventObj.name}，距离 {distance}");

                if (distance < minDistance)
                {
                    minDistance = distance;
                    resultTransform = vent.targetTransform;
                }
            }
        }

        if (resultTransform != null)
        {
            Debug.Log($"[Ladder] 找到最近通风口对应的传送位置：{resultTransform.name}");
        }

        return resultTransform;
    }


    private void ToggleLadderState()
    {
        isExtended = !isExtended;
        UpdateVisuals();
        Debug.Log($"[Ladder] 梯子状态切换为: {(isExtended ? "展开" : "收起")}");
    }

    private void UpdateVisuals()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isExtended ? extendedSprite : retractedSprite;
        }
        else
        {
            Debug.LogWarning("[Ladder] SpriteRenderer 未绑定！");
        }
    }
}

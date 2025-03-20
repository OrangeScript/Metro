using UnityEngine;
using System.Collections;

public class Clothing : InteractableObject
{
    [Header("扑灭设置")]
    public float extinguishRadius = 1.5f;
    public LayerMask combustibleLayer;
    private Rigidbody2D rb;

    private PlayerController player;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;

        player = GameManager.Instance.player;
        destroyOnUse = true;
    }

    public override void OnInteract()
    {
        base.OnInteract(); // 调用基类方法
        Debug.Log("衣服已拾取，存入背包！");
    }

    protected override void HandleUse()
    {
        ExtinguishNearbyCombustibles();
    }

    private void ExtinguishNearbyCombustibles()
    {
        Collider2D[] combustibles = Physics2D.OverlapCircleAll(
            transform.position,
            extinguishRadius,
            combustibleLayer
        );

        bool extinguished = false;
        foreach (var combustible in combustibles)
        {
            CombustibleItem targetCombustible = combustible.GetComponent<CombustibleItem>();
            if (targetCombustible != null && targetCombustible.isBurning)
            {
                targetCombustible.Extinguish();
                extinguished = true;
                Debug.Log("燃烧物已扑灭！");
                break; 
            }
        }

        if (!extinguished)
        {
            Debug.Log("周围没有燃烧的物品！");
        }
    }
}

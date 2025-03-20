using System.Collections;
using UnityEngine;

public class CombustibleItem : InteractableObject
{
    public enum CombustibleType { L1, L2, L3, Sober }

    [Header("燃烧物设置")]
    public CombustibleType type;
    public ParticleSystem fireEffect;
    public bool isBurning = false;
    public float burnInterval = 1f;

    [Header("烟雾设置")]
    public SmokeSystem smokeSystem;
    private Coroutine burnCoroutine;

    private Rigidbody2D rb; // 用于投掷

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
        if (!isBurning) // 只有未燃烧时才能拾取
        {
            InventorySystem.Instance.AddItem(this);
            gameObject.SetActive(false);
        }
    }

    protected override void HandleUse()
    {
            ThrowAndIgnite();
    }

    private void ThrowAndIgnite()
    {
        InventorySystem.Instance.RemoveItem(this);
        transform.SetParent(null);
        isEquipped = false;
        rb.isKinematic = false;
        rb.AddForce(player.transform.right * 5f, ForceMode2D.Impulse); // 投掷方向
        StartCoroutine(DelayIgnite(0.5f)); // 模拟投掷后落地燃烧
    }

    private IEnumerator DelayIgnite(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.isKinematic = true; // 物品落地后停止运动
        rb.velocity = Vector2.zero;
        Ignite();
    }

    public void Ignite()
    {
        if (!isBurning)
        {
            isBurning = true;
            fireEffect.Play();
            burnCoroutine = StartCoroutine(GenerateSmoke());
            Debug.Log($"{type} 级燃烧物开始燃烧！");
        }
    }

    public void Extinguish()
    {
        if (isBurning)
        {
            isBurning = false;
            fireEffect.Stop();
            if (burnCoroutine != null) StopCoroutine(burnCoroutine);
            Debug.Log("燃烧物已扑灭！");
        }
    }

    IEnumerator GenerateSmoke()
    {
        SmokeSystem.SmokeLevel level = GetSmokeLevel();
        while (isBurning)
        {
            smokeSystem.AddSmoke(transform.position, level, 1);
            yield return new WaitForSeconds(burnInterval);
        }
    }

    private SmokeSystem.SmokeLevel GetSmokeLevel()
    {
        switch (type)
        {
            case CombustibleType.L1: return SmokeSystem.SmokeLevel.Level1;
            case CombustibleType.L2: return SmokeSystem.SmokeLevel.Level2;
            case CombustibleType.L3: return SmokeSystem.SmokeLevel.Level3;
            case CombustibleType.Sober: return SmokeSystem.SmokeLevel.Sober;
            default: return SmokeSystem.SmokeLevel.Level1;
        }
    }

    public override void OnEquip(Transform parent)
    {
        if (isBurning)
        {
            Debug.Log("燃烧物正在燃烧，无法装备！");
            return;
        }
        base.OnEquip(parent);
    }

}

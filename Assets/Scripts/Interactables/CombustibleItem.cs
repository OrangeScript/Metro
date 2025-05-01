using System.Collections;
using UnityEngine;
using static UnityEditor.Progress;
public class CombustibleItem : InteractableObject
{
    public enum CombustibleLevel { L1, L2, L3, Sober }

    [Header("燃烧物设置")]
    public CombustibleLevel level;
    public bool isBurning = false;
    public float burnInterval = 1f;

    [Header("烟雾设置")]
    public SmokeSystem smokeSystem;
    private Coroutine burnCoroutine;

    [Header("烟雾生成控制")]
    public int maxSmokeCount = 10;     //最大烟雾数量
    private int currentSmokeCount = 0; //当前已生成数量

    [Header("烟雾随机偏移")]
    public Vector2 smokeRandomRange = new Vector2(0.5f, 0.5f);
    [Header("视觉表现")]
    public SpriteRenderer targetRenderer; 
    public Sprite visualSprite;

    [Header("火焰特效")]
    public GameObject Flame;
    public Vector3 flameOffset = new Vector3(0.15f, 1f, -0.1f);
    public Vector3 smokeOffset = new Vector3(0f, 0.9f, -0.9f);

    private Rigidbody2D rb;
    public BoxCollider2D itemCollider;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        smokeSystem=FindObjectOfType<SmokeSystem>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        itemCollider = rb.GetComponent<BoxCollider2D>();
        rb.isKinematic = true;
        destroyOnUse = true;

        // 应用贴图
        if (targetRenderer != null && visualSprite != null)
        {
            targetRenderer.sprite = visualSprite;
        }
    }

    public override void OnInteract()
    {
        if (!isBurning)
        {
            if (InventorySystem.Instance.AddItem(this))
            {
                gameObject.SetActive(false);
            }
        }
    }
    public override void UseItem()
    {
        if (!isEquipped)
        {
            Debug.LogWarning($"未装备 {itemName}，无法使用！");
            return;
        }

        HandleUse();
    }

    protected override void HandleUse()
    {
        ThrowAndIgnite();
    }

    private void ThrowAndIgnite()
    {
        ResetLayer();
        transform.SetParent(null);
        rb.isKinematic = false;
        transform.position = player.transform.position;

        rb.AddForce(player.transform.right * 10f, ForceMode2D.Impulse);
        StartCoroutine(DelayIgnite(0.2f));
    }

    private IEnumerator DelayIgnite(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;

        if (destroyOnUse)
        {
            Debug.Log("销毁一次性物品");

            if (player.equippedItem == this)
                player.equippedItem = null;
            InventorySystem.Instance.UpdateEquippedUI();
        }
        Ignite();
    }

    public void Ignite()
    {
        if (!isBurning)
        {
            isBurning = true;
            if (itemCollider != null)
            {
                itemCollider.isTrigger = false;  
            }
            if (Flame != null)
            {
                Flame.transform.localPosition = flameOffset;
                Flame.SetActive(true);
            }
            burnCoroutine = StartCoroutine(GenerateSmoke());
            
            Debug.Log($"{level} 级燃烧物开始燃烧！");
        }
    }


    public void Extinguish()
    {
        if (isBurning)
        {
            isBurning = false;
            if (itemCollider != null)
            {
                itemCollider.isTrigger = true;  
            }
            if (Flame != null)
                Flame.SetActive(false);

            if (burnCoroutine != null)
                StopCoroutine(burnCoroutine);

            Debug.Log("燃烧物已扑灭！");
        }
    }

    private IEnumerator GenerateSmoke()
    {
        SmokeSystem.SmokeLevel smokeLevel = ConvertToSmokeLevel(level);
        currentSmokeCount = 0; 

        while (currentSmokeCount < maxSmokeCount && isBurning)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-smokeRandomRange.x, smokeRandomRange.x),
                Random.Range(-smokeRandomRange.y, smokeRandomRange.y),
                0
            );

            smokeSystem.AddSmoke(
                transform.position + smokeOffset + randomOffset,
                smokeLevel,
                1,
                Quaternion.Euler(-135f, 0f, 0f)
            );

            currentSmokeCount++;
            yield return new WaitForSeconds(burnInterval);
        }

        Debug.Log("燃烧结束，物体销毁。");
        Destroy(gameObject);
    }




    private SmokeSystem.SmokeLevel ConvertToSmokeLevel(CombustibleLevel level)
    {
        switch (level)
        {
            case CombustibleLevel.L1: return SmokeSystem.SmokeLevel.Level1;
            case CombustibleLevel.L2: return SmokeSystem.SmokeLevel.Level2;
            case CombustibleLevel.L3: return SmokeSystem.SmokeLevel.Level3;
            case CombustibleLevel.Sober: return SmokeSystem.SmokeLevel.Sober;
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

using System.Collections;
using UnityEngine;
public class CombustibleItem : InteractableObject
{
    public enum CombustibleLevel { L1, L2, L3, Sober }

    [Header("ȼ��������")]
    public CombustibleLevel level;
    public bool isBurning = false;
    public float burnInterval = 1f;

    [Header("��������")]
    public SmokeSystem smokeSystem;
    private Coroutine burnCoroutine;

    [Header("�Ӿ�����")]
    public SpriteRenderer targetRenderer; 
    public Sprite visualSprite;

    [Header("������Ч")]
    public GameObject Flame;
    public Vector3 flameOffset = new Vector3(0, 0.5f, 0);

    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        destroyOnUse = true;

        // Ӧ����ͼ
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
            Debug.LogWarning($"δװ�� {itemName}���޷�ʹ�ã�");
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
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.AddForce(player.transform.right * 5f, ForceMode2D.Impulse);
        StartCoroutine(DelayIgnite(0.5f));
    }

    private IEnumerator DelayIgnite(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;

        if (destroyOnUse)
        {
            Debug.Log("����һ������Ʒ");

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

            if (Flame != null)
            {
                Flame.transform.position = transform.position + flameOffset;
                Flame.SetActive(true);
            }
            burnCoroutine = StartCoroutine(GenerateSmoke());
            StartCoroutine(DelayedDestroy());
            
            Debug.Log($"{level} ��ȼ���￪ʼȼ�գ�");
        }
    }


    public void Extinguish()
    {
        if (isBurning)
        {
            isBurning = false;
            if (Flame != null)
                Flame.SetActive(false);

            if (burnCoroutine != null)
                StopCoroutine(burnCoroutine);

            Debug.Log("ȼ����������");
        }
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject);
    }
    private IEnumerator GenerateSmoke()
    {
        SmokeSystem.SmokeLevel smokeLevel = ConvertToSmokeLevel(level);
        while (isBurning)
        {
            smokeSystem.AddSmoke(transform.position, smokeLevel, 1);
            yield return new WaitForSeconds(burnInterval);
        }
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
            Debug.Log("ȼ��������ȼ�գ��޷�װ����");
            return;
        }
        base.OnEquip(parent);
    }
}

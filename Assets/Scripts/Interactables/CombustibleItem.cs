using System.Collections;
using UnityEngine;
using static UnityEditor.Progress;
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

    [Header("�������ɿ���")]
    public int maxSmokeCount = 10;     //�����������
    private int currentSmokeCount = 0; //��ǰ����������

    [Header("�������ƫ��")]
    public Vector2 smokeRandomRange = new Vector2(0.5f, 0.5f);
    [Header("�Ӿ�����")]
    public SpriteRenderer targetRenderer; 
    public Sprite visualSprite;

    [Header("������Ч")]
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
            
            Debug.Log($"{level} ��ȼ���￪ʼȼ�գ�");
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

            Debug.Log("ȼ����������");
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

        Debug.Log("ȼ�ս������������١�");
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
            Debug.Log("ȼ��������ȼ�գ��޷�װ����");
            return;
        }
        base.OnEquip(parent);
    }
}

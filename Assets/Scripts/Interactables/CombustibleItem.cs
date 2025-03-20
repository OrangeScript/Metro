using System.Collections;
using UnityEngine;

public class CombustibleItem : InteractableObject
{
    public enum CombustibleType { L1, L2, L3, Sober }

    [Header("ȼ��������")]
    public CombustibleType type;
    public ParticleSystem fireEffect;
    public bool isBurning = false;
    public float burnInterval = 1f;

    [Header("��������")]
    public SmokeSystem smokeSystem;
    private Coroutine burnCoroutine;

    private Rigidbody2D rb; // ����Ͷ��

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
        if (!isBurning) // ֻ��δȼ��ʱ����ʰȡ
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
        rb.AddForce(player.transform.right * 5f, ForceMode2D.Impulse); // Ͷ������
        StartCoroutine(DelayIgnite(0.5f)); // ģ��Ͷ�������ȼ��
    }

    private IEnumerator DelayIgnite(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.isKinematic = true; // ��Ʒ��غ�ֹͣ�˶�
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
            Debug.Log($"{type} ��ȼ���￪ʼȼ�գ�");
        }
    }

    public void Extinguish()
    {
        if (isBurning)
        {
            isBurning = false;
            fireEffect.Stop();
            if (burnCoroutine != null) StopCoroutine(burnCoroutine);
            Debug.Log("ȼ����������");
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
            Debug.Log("ȼ��������ȼ�գ��޷�װ����");
            return;
        }
        base.OnEquip(parent);
    }

}

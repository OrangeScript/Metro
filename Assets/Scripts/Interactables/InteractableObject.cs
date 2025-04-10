using UnityEngine;
using static UnityEditor.Progress;

public abstract class InteractableObject : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;
    protected bool isEquipped = false;

    protected PlayerController player;
    public string itemDescription;

    public enum CarryType { None, Mask, NPC, Item }
    public enum UseTrigger { KeyF,RightClick,OnEquip}

    [Header("Я������")]
    public CarryType carryType = CarryType.None;
    private SpriteRenderer spriteRenderer;


    [Header("��Ʒ����")]
    public bool destroyOnUse = false;
    public string poolTag = "Items";
    public GameObject prefabReference;
    public UseTrigger useTrigger;
    public bool isContinuousUse = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"{gameObject.name} ȱ�� SpriteRenderer �����");
            enabled = false; 
        }
    }
    protected virtual void Start()
    {
        player = InventorySystem.Instance?.GetPlayer();
        if (player == null)
        {
            Debug.LogWarning("PlayerController δ��ȷ���ã���������ʧ�ܣ�");
        }
    }

    public virtual void OnInteract()
    {
        if (InventorySystem.Instance.AddItem(this))
        {
            gameObject.SetActive(false); // ���õ�ǰʵ��
        }
    }

    public virtual void OnEquip(Transform parent)
    {
        if (isEquipped) return;
        isEquipped = true;
        transform.SetParent(parent);
        transform.localPosition = Vector2.zero;
        transform.localRotation = Quaternion.identity;
        HandleEquip();
    }

    public virtual void OnUnequip()
    {
        if (!isEquipped) return;

        isEquipped = false;
        gameObject.SetActive(false);
        HandleUnequip();
    }

    public virtual void UseItem()
    {
        if (!isEquipped)
        {
            Debug.LogWarning($"δװ�� {itemName}���޷�ʹ�ã�");
            return;
        }

        HandleUse();

        if (destroyOnUse)
        {
            Debug.Log("����һ������Ʒ");

            if (player.equippedItem == this)
                player.equippedItem = null;
            Destroy(gameObject);
            InventorySystem.Instance.UpdateEquippedUI();
        }
        else if(!isContinuousUse)
        {
            Debug.Log("��һ������Ʒ�ɼ���ʹ��");
        }
    }

    public virtual void ResetItemState()
    {
        // ��������״̬
        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // ������ײ��
        if (TryGetComponent<Collider2D>(out var col))
        {
            col.enabled = true;
        }

        // ������Ⱦ��
        if (TryGetComponent<SpriteRenderer>(out var sr))
        {
            sr.enabled = true;
        }
    }

    void OnValidate()
    {
        if (prefabReference == null)
        {
            // �Զ���ȡԤ��������
            var prefab = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            if (prefab != null)
            {
                prefabReference = prefab;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
    }

    protected virtual void HandleEquip() { }
    protected virtual void HandleUnequip() { }
    protected virtual void HandleUse() { }

    public void SetEquippedLayer()
    {
        spriteRenderer.sortingLayerName = "AboveCharacter";
    }

    public void ResetLayer()
    {
        spriteRenderer.sortingLayerName = "Interactables";
    }
    public string GetDescription()
    {
        return $"����: {itemName}\n����: {itemDescription}";
    }

    void OnDrawGizmos()
    {
        if (this.gameObject.activeInHierarchy) // ȷ����Ʒ���ڻ״̬
        {
            Gizmos.color = Color.red; // ������ɫΪ��ɫ
            Gizmos.DrawSphere(transform.position, 0.2f); // ����Ʒλ�û�������
        }
    }

}

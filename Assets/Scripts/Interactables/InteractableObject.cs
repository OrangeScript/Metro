using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;
    public ItemCategory category = ItemCategory.Equipment;
    protected bool isEquipped = false;

    protected PlayerController player;

    public enum ItemCategory { Mask, Equipment }

    [Header("��Ʒ����")]
    public bool destroyOnUse = false; 

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
        if (InventorySystem.Instance == null)
        {
            Debug.LogWarning("InventorySystem δ��ʼ�����޷�������");
            return;
        }

        if (category == ItemCategory.Equipment || category == ItemCategory.Mask)
        {
            if (InventorySystem.Instance.AddItem(this))
            {
                gameObject.SetActive(false); // ������Ʒ
                Debug.Log($"{itemName} ��ʰȡ�����뱳����");
            }
            else
            {
                Debug.Log($"{itemName} �޷�ʰȡ������������");
            }
        }
    }

    public virtual void OnEquip(Transform parent)
    {
        if (isEquipped) return;

        isEquipped = true;
        transform.SetParent(parent);
        transform.localPosition = Vector2.zero;
        gameObject.SetActive(true);
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
            InventorySystem.Instance.RemoveItem(this);
            Destroy(gameObject);
        }
        else
        {
            InventorySystem.Instance.AddItem(this);
            gameObject.SetActive(false);
        }
    }

    protected virtual void HandleEquip() { }
    protected virtual void HandleUnequip() { }
    protected virtual void HandleUse() { }
}

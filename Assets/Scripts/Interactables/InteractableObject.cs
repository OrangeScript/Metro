using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;
    public ItemCategory category;
    protected bool isEquipped = false;

    public enum ItemCategory { Mask, Equipment }

    [Header("��Ʒ����")]
    public bool destroyOnUse = false; // �Ƿ�ʹ�ú�����

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
        isEquipped = false;
        gameObject.SetActive(false);
        HandleUnequip();
    }

    public virtual bool CanBePickedUp()
    {
        return true; // Ĭ�Ͽ��Ա�ʰȡ
    }

    public virtual void OnInteract()
    {
        if (CanBePickedUp() && InventorySystem.Instance != null)
        {
            InventorySystem.Instance.AddItem(this);
            gameObject.SetActive(false);
        }
    }

    public virtual void UseItem()
    {
        if (isEquipped)
        {
            HandleUse();
            if (destroyOnUse)
            {
                Destroy(gameObject); // ֱ������
            }
            else
            {
                InventorySystem.Instance.AddItem(this); // ���´��뱳��
                gameObject.SetActive(false); // ������Ʒ
            }
        }
    }

    protected virtual void Start() { }
    protected virtual void HandleEquip() { }
    protected virtual void HandleUnequip() { }
    protected virtual void HandleUse() { }
}

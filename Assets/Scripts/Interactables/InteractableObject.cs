using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;
    public ItemCategory category;
    protected bool isEquipped = false;

    public enum ItemCategory { Mask, Equipment }

    [Header("物品属性")]
    public bool destroyOnUse = false; // 是否使用后销毁

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
        return true; // 默认可以被拾取
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
                Destroy(gameObject); // 直接销毁
            }
            else
            {
                InventorySystem.Instance.AddItem(this); // 重新存入背包
                gameObject.SetActive(false); // 隐藏物品
            }
        }
    }

    protected virtual void Start() { }
    protected virtual void HandleEquip() { }
    protected virtual void HandleUnequip() { }
    protected virtual void HandleUse() { }
}

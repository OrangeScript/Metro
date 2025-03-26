using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;
    public ItemCategory category=ItemCategory.Equipment;
    protected bool isEquipped = false;

    protected PlayerController player;

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


    public virtual void OnInteract()
    {
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.AddItem(this);
            gameObject.SetActive(false);
            Debug.Log("物品已拾取！");
        }
    }

    public virtual void UseItem()
    {
            HandleUse();
            if (destroyOnUse)
            {
            InventorySystem.Instance.RemoveItem(this);
            Destroy(gameObject); // 直接销毁
            }
            else
            {
                InventorySystem.Instance.AddItem(this); // 重新存入背包
                gameObject.SetActive(false); // 隐藏物品
            }
    }

    protected virtual void Start() { }
    protected virtual void HandleEquip() { }
    protected virtual void HandleUnequip() { }
    protected virtual void HandleUse() { }
}

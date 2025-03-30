using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;
    public ItemCategory category = ItemCategory.Equipment;
    protected bool isEquipped = false;

    protected PlayerController player;

    public enum ItemCategory { Mask, Equipment }

    [Header("物品属性")]
    public bool destroyOnUse = false; 

    protected virtual void Start()
    {
        player = InventorySystem.Instance?.GetPlayer();
        if (player == null)
        {
            Debug.LogWarning("PlayerController 未正确设置，交互可能失败！");
        }
    }

    public virtual void OnInteract()
    {
        if (InventorySystem.Instance == null)
        {
            Debug.LogWarning("InventorySystem 未初始化，无法交互！");
            return;
        }

        if (category == ItemCategory.Equipment || category == ItemCategory.Mask)
        {
            if (InventorySystem.Instance.AddItem(this))
            {
                gameObject.SetActive(false); // 隐藏物品
                Debug.Log($"{itemName} 已拾取并存入背包！");
            }
            else
            {
                Debug.Log($"{itemName} 无法拾取，背包已满！");
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
            Debug.LogWarning($"未装备 {itemName}，无法使用！");
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

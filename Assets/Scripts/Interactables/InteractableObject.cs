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

    [Header("携带设置")]
    public CarryType carryType = CarryType.None;
    private SpriteRenderer spriteRenderer;


    [Header("物品属性")]
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
            Debug.LogError($"{gameObject.name} 缺少 SpriteRenderer 组件！");
            enabled = false; 
        }
    }
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
        if (InventorySystem.Instance.AddItem(this))
        {
            gameObject.SetActive(false); // 禁用当前实例
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
            Debug.LogWarning($"未装备 {itemName}，无法使用！");
            return;
        }

        HandleUse();

        if (destroyOnUse)
        {
            Debug.Log("销毁一次性物品");

            if (InventorySystem.Instance.equippedItem == this)
                InventorySystem.Instance.equippedItem = null;
            Destroy(gameObject);
            InventorySystem.Instance.UpdateEquippedUI();
        }
        else if(!isContinuousUse)
        {
            Debug.Log("非一次性物品可继续使用");
        }
    }

    public virtual void ResetItemState()
    {
        // 重置物理状态
        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // 重置碰撞体
        if (TryGetComponent<Collider2D>(out var col))
        {
            col.enabled = true;
        }

        // 重置渲染器
        if (TryGetComponent<SpriteRenderer>(out var sr))
        {
            sr.enabled = true;
        }
    }

    void OnValidate()
    {
        if (prefabReference == null)
        {
            // 自动获取预制体引用
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
        return $"名称: {itemName}\n描述: {itemDescription}";
    }

    void OnDrawGizmos()
    {
        if (this.gameObject.activeInHierarchy) // 确保物品处于活动状态
        {
            Gizmos.color = Color.red; // 设置颜色为红色
            Gizmos.DrawSphere(transform.position, 0.2f); // 在物品位置绘制球体
        }
    }

}

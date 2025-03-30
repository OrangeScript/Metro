using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    public Transform itemContainer; // 背包物品容器
    public Transform equippedContainer; // 装备栏容器
    public List<InteractableObject> items = new List<InteractableObject>(); // 背包中的物品
    public List<InteractableObject> equippedItems = new List<InteractableObject>(); // 装备栏中的物品
    private PlayerController player;
    private int maxEquippedItems = 5; 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        player = GetComponent<PlayerController>();
    }

    // 向背包中添加物品
    public bool AddItem(InteractableObject newItem)
    {
        if (newItem == null) return false;

        if (items.Count < 100) 
        {
            items.Add(newItem);
            newItem.transform.SetParent(itemContainer);
            newItem.gameObject.SetActive(true);
            return true;
        }
        return false;
    }

    // 从背包中移除物品
    public void RemoveItem(InteractableObject item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            item.transform.SetParent(null);
            item.gameObject.SetActive(false);
        }
    }

    // 将物品装备到装备栏
    public void EquipItem(InteractableObject item)
    {
        if (equippedItems.Count >= maxEquippedItems)
        {
            Debug.LogWarning("装备栏已满！");
            return;
        }

        if (!items.Contains(item)) return;

        equippedItems.Add(item);
        player.EquipItem(item);
        UpdateEquippedItemsUI(); // 更新装备栏 UI
    }

    // 从装备栏中卸下物品
    public void UnequipItem(InteractableObject item)
    {
        if (equippedItems.Contains(item))
        {
            equippedItems.Remove(item);
            item.OnUnequip();
            UpdateEquippedItemsUI(); // 更新装备栏 UI
        }
    }

    // 获取玩家对象
    public PlayerController GetPlayer()
    {
        return player;
    }

    // 设置玩家对象
    public void SetPlayer(PlayerController player)
    {
        this.player = player;
    }

    // 更新装备栏 UI
    private void UpdateEquippedItemsUI()
    {
        // 更新装备栏的显示内容，具体需要根据UI实现来进行更新
        // 假设你有UI显示物品的逻辑，这里可以添加更新装备栏UI的方法
    }
}

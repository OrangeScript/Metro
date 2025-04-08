using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Header("背包设置")]
    public GameObject inventoryPanel;
    public Transform itemContainer; //背包物品容器
    public int maxInventorySlots = 50;
    public List<InteractableObject> items = new List<InteractableObject>(); //背包中的物品
    public GameObject inventorySlotPrefab;
    public Sprite background;
    private ItemSlotUI hoveredSlot;
    bool isInventoryOpen = false;

    [Header("装备栏设置")]
    public Transform equippedItemsContainer;//装备容器
    public InteractableObject equippedMask; // Mask装备槽
    public InteractableObject equippedItem; // Item装备槽
    private PlayerController player;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        player = GetComponent<PlayerController>();
    }

    private void Start()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(isInventoryOpen);
        else
            Debug.LogError("未绑定InventoryPanel!");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
            return; 
        }

        if (isInventoryOpen)
        {
            HandleEquipmentInput();
            Time.timeScale = 0;
        }
        else
        {
            player.HandleInteractionInput();
            player.HandleUseItemInput();
            Time.timeScale = 1; 
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isInventoryOpen);
            Debug.Log($"背包状态：{(isInventoryOpen ? "打开" : "关闭")}"); 
        }
    }

    public void HandleEquipmentInput()
    {
        if (hoveredSlot != null && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("装备：" + hoveredSlot.item.name);
            InteractableObject itemToEquip = hoveredSlot.GetItem();
            if (itemToEquip != null)
            {
                EquipItem(itemToEquip);
            }
        }
    }

    public bool AddItem(InteractableObject item)
    {
        GameObject slot = ObjectPoolManager.Instance.GetObject(inventorySlotPrefab);
        slot.transform.SetParent(itemContainer);

        ItemSlotUI slotUI = slot.GetComponent<ItemSlotUI>();
        slotUI.LoadItem(item);

        items.Add(item);
        UpdateInventoryUI();
        return true;
    }

    public void RemoveItem(InteractableObject item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            ObjectPoolManager.Instance.ReturnObject(item.gameObject, item.gameObject);
            UpdateInventoryUI();
        }
    }
    public void EquipItem(InteractableObject item)
    {
        if (!items.Contains(item)) return;

        // 获取预制体引用（确保InteractableObject有prefab字段）
        GameObject prefab = item.prefabReference;

        // 从对象池获取新实例
        GameObject newObj = ObjectPoolManager.Instance.GetObject(prefab);
        InteractableObject newItem = newObj.GetComponent<InteractableObject>();

        // 装备新实例
        switch (newItem.carryType)
        {
            case InteractableObject.CarryType.Mask:
                if (equippedMask != null) UnequipItem(equippedMask);
                equippedMask = newItem;
                break;

            case InteractableObject.CarryType.Item:
                if (equippedItem != null) UnequipItem(equippedItem);
                equippedItem = newItem;
                break;
        }

        // 处理原始实例
        items.Remove(item);
        ObjectPoolManager.Instance.ReturnObject(prefab, item.gameObject); // 返回原实例到对象池

        // 应用效果到新实例
        ApplyEquipmentEffects(newItem);
        UpdateUI();
        newItem.gameObject.SetActive(true);
    }



    private void UpdateUI()
    {
        UpdateInventoryUI();
        UpdateEquippedUI();
    }


    // 修改后的UnequipItem方法
    public void UnequipItem(InteractableObject item)
    {
        if (item == null) return;

        item.ResetLayer();
        switch (item.carryType)
        {
            case InteractableObject.CarryType.Mask:
                if (equippedMask == item)
                {
                    DropItemToWorld(item); // 直接丢弃到地图
                    equippedMask = null;
                }
                break;

            case InteractableObject.CarryType.Item:
                if (equippedItem == item)
                {
                    DropItemToWorld(item); // 直接丢弃到地图
                    equippedItem = null;
                }
                break;
        }

        RemoveEquipmentEffects(item);
        UpdateInventoryUI();
        UpdateEquippedUI();
    }

    public void DropItemToWorld(InteractableObject item)
    {
        // 获取原始预制体
        GameObject prefab = item.gameObject;

        // 从对象池获取新实例
        GameObject newObj = ObjectPoolManager.Instance.GetObject(prefab);
        newObj.transform.position = player.transform.position;
        newObj.SetActive(true);

        // 从背包移除
        items.Remove(item);
        UpdateInventoryUI();
    }


    private void ApplyEquipmentEffects(InteractableObject item)
    {
       item.gameObject.SetActive(true);
       item.SetEquippedLayer();
       player.EquipItem(item);
       Debug.Log($"[装备调试] 装备物品: {item.name}，" +
           $"位置: {item.transform.position}，" +
            $"缩放: {item.transform.localScale}" +
            $"父物体: {(item.transform.parent != null ? item.transform.parent.name : "null")}，" +
            $"Sorting Layer: {item.GetComponent<SpriteRenderer>()?.sortingLayerName}");
    }

    private void RemoveEquipmentEffects(InteractableObject item)
    {
        player.UnequipItem(item.carryType);
    }

    private void UpdateInventoryUI()
    {
        items.RemoveAll(item => item == null); 

        var existingSlots = itemContainer.GetComponentsInChildren<ItemSlotUI>(true).ToList(); 

        for (int i = 0; i < Mathf.Max(items.Count, existingSlots.Count); i++)
        {
            if (i < items.Count)
            {
                ItemSlotUI slot;
                if (i < existingSlots.Count)
                {
                    slot = existingSlots[i]; 
                }
                else
                {
                    GameObject slotObj = ObjectPoolManager.Instance.GetObject(inventorySlotPrefab);
                    slot = slotObj.GetComponent<ItemSlotUI>();
                    slot.transform.SetParent(itemContainer); 
                }

                slot.LoadItem(items[i]); 
                slot.gameObject.SetActive(true); 
            }
            else
            {
                existingSlots[i].gameObject.SetActive(false); 
                ObjectPoolManager.Instance.ReturnObject(inventorySlotPrefab, existingSlots[i].gameObject); // 将槽位返回到对象池
            }
        }
        StartCoroutine(DelayedLayoutRefresh()); 
    }

    private IEnumerator DelayedLayoutRefresh()
    {
        yield return null; 
        if (itemContainer.TryGetComponent<GridLayoutGroup>(out var layout))
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                layout.GetComponent<RectTransform>());
        }
    }



    public void SetHoveredSlot(ItemSlotUI slot)
    {
        hoveredSlot = slot;
    }

    public void UpdateEquippedUI()
    {
        foreach (Transform child in equippedItemsContainer)
            Destroy(child.gameObject);

        // 生成装备槽
        if (equippedMask != null)
            CreateEquippedSlot(equippedMask);
        if (equippedItem != null)
            CreateEquippedSlot(equippedItem);
    }

    private void CreateEquippedSlot(InteractableObject item)
    {
        GameObject slot = Instantiate(inventorySlotPrefab, equippedItemsContainer);
        ItemSlotUI slotUI = slot.GetComponent<ItemSlotUI>();
        slotUI.LoadItem(item);
    }


    public PlayerController GetPlayer()
    {
        return player;
    }

    // 设置玩家对象
    public void SetPlayer(PlayerController player)
    {
        this.player = player;
    }
}

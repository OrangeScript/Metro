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
    public bool isInventoryOpen = false;
    public bool isFirstAdd = false;
    public bool isFirstEquipped = false;

    [Header("装备栏设置")]
    public Transform equippedItemsContainer;//装备容器
    private PlayerController player;

    [Header("提示设置")]
    public string firstAddText;
    public string firstEquipText;


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
            HandleUnequipment();
            HandleReturnEquipment();
        }
    }

    public PlayerController GetPlayer()
    {
        return player;
    }

    public void SetPlayer(PlayerController player)
    {
        this.player = player;
    }

    #region 背包操作
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
    #endregion

    #region 装备物品
    public bool AddItem(InteractableObject item)
    {
        item.gameObject.SetActive(false);
        item.transform.SetParent(itemContainer);

        ItemSlotUI slot = ObjectPoolManager.Instance
            .GetObject(inventorySlotPrefab)
            .GetComponent<ItemSlotUI>();
        slot.LoadItem(item);

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
        if(isFirstEquipped==false) {
            isFirstEquipped=true;
            ShowEquipTips();
        }
        item.gameObject.SetActive(false);

        InteractableObject equipTarget = item;

        switch (equipTarget.carryType)
        {
            case InteractableObject.CarryType.Mask:
                if (player.equippedMask != null)
                    ReturnToInventory(player.equippedMask);
                player.equippedMask = equipTarget;
                break;

            case InteractableObject.CarryType.Item:
                if (player.equippedItem != null)
                    ReturnToInventory(player.equippedItem);
                player.equippedItem = equipTarget;
                break;
        }

        //从背包移动到装备状态
        items.Remove(item);
        ApplyEquipmentEffects(equipTarget);
        UpdateUI();
    }



    public void UnequipItem(InteractableObject item)
    {
        if (item == null) return;

        // 处理物品返回对象池
        ReturnItemToPool(item);

        // 清除玩家引用
        switch (item.carryType)
        {
            case InteractableObject.CarryType.Mask:
                if (player.equippedMask == item)
                    player.equippedMask = null;
                break;

            case InteractableObject.CarryType.Item:
                if (player.equippedItem == item)
                    player.equippedItem = null;
                break;
        }
        DropItemToWorld(item);
        RemoveEquipmentEffects(item);
        UpdateUI();
    }
    public void HandleUnequipment()
    {
        if (Input.GetKeyDown(KeyCode.Q) && player.equippedItem != null)
        {
            UnequipItem(player.equippedItem);
        }
        if (Input.GetKeyDown(KeyCode.Q) && player.equippedMask != null)
        {
            UnequipItem(player.equippedMask);
        }
    }

    public void HandleReturnEquipment()
    {
        if (Input.GetKeyDown(KeyCode.E) && player.equippedItem != null)
        {
            ReturnToInventory(player.equippedItem);
        }
        if (Input.GetKeyDown(KeyCode.E) && player.equippedMask != null)
        {
            ReturnToInventory(player.equippedMask);
        }
    }
    public void ReturnToInventory(InteractableObject item)
    {
        if (item == null) return;

        if (!items.Contains(item))
        {
            UnequipItem(item);
            AddItem(item);

            UpdateUI();
        }
        else
        {
            Debug.LogWarning("该物品已在背包中！");
        }
    }


    private void ReturnItemToPool(InteractableObject item)
    {
        item.gameObject.SetActive(true);
        item.transform.SetParent(null);
        item.ResetLayer();
        item.OnUnequip();

        item.transform.position = player.transform.position;

    }

    public void DropItemToWorld(InteractableObject item)
    {
        // 直接启用现有实例
        item.gameObject.SetActive(true);
        item.transform.position = player.transform.position;

 
        items.Remove(item);
        UpdateInventoryUI();
    }
    #endregion

    #region 效果显示
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
    private void UpdateUI()
    {
        UpdateInventoryUI();
        UpdateEquippedUI();
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
        if (player.equippedMask != null)
            CreateEquippedSlot(player.equippedMask);
        if (player.equippedItem != null)
            CreateEquippedSlot(player.equippedItem);
    }

    private void CreateEquippedSlot(InteractableObject item)
    {
        GameObject slot = Instantiate(inventorySlotPrefab, equippedItemsContainer);
        ItemSlotUI slotUI = slot.GetComponent<ItemSlotUI>();
        slotUI.LoadItem(item);
    }
    #endregion

    #region 提示显示
    public void ShowAddTips()
    {
        if (isFirstAdd)
        {
            UIManager.Instance.ShowTips(firstAddText);
        }
    }

    public void ShowEquipTips()
    {
        if (isFirstEquipped)
        {
            UIManager.Instance.ShowTips(firstEquipText);
        }
    }
    #endregion

}
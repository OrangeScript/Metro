using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Header("��������")]
    public GameObject inventoryPanel;
    public Transform itemContainer; //������Ʒ����
    public int maxInventorySlots = 50;
    public List<InteractableObject> items = new List<InteractableObject>(); //�����е���Ʒ
    public GameObject inventorySlotPrefab;
    public Sprite background;
    private ItemSlotUI hoveredSlot;
    public bool isInventoryOpen = false;
    public bool isFirstAdd = false;
    public bool isFirstEquipped = false;

    [Header("װ��������")]
    public Transform equippedItemsContainer;//װ������
    private PlayerController player;

    [Header("��ʾ����")]
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
            Debug.LogError("δ��InventoryPanel!");
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

    #region ��������
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isInventoryOpen);
            Debug.Log($"����״̬��{(isInventoryOpen ? "��" : "�ر�")}");
        }
    }

    public void HandleEquipmentInput()
    {
        if (hoveredSlot != null && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("װ����" + hoveredSlot.item.name);
            InteractableObject itemToEquip = hoveredSlot.GetItem();
            if (itemToEquip != null)
            {
                EquipItem(itemToEquip);
            }
        }
    }
    #endregion

    #region װ����Ʒ
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

        //�ӱ����ƶ���װ��״̬
        items.Remove(item);
        ApplyEquipmentEffects(equipTarget);
        UpdateUI();
    }



    public void UnequipItem(InteractableObject item)
    {
        if (item == null) return;

        // ������Ʒ���ض����
        ReturnItemToPool(item);

        // ����������
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
            Debug.LogWarning("����Ʒ���ڱ����У�");
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
        // ֱ����������ʵ��
        item.gameObject.SetActive(true);
        item.transform.position = player.transform.position;

 
        items.Remove(item);
        UpdateInventoryUI();
    }
    #endregion

    #region Ч����ʾ
    private void ApplyEquipmentEffects(InteractableObject item)
    {
        item.gameObject.SetActive(true);
        item.SetEquippedLayer();
        player.EquipItem(item);
        Debug.Log($"[װ������] װ����Ʒ: {item.name}��" +
            $"λ��: {item.transform.position}��" +
             $"����: {item.transform.localScale}" +
             $"������: {(item.transform.parent != null ? item.transform.parent.name : "null")}��" +
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
                ObjectPoolManager.Instance.ReturnObject(inventorySlotPrefab, existingSlots[i].gameObject); // ����λ���ص������
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

        // ����װ����
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

    #region ��ʾ��ʾ
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
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
    bool isInventoryOpen = false;

    [Header("װ��������")]
    public Transform equippedItemsContainer;//װ������
    public InteractableObject equippedMask; // Maskװ����
    public InteractableObject equippedItem; // Itemװ����
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
        }
    }

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

        // ��ȡԤ�������ã�ȷ��InteractableObject��prefab�ֶΣ�
        GameObject prefab = item.prefabReference;

        // �Ӷ���ػ�ȡ��ʵ��
        GameObject newObj = ObjectPoolManager.Instance.GetObject(prefab);
        InteractableObject newItem = newObj.GetComponent<InteractableObject>();

        // װ����ʵ��
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

        // ����ԭʼʵ��
        items.Remove(item);
        ObjectPoolManager.Instance.ReturnObject(prefab, item.gameObject); // ����ԭʵ���������

        // Ӧ��Ч������ʵ��
        ApplyEquipmentEffects(newItem);
        UpdateUI();
        newItem.gameObject.SetActive(true);
    }



    private void UpdateUI()
    {
        UpdateInventoryUI();
        UpdateEquippedUI();
    }


    // �޸ĺ��UnequipItem����
    public void UnequipItem(InteractableObject item)
    {
        if (item == null) return;

        item.ResetLayer();
        switch (item.carryType)
        {
            case InteractableObject.CarryType.Mask:
                if (equippedMask == item)
                {
                    DropItemToWorld(item); // ֱ�Ӷ�������ͼ
                    equippedMask = null;
                }
                break;

            case InteractableObject.CarryType.Item:
                if (equippedItem == item)
                {
                    DropItemToWorld(item); // ֱ�Ӷ�������ͼ
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
        // ��ȡԭʼԤ����
        GameObject prefab = item.gameObject;

        // �Ӷ���ػ�ȡ��ʵ��
        GameObject newObj = ObjectPoolManager.Instance.GetObject(prefab);
        newObj.transform.position = player.transform.position;
        newObj.SetActive(true);

        // �ӱ����Ƴ�
        items.Remove(item);
        UpdateInventoryUI();
    }


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

    // ������Ҷ���
    public void SetPlayer(PlayerController player)
    {
        this.player = player;
    }
}

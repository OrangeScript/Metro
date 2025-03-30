using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    public Transform itemContainer; // ������Ʒ����
    public Transform equippedContainer; // װ��������
    public List<InteractableObject> items = new List<InteractableObject>(); // �����е���Ʒ
    public List<InteractableObject> equippedItems = new List<InteractableObject>(); // װ�����е���Ʒ
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

    // �򱳰��������Ʒ
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

    // �ӱ������Ƴ���Ʒ
    public void RemoveItem(InteractableObject item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            item.transform.SetParent(null);
            item.gameObject.SetActive(false);
        }
    }

    // ����Ʒװ����װ����
    public void EquipItem(InteractableObject item)
    {
        if (equippedItems.Count >= maxEquippedItems)
        {
            Debug.LogWarning("װ����������");
            return;
        }

        if (!items.Contains(item)) return;

        equippedItems.Add(item);
        player.EquipItem(item);
        UpdateEquippedItemsUI(); // ����װ���� UI
    }

    // ��װ������ж����Ʒ
    public void UnequipItem(InteractableObject item)
    {
        if (equippedItems.Contains(item))
        {
            equippedItems.Remove(item);
            item.OnUnequip();
            UpdateEquippedItemsUI(); // ����װ���� UI
        }
    }

    // ��ȡ��Ҷ���
    public PlayerController GetPlayer()
    {
        return player;
    }

    // ������Ҷ���
    public void SetPlayer(PlayerController player)
    {
        this.player = player;
    }

    // ����װ���� UI
    private void UpdateEquippedItemsUI()
    {
        // ����װ��������ʾ���ݣ�������Ҫ����UIʵ�������и���
        // ��������UI��ʾ��Ʒ���߼������������Ӹ���װ����UI�ķ���
    }
}

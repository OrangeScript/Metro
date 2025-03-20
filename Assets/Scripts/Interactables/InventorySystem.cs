using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    public Transform itemContainer;
    private List<InteractableObject> items = new List<InteractableObject>();
    private int _currentIndex = -1;
    private PlayerController player;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        player = GetComponent<PlayerController>(); // 自动获取组件
    }

    public bool AddItem(InteractableObject newItem)
    {
        if (newItem == null || items.Contains(newItem)) return false;

        if (items.Count < 100)
        {
            items.Add(newItem);
            newItem.transform.SetParent(itemContainer);
            newItem.gameObject.SetActive(false);
            if (_currentIndex == -1) SwitchItem(0);
            return true;
        }
        return false;
    }

    public void SwitchItem(int direction)
    {
        if (items.Count == 0) return;

        if (_currentIndex != -1)
            items[_currentIndex].OnUnequip();

        _currentIndex = (_currentIndex + direction + items.Count) % items.Count;
        player.EquipItem(items[_currentIndex]);
    }

    public void RemoveItem(InteractableObject item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            item.transform.SetParent(null);
            item.gameObject.SetActive(true);
        }
    }

    public void SetPlayer(PlayerController player)
    {
        this.player = player; 
        Debug.Log("Player set in inventory system.");
    }
}

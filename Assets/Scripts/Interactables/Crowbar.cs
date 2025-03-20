using System.Collections.Generic;
using UnityEngine;

public class Crowbar : InteractableObject
{
    [Header("«Àπ˜…Ë÷√")]
    public float openRadius = 1.5f;

    private List<MetroDoor> nearbyDoors = new List<MetroDoor>();

    public override void OnInteract() 
    {
        if (!isEquipped)
        {
            if (InventorySystem.Instance != null)
            {
                InventorySystem.Instance.AddItem(this);
                gameObject.SetActive(false);
            }
        }
        else
        {
            TryOpenDoor();
        }
    }

    void TryOpenDoor()
    {
        foreach (var door in nearbyDoors)
        {
            if (door.currentState == MetroDoor.DoorState.Jammed)
            {
                door.OpenDoor();
                
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        MetroDoor door = other.GetComponent<MetroDoor>();
        if (door != null && !nearbyDoors.Contains(door))
        {
            nearbyDoors.Add(door);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        MetroDoor door = other.GetComponent<MetroDoor>();
        if (door != null)
        {
            nearbyDoors.Remove(door);
        }
    }

    public override void OnEquip(Transform parent)
    {
        base.OnEquip(parent);
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public override void OnUnequip()
    {
        base.OnUnequip();
        GetComponent<SpriteRenderer>().enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : InteractableObject
{
    private PlayerController player;

    protected override void Start()
    {
        base.Start();
        player = GameManager.Instance.player;
        destroyOnUse = true;
    }

    protected override void HandleUse()
    {
        if (player == null) return;
        if (player.nearestInteractable != null && player.nearestInteractable.CompareTag("MetroDoor"))
        {
            MetroDoor door = player.nearestInteractable.GetComponent<MetroDoor>();
            if (door != null)
            {
                door.currentState = MetroDoor.DoorState.Open;
                Debug.Log("车门已通电！");

                InventorySystem.Instance.RemoveItem(this);
            }
        }
    }
}

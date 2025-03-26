using System.Collections.Generic;
using UnityEngine;

public class Crowbar : InteractableObject
{

    protected override void Start()
    {
        base.Start();
        destroyOnUse = false;
    }

    protected override void HandleUse()
    {
        if (player == null) return;
        if (player.nearestInteractable != null && player.nearestInteractable.CompareTag("MetroDoor"))
        {
            MetroDoor door = player.nearestInteractable.GetComponent<MetroDoor>();
            if (door != null && ((door.currentFault == MetroDoor.FaultType.Type1) || (door.currentFault == MetroDoor.FaultType.Type2)))
            {
                Debug.Log("车门已打开！");
                door.OpenDoor();
            }
        }
    }
}

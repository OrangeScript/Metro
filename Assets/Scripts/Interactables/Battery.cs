using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : InteractableObject
{
    public static bool isPowered=false;
    protected override void Start()
    {
        base.Start();
        destroyOnUse = true;
    }

    protected override void HandleUse()
    {
        if (player == null) return;
        if (player.nearestInteractable != null && player.nearestInteractable.CompareTag("MetroDoor"))
        {
            MetroDoor door = player.nearestInteractable.GetComponent<MetroDoor>();
            if (door != null&&((door.currentFault==MetroDoor.FaultType.Type3)|| (door.currentFault == MetroDoor.FaultType.Type4) ||
                (door.currentFault == MetroDoor.FaultType.Type5)))
            {
                isPowered = true;
                Debug.Log("车门已通电！");
            }
        }
    }
}

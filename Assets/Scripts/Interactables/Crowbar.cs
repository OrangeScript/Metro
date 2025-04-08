using System.Collections.Generic;
using UnityEngine;

public class Crowbar : InteractableObject
{

    protected override void Start()
    {
        base.Start();
        destroyOnUse = false;
        useTrigger = UseTrigger.KeyF;
    }

    protected override void HandleUse()
    {
        if (player == null) return;
        MetroDoor door = player.nearestMetroDoor;

        if (door != null &&
            (door.currentFault == MetroDoor.FaultType.Type1 ||
             door.currentFault == MetroDoor.FaultType.Type2))
        {
            Debug.Log("车门已打开！");
            door.OpenDoor();
        }
        else
        {
            Debug.LogError("Can't find MetroDoor");
        }
    }
}
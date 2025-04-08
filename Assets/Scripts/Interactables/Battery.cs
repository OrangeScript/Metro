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
        useTrigger = UseTrigger.KeyF;
    }

    protected override void HandleUse()
    {
        if (player == null) return;
        MetroDoor door =  player.nearestMetroDoor;

        if (Input.GetKeyDown(KeyCode.F) && door != null)
        {
            if (door.currentFault == MetroDoor.FaultType.Type3)
            {
                isPowered = true;
                Debug.Log("车门已通电！");
                door.currentFault = MetroDoor.FaultType.Type1;
            }
            else if (door.currentFault == MetroDoor.FaultType.Type4)
            {
                isPowered = true;
                Debug.Log("车门已通电！");
                door.currentFault = MetroDoor.FaultType.Type2;
            }
            else if (door.currentFault == MetroDoor.FaultType.Type5)
            {
                isPowered = true;
                Debug.Log("车门已通电！");
                door.currentFault = MetroDoor.FaultType.Type2;
                door.StartMazePuzzleWithNoChange();
            }

            Debug.Log($"当前故障类型为：{door.currentFault}");
            door.TryInteract(player);
        }
    }
}

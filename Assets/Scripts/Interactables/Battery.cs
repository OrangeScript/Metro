
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : InteractableObject
{
    public static bool isPowered = false;
    public MetroDoor door;
    protected override void Start()
    {
        base.Start();
        destroyOnUse = true;
        useTrigger = UseTrigger.KeyF;
    }


    protected override void HandleUse()
    {
        if (player == null) return;
        MetroDoor door = player.nearestMetroDoor;

        if (!isPowered && door != null)
        {
            if (door.currentFault == MetroDoor.FaultType.Type3 ||
                door.currentFault == MetroDoor.FaultType.Type4 ||
                door.currentFault == MetroDoor.FaultType.Type5)
            {
                isPowered = true;
                Debug.Log("车门已通电，请再次按下 F 以修复故障");
                UIManager.Instance.ShowMessage("车门已通电，请再次按下 F 以修复故障");
                player.awaitingSecondFPress = true;
                player.poweredDoor = door;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GasMask : InteractableObject
{
    public override void OnUnequip()
    {
        base.OnUnequip();
        if (player.equippedMask == this)
        {
            player.equippedMask = null; 
        }
    }
}

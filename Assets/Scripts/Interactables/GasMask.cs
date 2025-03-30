using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GasMask : InteractableObject
{
    private void Awake()
    {
        category = ItemCategory.Mask;
    }

    protected override void Start()
    {
        base.Start();
        destroyOnUse = false;
    }

    public override void OnEquip(Transform parent)
    {
        base.OnEquip(parent);
        player.equippedMask = this;
    }

    public override void OnUnequip()
    {
        base.OnUnequip();
        if (player.equippedMask == this)
        {
            player.equippedMask = null; 
        }
    }
}

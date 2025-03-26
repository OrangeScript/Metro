using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GasMask : InteractableObject
{
    protected override void Start()
    {
        base.Start();
        destroyOnUse = false ;
    }

    public GasMask()
    {
        category=ItemCategory.Mask;
    }

    public override void OnEquip(Transform parent)
    {
        base.OnEquip(parent);
        player.HasGasMask=true;
    }

    public override void OnUnequip()
    {
        base.OnUnequip();
        player.HasGasMask = false;
    }

}
using UnityEngine;
using System.Collections;

public class Clothing : InteractableObject
{
    protected override void Start()
    {
        base.Start();
        destroyOnUse = true;
    }

    protected override void HandleUse()
    {
        ExtinguishNearbyCombustibles();
    }

    private void ExtinguishNearbyCombustibles()
    {
        bool extinguished = false;
        if (player.nearestInteractable != null && player.nearestInteractable.CompareTag("CombustibleItem")) 
        {
            CombustibleItem targetCombustible = player.nearestInteractable.GetComponent<CombustibleItem>();
            if (targetCombustible != null && targetCombustible.isBurning)
            {
                targetCombustible.Extinguish();
                extinguished = true;
                Debug.Log("ȼ����������");
            }
        }

        if (!extinguished)
        {
            Debug.Log("��Χû��ȼ�յ���Ʒ��");
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class Flashlight : InteractableObject
{
    [Header("手电筒设置")]
    public bool isFlashlightOn = false; // 是否开启手电筒
    private bool isNearVentExit = false; // 是否靠近通风管道出口
    private bool isInVentilation = false; // 是否在通风管道内
    private GameObject actionUI; // UI 提示
    [SerializeField]private new Light light;
    protected override void Start()
    {
        base.Start();
        actionUI = GameObject.Find("ActionUI");
        if (actionUI != null) actionUI.SetActive(false);
        destroyOnUse = false;
    }

    public override void OnEquip(Transform parent)
    {
        base.OnEquip(parent);
    }

    public override void OnUnequip()
    {
        base.OnUnequip();
        isFlashlightOn = false;
    }
    public override void UseItem()
    {
        base.UseItem();
    }
    protected override void HandleUse()
    {
        ToggleFlashlight();
    }
    

    private void ToggleFlashlight()
    {
        isFlashlightOn = !isFlashlightOn;
        light.enabled = isFlashlightOn;
    }

    private void EnterVentExit()
    {
        Debug.Log("玩家进入通风管道出口");
    }

    private void ReturnToInventory()
    {
        Debug.Log("手电筒收回至背包");
        InventorySystem.Instance.AddItem(this);
        isEquipped = false;
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("VentExit"))
        {
            isNearVentExit = true;
        }
        else if (other.CompareTag("Ventilation"))
        {
            isInVentilation = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("VentExit"))
        {
            isNearVentExit = false;
        }
        else if (other.CompareTag("Ventilation"))
        {
            isInVentilation = false;
        }
    }
}

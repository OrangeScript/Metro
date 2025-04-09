using UnityEngine;
using UnityEngine.UI;

public class Flashlight : InteractableObject
{
    [Header("�ֵ�Ͳ����")]
    public bool isFlashlightOn = false; // �Ƿ����ֵ�Ͳ
    private bool isNearVentExit = false; // �Ƿ񿿽�ͨ��ܵ�����
    private bool isInVentilation = false; // �Ƿ���ͨ��ܵ���
    private GameObject actionUI; // UI ��ʾ
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
        Debug.Log("��ҽ���ͨ��ܵ�����");
    }

    private void ReturnToInventory()
    {
        Debug.Log("�ֵ�Ͳ�ջ�������");
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

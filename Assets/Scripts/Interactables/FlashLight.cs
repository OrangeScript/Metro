using UnityEngine;
using UnityEngine.UI;

public class Flashlight : InteractableObject
{
    [Header("�ֵ�Ͳ����")]
    public bool isFlashlightOn = false; // �Ƿ����ֵ�Ͳ
    [SerializeField]private new Light light;

    protected override void Start()
    {
        base.Start();
        light.gameObject.SetActive(false);
        light.enabled = true;
    }
    public override void OnUnequip()
    {
        base.OnUnequip();
        light.enabled = false;
        isFlashlightOn = false;
    }
    protected override void HandleUse()
    {
        light.gameObject.SetActive(true);
        isFlashlightOn = !isFlashlightOn;
        light.enabled = isFlashlightOn;
    }
    
}

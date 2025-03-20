using UnityEngine;
using UnityEngine.UI;

public class Flashlight : InteractableObject
{
    [Header("手电筒设置")]
    //public Light2D flashlightLight;

    public bool isFlashlightOn = false; // 是否开启手电筒
    private bool isNearVentExit = false; // 是否靠近通风管道出口
    private bool isInVentilation = false; // 是否在通风管道内
    private GameObject actionUI; // UI 提示
    private PlayerController player;

    protected override void Start()
    {
        base.Start();
        player = GameManager.Instance.player;

        // 获取 UI 组件
        actionUI = GameObject.Find("ActionUI"); // 确保你的 UI GameObject 名称正确
        if (actionUI != null) actionUI.SetActive(false);

        //flashlightLight.enabled = false; // 初始状态关闭手电筒
    }

    public override void OnInteract()
    {
        if (!isEquipped)
        {
            InventorySystem.Instance.AddItem(this);
            gameObject.SetActive(false);
        }
    }

    public override void OnEquip(Transform parent)
    {
        base.OnEquip(parent);
        ShowUI("[F] 打开");
    }

    public override void OnUnequip()
    {
        base.OnUnequip();
        //flashlightLight.enabled = false;
        isFlashlightOn = false;
        HideUI();
    }

    void Update()
    {
        if (isEquipped)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (isNearVentExit)
                {
                    EnterVentExit();
                }
                else if (isInVentilation)
                {
                    ReturnToInventory();
                }
                else
                {
                    ToggleFlashlight();
                }
            }
        }
    }

    private void ToggleFlashlight()
    {
        isFlashlightOn = !isFlashlightOn;
        //flashlightLight.enabled = isFlashlightOn;
        ShowUI(isFlashlightOn ? "[F] 关闭" : "[F] 打开");
    }

    private void EnterVentExit()
    {
        Debug.Log("玩家进入通风管道出口");
        HideUI();
        // 这里可以加切换场景或进入新区域的逻辑
    }

    private void ReturnToInventory()
    {
        Debug.Log("手电筒收回至背包");
        InventorySystem.Instance.AddItem(this);
        isEquipped = false;
        gameObject.SetActive(false);
        HideUI();
    }

    private void ShowUI(string message)
    {
        if (actionUI != null)
        {
            actionUI.GetComponent<Text>().text = message;
            actionUI.SetActive(true);
        }
    }

    private void HideUI()
    {
        if (actionUI != null)
        {
            actionUI.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("VentExit"))
        {
            isNearVentExit = true;
            ShowUI("[F] 进入出口");
        }
        else if (other.CompareTag("Ventilation"))
        {
            isInVentilation = true;
            ShowUI("[F] 收回至背包");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("VentExit"))
        {
            isNearVentExit = false;
            ShowUI("[F] 打开");
        }
        else if (other.CompareTag("Ventilation"))
        {
            isInVentilation = false;
            ShowUI("[F] 打开");
        }
    }
}

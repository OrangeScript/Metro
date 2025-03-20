using UnityEngine;
using UnityEngine.UI;

public class Flashlight : InteractableObject
{
    [Header("�ֵ�Ͳ����")]
    //public Light2D flashlightLight;

    public bool isFlashlightOn = false; // �Ƿ����ֵ�Ͳ
    private bool isNearVentExit = false; // �Ƿ񿿽�ͨ��ܵ�����
    private bool isInVentilation = false; // �Ƿ���ͨ��ܵ���
    private GameObject actionUI; // UI ��ʾ
    private PlayerController player;

    protected override void Start()
    {
        base.Start();
        player = GameManager.Instance.player;

        // ��ȡ UI ���
        actionUI = GameObject.Find("ActionUI"); // ȷ����� UI GameObject ������ȷ
        if (actionUI != null) actionUI.SetActive(false);

        //flashlightLight.enabled = false; // ��ʼ״̬�ر��ֵ�Ͳ
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
        ShowUI("[F] ��");
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
        ShowUI(isFlashlightOn ? "[F] �ر�" : "[F] ��");
    }

    private void EnterVentExit()
    {
        Debug.Log("��ҽ���ͨ��ܵ�����");
        HideUI();
        // ������Լ��л������������������߼�
    }

    private void ReturnToInventory()
    {
        Debug.Log("�ֵ�Ͳ�ջ�������");
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
            ShowUI("[F] �������");
        }
        else if (other.CompareTag("Ventilation"))
        {
            isInVentilation = true;
            ShowUI("[F] �ջ�������");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("VentExit"))
        {
            isNearVentExit = false;
            ShowUI("[F] ��");
        }
        else if (other.CompareTag("Ventilation"))
        {
            isInVentilation = false;
            ShowUI("[F] ��");
        }
    }
}

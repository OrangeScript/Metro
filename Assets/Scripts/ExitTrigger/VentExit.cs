using UnityEngine;

public class VentExit : MonoBehaviour
{
    [Header("���ڶ�Ӧ���ⲿλ��")]
    [SerializeField] private Transform targetExitPoint;

    [Header("�Ƿ���Ҫ�ֵ�Ͳ�����뿪")]
    [SerializeField] private bool requiresFlashlight = false;

    private PlayerController player;
    private bool playerInTrigger = false;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (playerInTrigger && player != null)
        {
            if (CanTeleport())
            {
                UIManager.Instance.ShowInteractUI(true, "[R]�뿪ͨ���");
                UpdateInteractUIPosition();

                if (Input.GetKeyDown(KeyCode.R))
                {
                    TeleportPlayer();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = true;
            Debug.Log("��ҿ���ͨ��ڳ���");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }

    private bool CanTeleport()
    {
        if (!requiresFlashlight) return true;

        return player.equippedItem is Flashlight flashlight && flashlight.isFlashlightOn;
    }

    private void TeleportPlayer()
    {
        if (targetExitPoint != null)
        {
            UIManager.Instance.ShowInteractUI(false, "");
            player.transform.position = targetExitPoint.position;
            player.currentState = PlayerController.PlayerState.Normal;
            Debug.Log($"[VentExit] ��Ҵ��͵��˳���λ��: {targetExitPoint.name}");
        }
        else
        {
            Debug.LogError("[VentExit] δ����Ŀ�����λ�ã�");
        }
    }

    private void UpdateInteractUIPosition()
    {
        Vector3 worldPosition = transform.position + new Vector3(0, -0.5f, 0);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        UIManager.Instance.interactUI.GetComponent<RectTransform>().position = screenPosition;
    }
}

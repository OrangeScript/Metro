using UnityEngine;

public class VentExit : MonoBehaviour
{
    [Header("���ڶ�Ӧ���ⲿλ��")]
    [SerializeField] private Transform targetExitPoint;

    [Header("�Ƿ���Ҫ�ֵ�Ͳ�����뿪")]
    [SerializeField] private bool requiresFlashlight = false;

    private PlayerController player;
    private bool playerInTrigger = false;
    private UIManager.InteractRequest ventRequest;
    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        ventRequest = new UIManager.InteractRequest
        {
            text = "[R]�뿪ͨ���",
            worldPosition = transform.position + Vector3.down * 0.5f,
            priority = 2,
            source = this
        };
    }

    private void Update()
    {
        if (playerInTrigger && player != null)
        {
            if (CanTeleport())
            {
                UIManager.Instance.RegisterInteract(ventRequest);
                if (Input.GetKeyDown(KeyCode.R))
                {
                    TeleportPlayer();
                }
            }
            else
            {
                UIManager.Instance.UnregisterInteract(ventRequest);
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
            UIManager.Instance.UnregisterInteract(ventRequest);
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
            player.transform.position = targetExitPoint.position;
            player.currentState = PlayerController.PlayerState.Normal;
            Debug.Log($"[VentExit] ��Ҵ��͵��˳���λ��: {targetExitPoint.name}");
        }
        else
        {
            Debug.LogError("[VentExit] δ����Ŀ�����λ�ã�");
        }
    }
}

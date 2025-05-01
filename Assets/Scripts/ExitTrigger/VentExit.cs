using UnityEngine;

public class VentExit : MonoBehaviour
{
    [Header("出口对应的外部位置")]
    [SerializeField] private Transform targetExitPoint;

    [Header("是否需要手电筒才能离开")]
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
                UIManager.Instance.ShowInteractUI(true, "[R]离开通风口");
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
            Debug.Log("玩家靠近通风口出口");
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
            Debug.Log($"[VentExit] 玩家传送到了出口位置: {targetExitPoint.name}");
        }
        else
        {
            Debug.LogError("[VentExit] 未设置目标出口位置！");
        }
    }

    private void UpdateInteractUIPosition()
    {
        Vector3 worldPosition = transform.position + new Vector3(0, -0.5f, 0);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        UIManager.Instance.interactUI.GetComponent<RectTransform>().position = screenPosition;
    }
}

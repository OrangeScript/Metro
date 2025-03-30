using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Normal, Climbing, Carrying, Illusion }

    [Header("角色状态")]
    public PlayerState currentState = PlayerState.Normal;
    public bool isCrawling = false;

    [Header("移动设置")]
    [SerializeField] public float walkSpeed = 5f;
    [SerializeField] public float crawlSpeed = 2f;
    private Vector2 movement;
    private Rigidbody2D rb;

    [Header("组件引用")]
    [SerializeField] private Transform equipPoint;
    [SerializeField] private Transform maskEquipPoint;
    private Animator anim;
    public InventorySystem inventory;
    public InteractableObject equippedItem = null;
    public InteractableObject equippedMask = null;

    [Header("交互设置")]
    [SerializeField] private float interactRadius = 1f;
    [SerializeField] private LayerMask interactableLayer;
    public InteractableObject nearestInteractable;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        inventory = GetComponent<InventorySystem>();
        inventory.SetPlayer(this);
    }

    void Update()
    {
        HandleInput();
        HandleAnimation();
        CheckInteractables();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleInput()
    {
        if (currentState != PlayerState.Illusion)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }
        else
        {
            movement.x = Input.GetAxisRaw("Horizontal");
        }

        if (Input.GetKeyDown(KeyCode.F)) HandleInteraction();
    }

    public void SwitchEquippedItem(int index)
    {
        if (index >= 0 && index < inventory.equippedItems.Count)
        {
            EquipItem(inventory.equippedItems[index]); 
        }
    }

    private void HandleAnimation()
    {
        if (anim != null)
        {
            anim.SetFloat("Speed", movement.magnitude);
            anim.SetBool("IsCrawling", isCrawling);
        }
    }

    private void HandleMovement()
    {
        if (currentState != PlayerState.Illusion)
        {
            rb.velocity = new Vector2(movement.x * (isCrawling ? crawlSpeed : walkSpeed), rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(movement.x * (isCrawling ? crawlSpeed : walkSpeed), 0);
        }
    }

    private void CheckInteractables()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, interactRadius, interactableLayer);
        InteractableObject closestObj = null;
        float closestDistance = interactRadius;

        foreach (var col in hitColliders)
        {
            InteractableObject obj = col.GetComponent<InteractableObject>();
            if (obj != null)
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestObj = obj;
                }
            }
        }

        nearestInteractable = closestObj;
    }

    private void HandleInteraction()
    {
        if (nearestInteractable != null)
        {
            nearestInteractable.OnInteract();
        }
    }

    public void EquipItem(InteractableObject item)
    {
        if (item == null) return;

        if (item.category == InteractableObject.ItemCategory.Equipment)
        {
            if (equippedItem != null)
            {
                equippedItem.OnUnequip();
                equippedItem.gameObject.SetActive(false);
            }

            equippedItem = item;
            item.OnEquip(GetEquipPoint(item));
        }
        else if (item.category == InteractableObject.ItemCategory.Mask)
        {
            if (equippedMask != null)
            {
                equippedMask.OnUnequip();
                equippedMask.gameObject.SetActive(false);
            }

            equippedMask = item;
            item.OnEquip(GetEquipPoint(item));
        }
    }

    public Transform GetEquipPoint(InteractableObject item)
    {
        return item.category == InteractableObject.ItemCategory.Equipment ? equipPoint : maskEquipPoint;
    }


    public void EnterIllusionWorld()
    {
        currentState = PlayerState.Illusion;
        UIManager.Instance.ShowMessage("你进入了幻觉世界...");
        SceneManager.LoadScene("IllusionScene");
        //传送到幻觉场景
    }

    public void BlockMovement()
    {
        rb.velocity = Vector2.zero;
        UIManager.Instance.ShowMessage("烟雾太浓，你无法前进！");
    }

    public void RecoverFromIllusion()
    {
        if (currentState == PlayerState.Illusion)
        {
            currentState = PlayerState.Normal;
            UIManager.Instance.ShowMessage("你恢复了意识。");
            // 切换回正常世界的场景
            SceneManager.LoadScene("NormalScene"); 
        }
    }
}

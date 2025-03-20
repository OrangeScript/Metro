using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Normal, Climbing, Carrying, Illusion }

    [Header("角色状态")]
    public PlayerState currentState = PlayerState.Normal;
    public bool isCrawling = false;
    public bool HasGasMask = false; // 是否佩戴防毒面具

    [Header("移动设置")]
    [SerializeField] public float walkSpeed = 5f;
    [SerializeField] public float crawlSpeed = 2f;
    private Vector2 movement;
    private Rigidbody2D rb;

    [Header("组件引用")]
    [SerializeField] private Transform equipPoint;  // 物品挂点
    [SerializeField] private Transform maskEquipPoint;  // 防毒面具挂点
    private Animator anim;
    public InventorySystem inventory;
    public InteractableObject equippedItem = null;

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
            movement = Vector2.zero; // 在幻觉世界中无法自由移动
        }

        if (Input.GetKeyDown(KeyCode.F)) HandleInteraction();
        if (Input.GetKeyDown(KeyCode.Q)) inventory.SwitchItem(-1);
        if (Input.GetKeyDown(KeyCode.E)) inventory.SwitchItem(1);
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
            rb.velocity = movement.normalized * (isCrawling ? crawlSpeed : walkSpeed);
        }
        else
        {
            rb.velocity = Vector2.zero;
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
        if (nearestInteractable != null)
        {
            UIManager.Instance.ShowInteractionPrompt($"[F]拾取 {nearestInteractable.itemName}", nearestInteractable.transform.position);
        }
        else
        {
            UIManager.Instance.HideInteractionPrompt();
        }
    }

    private void HandleInteraction()
    {
        if (nearestInteractable != null)
        {
            nearestInteractable.OnInteract();
            UIManager.Instance.HideInteractionPrompt();
        }
        else if (equippedItem != null)
        {
            equippedItem.UseItem();
        }
    }

    public void HandleCharacterEnterSmoke(SmokeSystem.SmokeLevel level)
    {
        if (level == SmokeSystem.SmokeLevel.Level2 && !HasGasMask)
        {
            EnterIllusionWorld();
        }
        else if (level == SmokeSystem.SmokeLevel.Level3 && !HasGasMask)
        {
            BlockMovement();
        }
    }

    public void EnterIllusionWorld()
    {
        currentState = PlayerState.Illusion;
        //UIManager.Instance.ShowMessage("你进入了幻觉世界...");
        // 传送到幻觉场景
    }

    public void BlockMovement()
    {
        rb.velocity = Vector2.zero;
        //UIManager.Instance.ShowMessage("烟雾太浓，你无法前进！");
    }

    public void RecoverFromIllusion()
    {
        currentState = PlayerState.Normal;
        //UIManager.Instance.ShowMessage("你恢复了意识。");
    }

    public void RecoverFromEffects()
    {
        if (currentState == PlayerState.Illusion)
        {
            RecoverFromIllusion();
        }
    }

    public Transform GetEquipPoint(InteractableObject item)
    {
        return item.category == InteractableObject.ItemCategory.Equipment ? equipPoint : maskEquipPoint;
    }

    public void EquipItem(InteractableObject item)
    {
        if (item == null) return;

        // 处理装备类物品
        if (item.category == InteractableObject.ItemCategory.Equipment)
        {
            // 卸下当前装备
            if (equippedItem != null)
            {
                equippedItem.OnUnequip();
                equippedItem.gameObject.SetActive(false); // 放回背包
            }

            equippedItem = item;
            item.OnEquip(GetEquipPoint(item));
        }
        else // 处理其他类型物品（如面具）
        {
            equippedItem = item;
            item.OnEquip(GetEquipPoint(item));
        }
    }
}

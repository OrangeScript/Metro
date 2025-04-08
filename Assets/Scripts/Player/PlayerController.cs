using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static InteractableObject;
using static UnityEditor.Progress;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Idle, Normal, Climbing, Carrying, Illusion, Crawling }

    [Header("��������")]
    public PlayerState currentState = PlayerState.Normal;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField]private Vector2 movement;
    private Rigidbody2D rb;

    [Header("�������")]
    private Animator anim;
    public InventorySystem inventory;
    
    [Header("��������")]
    [SerializeField] private Transform maskEquipPoint;
    [SerializeField] private Transform npcEquipPoint;
    [SerializeField] private Transform itemEquipPoint;
    [SerializeField] private float npcSpeedPenalty = 0.7f;
    public InteractableObject equippedMask;
    public InteractableObject equippedItem;
    private InteractableObject currentCarriedObject;
    private CarryType currentCarryType = CarryType.None;
    [SerializeField] private float interactRadius = 3f;
    [SerializeField] private LayerMask interactableLayer;
    public InteractableObject nearestInteractable;

    [Header("������")]
    public MetroDoor nearestMetroDoor;

    [Header("�ܵ�����")]
    [SerializeField] private LayerMask tunnelLayer;
    [SerializeField] private float tunnelSpeedMultiplier = 0.4f;
    private bool isInTunnel = false;
    private PlayerState previousStateBeforeTunnel;

    [Header("��������")]
    private int[] protectedStates = { 2, 3, 4 }; // Climbing, Carrying, Illusion



    [Header("test")]
    [SerializeField] private bool no;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;//ʹ�˶�����������ƽ��
        inventory = GetComponent<InventorySystem>();
        inventory.SetPlayer(this);
    }

    void Update()
    {
        if (MetroDoor.isDancing)
        {
            rb.velocity = Vector2.zero;  
            return; 
        }
        HandleInput();
        HandleImmediateAnimation();
        CheckInteractables();
        CheckNearestMetroDoor();
       
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    #region �������
    private void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 input=(horizontal*transform.right+vertical*transform.up).normalized;
        horizontal = input.x;
        vertical = input.y;
        bool hasMovementInput = horizontal != 0 || vertical != 0;

        HandleTunnelState(horizontal, vertical);
        if (isInTunnel) return;

        HandleManualStateSwitch();
        HandleAutoStateTransition(hasMovementInput);
        HandleMovementDirection(horizontal, vertical);
    }

    private void HandleManualStateSwitch()
    {
        if (Input.GetKeyDown(KeyCode.C) && !IsProtectedState())
        {
            currentState = currentState == PlayerState.Crawling ?
                PlayerState.Normal : PlayerState.Crawling;
        }
    }

    private void HandleAutoStateTransition(bool hasInput)
    {
        if (IsProtectedState() || currentState == PlayerState.Crawling) return;

        if (hasInput)
        {
            if (currentState == PlayerState.Idle)
                TransitionState(PlayerState.Normal);
        }
        else
        {
            if (currentState == PlayerState.Normal)
                TransitionState(PlayerState.Idle);
        }
    }

    private void HandleMovementDirection(float h, float v)
    {
        switch (currentState)
        {
            case PlayerState.Climbing:
                movement = new Vector2(0, v);
                break;
            case PlayerState.Illusion:
                movement = new Vector2(h, 0);
                break;
            default:
                movement = new Vector2(h, v);
                break;
        }
    }
    #endregion

    #region �ܵ�ϵͳ

    private void HandleTunnelState(float h, float v)
    {
        if (isInTunnel)
        {
            movement = new Vector2(h, v) * tunnelSpeedMultiplier;
            currentState = PlayerState.Crawling;
        }
    }

    public void EnterTunnel(Ventilation vent)
    {
        previousStateBeforeTunnel = currentState;
        isInTunnel = true;
        tunnelSpeedMultiplier = vent.crawlSpeedMultiplier; 
    }

    public void ExitTunnel()
    {
        isInTunnel = false;
        TransitionState(previousStateBeforeTunnel);
    }

    #endregion

    #region ����ϵͳ
    private void HandleImmediateAnimation()
    {
        if (!anim) return;

        anim.SetInteger("PlayerState", (int)currentState);
        anim.SetFloat("Speed", CalculateAnimationSpeed());
        anim.SetBool("InTunnel", isInTunnel);
        //test:anim.SetBool("IsCrawling", currentState == PlayerState.Crawling);

        HandleSpecialAnimations();
    }

    private float CalculateAnimationSpeed()
    {
        return currentState switch
        {
            PlayerState.Climbing => Mathf.Abs(movement.y),
            PlayerState.Illusion => Mathf.Abs(movement.x),
            _ => movement.magnitude
        };
    }

    private void HandleSpecialAnimations()
    {
        if (currentState == PlayerState.Carrying)
            anim.SetFloat("CarrySpeed", movement.magnitude);

        //if (isInTunnel)
            //anim.SetBool("IsCrawling", currentState == PlayerState.Crawling);
    }

    #endregion

    #region �ƶ�ϵͳ
    private void HandleMovement()
    {
        rb.velocity = movement * GetCurrentSpeed();
    }

    private float GetCurrentSpeed()
    {
        float baseSpeed = walkSpeed;

        if (currentState == PlayerState.Crawling||(currentState==PlayerState.Carrying&&currentCarryType==InteractableObject.CarryType.NPC))
            baseSpeed *= 0.6f;
        else if (currentState == PlayerState.Illusion)
            baseSpeed *= 1.2f;

        return baseSpeed;
    }
    
#endregion

    #region ״̬ϵͳ
    private void TransitionState(PlayerState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }

    private bool IsProtectedState()
    {
        return System.Array.Exists(protectedStates, s => s == (int)currentState);
    }
    #endregion

    #region ����ϵͳ
    public void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.F) && nearestInteractable)       
            nearestInteractable.OnInteract(); 
    }

    public void HandleUseItemInput()
    {
        if (currentCarriedObject == null) return;

        switch (currentCarriedObject.useTrigger)
        {
            case UseTrigger.KeyF:
                if (Input.GetKeyDown(KeyCode.F))
                {
                    Debug.Log("��ʼʹ����Ʒ������ F ����");
                    currentCarriedObject.UseItem();
                }
                break;

            case UseTrigger.RightClick:
                if (Input.GetMouseButtonDown(1)) 
                {
                    Debug.Log("��ʼʹ����Ʒ������Ҽ���");
                    currentCarriedObject.UseItem();
                }
                break;

            case UseTrigger.OnEquip:
                if (currentCarriedObject!=null)
                {
                    currentCarriedObject.UseItem(); // һ��װ����������
                }
                break;

            default:
                Debug.LogError("δ֪��ʹ�ô���������");
                break;
        }
    }

    private void CheckInteractables()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius, interactableLayer);
        nearestInteractable = GetNearestInteractable(hits);
    }

    private InteractableObject GetNearestInteractable(Collider2D[] cols)
    {
        InteractableObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var col in cols)
        {
            var obj = col.GetComponent<InteractableObject>();
            if (!obj) continue;

            if (obj == currentCarriedObject) continue;

            float dist = Vector2.Distance(transform.position, col.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = obj;
            }
        }
        return nearest;
    }

    private void CheckNearestMetroDoor()
    {
        float radius = 10f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        MetroDoor closestDoor = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("MetroDoor"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestDoor = hit.GetComponent<MetroDoor>();
                }
            }
        }

        nearestMetroDoor = closestDoor;
        if (nearestMetroDoor == null)
        {
            Debug.Log("��Χ��û�е�����");
        }
        else {
            if(currentCarriedObject != null &&
        (currentCarriedObject.CompareTag("Battery") ||
         currentCarriedObject.CompareTag("Crowbar")))
            nearestMetroDoor.TryInteract(this); }
    }
    #endregion

    #region ��������

    public void EquipItem(InteractableObject item)
    {        
        currentCarriedObject = item;
        currentCarryType = item.carryType;

        Transform targetPoint = GetAttachPoint(item);
        if (targetPoint == null)
        {
            Debug.LogError("tatgetPoint is null");
        }
        item.OnEquip(targetPoint);

        InventorySystem.Instance.equippedItem = item;
    }

    private Transform GetAttachPoint(InteractableObject item)
    {   
        return item.carryType switch
        {
            CarryType.Mask => maskEquipPoint,
            CarryType.NPC => npcEquipPoint,
            CarryType.Item => itemEquipPoint,
            _ => transform 
        };
    }

    public void UnequipItem(CarryType type)
    {
        if (currentCarryType != type) return;

        if (type == CarryType.NPC)
            walkSpeed /= npcSpeedPenalty;

        currentCarriedObject.OnUnequip();
        currentCarryType = CarryType.None;
        currentCarriedObject = null;
        //anim.SetBool("IsCarrying", false);

        TransitionState(PlayerState.Normal);
        InventorySystem.Instance.UnequipItem(currentCarriedObject);
        currentCarriedObject = null;
    }

    #endregion

    #region ������
    public void EnterIllusionWorld()
    {
        if (currentState != PlayerState.Illusion)
        {
            TransitionState(PlayerState.Illusion);
        }
        UIManager.Instance.ShowMessage("������˻þ�����...");
        SceneManager.LoadScene("IllusionScene");
    }

    public void BlockMovement()
    {
        rb.velocity = Vector2.zero;
        UIManager.Instance.ShowMessage("����̫Ũ�����޷�ǰ����");
    }

    public void RecoverFromIllusion()
    {
        if (currentState == PlayerState.Illusion)
        {
            TransitionState(PlayerState.Normal);
            UIManager.Instance.ShowMessage("��ָ�����ʶ��");
            SceneManager.LoadScene("NormalScene"); 
        }
    }
    #endregion
}

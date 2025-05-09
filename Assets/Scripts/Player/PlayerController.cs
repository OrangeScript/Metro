using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static InteractableObject;
using static UnityEditor.Progress;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Idle, Normal, Climbing, Carrying, Illusion, Crawling }

    public class InteractionPriorities
    {
        public const int NPC = 0;
        public const int Item = 1;
    }
    [Header("��������")]
    public PlayerState currentState = PlayerState.Normal;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField]private Vector2 movement;
    private Rigidbody2D rb;
    private CapsuleCollider2D col;

    [Header("�������")]
    private Animator anim;
    public InventorySystem inventory;
    
    [Header("��������")]
    [SerializeField] private Transform maskEquipPoint;
    [SerializeField] private Transform itemEquipPoint;
    public InteractableObject equippedMask;
    public InteractableObject equippedItem;
    private InteractableObject currentCarriedObject;
    private CarryType currentCarryType = CarryType.None;
    [SerializeField] private float interactRadius = 2f;
    [SerializeField] private LayerMask interactableLayer;
    public InteractableObject nearestInteractable;

    [Header("������")]
    public MetroDoor nearestMetroDoor;
    public bool awaitingSecondFPress = false;
    public MetroDoor poweredDoor = null;


    [Header("��������")]
    private int[] protectedStates = { 2, 3, 4 }; // Climbing, Carrying, Illusion

    [Header("NPC����")]
    public NPC currentNPC;
    private NPC carriedNPC;
    //�־û������������洢����ǰ����
    public class PersistentDataContainer : MonoBehaviour
    {
        public static Vector3 enterPosition;
    }

    [Header("��������")]
    private AirWallController airWall;

    private Dictionary<System.Type, UIManager.InteractRequest> activeRequests = new();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;//ʹ�˶�����������ƽ��
        inventory = GetComponent<InventorySystem>();
        inventory.SetPlayer(this);

        if (GameObject.Find("DataKeeper") == null)
        {
            GameObject dataObj = new GameObject("DataKeeper");
            dataObj.AddComponent<PersistentDataContainer>();
            DontDestroyOnLoad(dataObj);
        }
        //Debug.Log("Current Render Pipeline Asset: " + GraphicsSettings.renderPipelineAsset);
        //DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!GameManager.Instance.isGameStarted)
            return;
        if (ArrowManager.S.IsInWave())
        {
            rb.velocity = Vector2.zero;  
            return; 
        }
        TryInteractWithNPC();
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (currentState == PlayerState.Carrying) {
                TryDropNPC();
            }
            else
            {
                TryCarryNPC();
            }
        }
        HandleInput();
        HandleImmediateAnimation();
        CheckInteractables();
        CheckNearestMetroDoor();
        CheckPoweredDoor();
        HandleNPCPutOutFire();
        ManageInteractionRequests();
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.isGameStarted)
            return;
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

        //HandleManualStateSwitch();
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
            default:
                movement = new Vector2(h, v);
                break;
        }
    }
    #endregion


    #region ����ϵͳ

    private void HandleImmediateAnimation()
    {
        if (!anim) return;

        anim.SetInteger("PlayerState", (int)currentState);
        anim.SetFloat("inputX", movement.x);
        anim.SetFloat("inputY", movement.y);
        anim.SetFloat("Speed", CalculateAnimationSpeed());
        anim.SetBool("IsCrawling", currentState == PlayerState.Crawling);

        HandleSpecialAnimations();
    }

    private float CalculateAnimationSpeed()
    {
        return currentState switch
        {
            PlayerState.Climbing => Mathf.Abs(movement.y),
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
            baseSpeed *= 0.3f;

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
        if (equippedMask != null)
        {
            switch (equippedMask.useTrigger)
            {
                case UseTrigger.KeyF:
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        Debug.Log("��ʼʹ����ߣ����� F ����");
                        equippedMask.UseItem();
                    }
                    break;

                case UseTrigger.RightClick:
                    if (Input.GetMouseButtonDown(1))
                    {
                        Debug.Log("��ʼʹ����ߣ�����Ҽ���");
                        equippedMask.UseItem();
                    }
                    break;

                case UseTrigger.OnEquip:
                    equippedMask.UseItem(); 
                    break;

                default:
                    Debug.LogError("δ֪��ʹ�ô���������");
                    break;
            }
        }

        //������װ������ͨ��Ʒ
        if (equippedItem != null)
        {
            switch (equippedItem.useTrigger)
            {
                case UseTrigger.KeyF:
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        Debug.Log("��ʼʹ����Ʒ������ F ����");
                        equippedItem.UseItem();
                    }
                    break;

                case UseTrigger.RightClick:
                    if (Input.GetMouseButtonDown(1))
                    {
                        Debug.Log("��ʼʹ����Ʒ������Ҽ���");
                        equippedItem.UseItem();
                    }
                    break;

                case UseTrigger.OnEquip:
                    equippedItem.UseItem(); 
                    break;

                default:
                    Debug.LogError("δ֪��ʹ�ô���������");
                    break;
            }
        }
    }

    private void CheckInteractables()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius, interactableLayer);
        nearestInteractable = GetNearestInteractable(hits);

        if (nearestInteractable != null && !(nearestInteractable is CombustibleItem ci && ci.isBurning))
        {
            var request = new UIManager.InteractRequest
            {
                text = "[F]ʰȡ",
                worldPosition = nearestInteractable.transform.position + Vector3.down * 0.5f,
                priority = InteractionPriorities.Item,
                source = nearestInteractable
            };
            RegisterRequest(typeof(InteractableObject), request);
        }
        else
        {
            UnregisterRequest(typeof(InteractableObject));
        }
    }

    private InteractableObject GetNearestInteractable(Collider2D[] cols)
    {
        InteractableObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var col in cols)
        {
            var obj = col.GetComponent<InteractableObject>();
            if (!obj) continue;

            if (obj == equippedMask || obj == equippedItem) continue;

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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius);

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
        if (Input.GetKeyDown(KeyCode.F)&& nearestMetroDoor != null){
            var door = nearestMetroDoor;
            door.TryInteract(this);
        }
    }

    private void CheckPoweredDoor()
    {
        if (Input.GetKeyDown(KeyCode.F) && poweredDoor != null)
        {
            if (awaitingSecondFPress)
            {
                var door = poweredDoor;
                if (door.currentFault == MetroDoor.FaultType.Type3)
                {
                    door.currentFault = MetroDoor.FaultType.Type1;
                }
                else if (door.currentFault == MetroDoor.FaultType.Type4)
                {
                    door.currentFault = MetroDoor.FaultType.Type2;
                }
                else if (door.currentFault == MetroDoor.FaultType.Type5)
                {
                    StartCoroutine(door.HandleMazePuzzleWithNoChange(door));
                }

                door.TryInteract(this);
                awaitingSecondFPress = false;
                poweredDoor = null;
            }
        }
    }

    private void ManageInteractionRequests()
    {
        // �Զ�������Ч����
        var keysToRemove = activeRequests.Where(p => p.Value.source == null).Select(p => p.Key).ToList();
        foreach (var key in keysToRemove)
        {
            UIManager.Instance.UnregisterInteract(activeRequests[key]);
            activeRequests.Remove(key);
        }
    }

    private void RegisterRequest(System.Type type, UIManager.InteractRequest request)
    {
        if (activeRequests.TryGetValue(type, out var existing))
        {
            UIManager.Instance.UnregisterInteract(existing);
        }
        UIManager.Instance.RegisterInteract(request);
        activeRequests[type] = request;
    }

    private void UnregisterRequest(System.Type type)
    {
        if (activeRequests.TryGetValue(type, out var request))
        {
            UIManager.Instance.UnregisterInteract(request);
            activeRequests.Remove(type);
        }
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
        if (currentCarryType == InteractableObject.CarryType.Mask)
        {
            equippedMask = item;
        }
        else if(currentCarryType == InteractableObject.CarryType.Item)
        {
            equippedItem = item; }
    }

    private Transform GetAttachPoint(InteractableObject item)
    {   
        return item.carryType switch
        {
            CarryType.Mask => maskEquipPoint,
            CarryType.Item => itemEquipPoint,
            _ => transform 
        };
    }

    public void UnequipItem(CarryType type)
    {
        switch (type)
        {
            case CarryType.Mask:
                if (equippedMask != null)
                {
                    InventorySystem.Instance.UnequipItem(equippedMask);
                }
                break;

            case CarryType.Item:
                if (equippedItem != null)
                {
                    InventorySystem.Instance.UnequipItem(equippedItem);
                }
                break;
        }

        // ���µ�ǰЯ��״̬
        currentCarriedObject = equippedMask ?? equippedItem;
        currentCarryType = currentCarriedObject?.carryType ?? CarryType.None;
    }


    #endregion

    #region ������
    public void BlockMovement()
    {
        airWall = FindObjectOfType<AirWallController>();
        airWall.SetMaskState(equippedMask);
    }

    #endregion

    #region NPC����
    public void TryInteractWithNPC()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRadius);
        foreach (var collider in colliders)
        {
            NPC npc = collider.GetComponent<NPC>();
            if (npc != null)
            {
                currentNPC = npc;
                if (Input.GetKeyDown(KeyCode.T))currentNPC.Interact(this);  
                break;
            }
        }

        if (currentNPC != null)
        {
            var request = new UIManager.InteractRequest
            {
                text = "[T]��̸",
                worldPosition = currentNPC.transform.position + Vector3.down * 0.5f,
                priority = InteractionPriorities.NPC,
                source = currentNPC
            };
            RegisterRequest(typeof(NPC), request);
        }
        else
        {
            UnregisterRequest(typeof(NPC));
        }
    }

    public void TryCarryNPC()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRadius);
        NPC closestNPC = null;
        float closestDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            NPC npc = collider.GetComponent<NPC>();
            if (npc != null && npc.CompareTag("UnconsciousNPC") && npc.gameObject.activeSelf)
            {
                float distance = Vector2.Distance(transform.position, npc.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNPC = npc;
                }
            }
        }
        if (closestNPC != null)
        {
            carriedNPC = closestNPC;
            carriedNPC.gameObject.SetActive(false);
            TransitionState(PlayerState.Carrying);
        }
    }

    public void TryDropNPC()
    {
        if (carriedNPC != null)
        {
            carriedNPC.gameObject.SetActive(true);
            carriedNPC.stateMachine.Initialize(carriedNPC.unconsciousState);
            carriedNPC.transform.position = transform.position + new Vector3(5f, 0, 0); 
            carriedNPC = null;
            TransitionState(PlayerState.Normal);
        }
    }

    private void HandleNPCPutOutFire()
    {
        if (currentNPC != null )
        {
            CombustibleItem burningItem = currentNPC.GetComponentInChildren<CombustibleItem>();

            if (burningItem != null && burningItem.isBurning)
            {
                StartCoroutine(ExtinguishAfterDelay(burningItem));
            }
        }
    }

    private IEnumerator ExtinguishAfterDelay(CombustibleItem item)
    {
        yield return new WaitForSeconds(3f);
        item.Extinguish();
    }
    #endregion
}

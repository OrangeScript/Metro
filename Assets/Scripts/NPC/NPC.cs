using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Entity
{
    public Animator anim;
    public Rigidbody2D rb;
    public enum NPCLevel { Tier1, Tier2 }

    [Header("NPC����")]
    public NPCLevel npcLevel = NPCLevel.Tier1;
    //public NPCState currentState = NPCState.Normal;

    [Header("��Ʒ����")]
    public GameObject collectibleItemPrefab;  // ��ʰȡ����Ԥ����
    public Transform itemSpawnPoint;          // ��������λ��

    [Header("�Ի�����")]
    [TextArea(3, 5)] public string[] normalDialogue;
    [TextArea(3, 5)] public string[] hallucinatingDialogue;
    [TextArea(3, 5)] public string[] soberDialogue;

    [Header("״̬����ʱ��")]
    public float hallucinationDuration = 10f;
    public float unconsciousDuration = 5f;

    [Header("Я��״̬")]
    public Sprite carriedSprite;
    private Sprite normalSprite;
    private SpriteRenderer spriteRenderer;

    private bool hasGivenItem = false;
    private Coroutine stateRoutine;

    private NPCStateMachine stateMachine;
    #region State
    public NPCHallucinatingState hallucinatingState;
    public NPCNormalState normalState;
    public NPCUnconsciousState unconsciousState;

    #endregion

    private void Awake()
    {
        anim = GetComponent<Animator>();
        GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalSprite = spriteRenderer.sprite;


        stateMachine = new NPCStateMachine();


        hallucinatingState = new NPCHallucinatingState(this, stateMachine, "Hallucinating");
        normalState = new NPCNormalState(this, stateMachine, "Normal");
        unconsciousState = new NPCUnconsciousState(this, stateMachine, "Unconscious");
       
    }
    private void Start()
    {
        stateMachine.Initialize(normalState);
    }
    private void Update()
    {

        stateMachine.currentState.Update();


    }
    
    private void UpdateVisuals()
    {
        // �������Ч�������ʱ仯�������Ӿ�Ч��
    }

    private void ResetNPC()
    {
        hasGivenItem = false;
        // ��������״̬��ز���
    }

    // ��ҽ������
    public virtual void Interact(PlayerController player)
    {
        StartCoroutine(DialogueInteraction(player));
    }

    private IEnumerator DialogueInteraction(PlayerController player)
    {
        // ��ʾ�Ի�UI
        string[] dialogue = GetCurrentDialogue();
        UIManager.Instance.ShowDialogue(dialogue);

        // �ȴ��Ի����
        while (UIManager.Instance.IsDialogueActive)
        {
            yield return null;
        }

        // һ��NPC������״̬�������
        if (npcLevel == NPCLevel.Tier1 &&
            stateMachine.currentState!=unconsciousState&&
            !hasGivenItem)
        {
            GiveCollectibleItem(player);
            hasGivenItem = true;
        }
    }
    private string[] GetCurrentDialogue()
    {

        
        if (stateMachine.currentState == hallucinatingState)
            return hallucinatingDialogue;
        if ((stateMachine.currentState == normalState) && hasGivenItem)
            return soberDialogue;
        if (stateMachine.currentState == normalState)
            return normalDialogue;

        return new string[] { "..." };
    }

    private void GiveCollectibleItem(PlayerController player)
    {
        if (collectibleItemPrefab == null) return;

        GameObject item = Instantiate(collectibleItemPrefab,
            itemSpawnPoint.position,
            Quaternion.identity);

        // player.inventory.AddItem(item.GetComponent<InteractableObject>());
    }

    // ����״̬��������
    public void SetUnconsciousPhysics(bool isUnconscious)
    {
        Collider2D col = GetComponent<Collider2D>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        col.enabled = !isUnconscious;
        rb.simulated = !isUnconscious;
        rb.velocity = Vector2.zero;
    }

    public void SetCarriedState(bool isCarried)
    {
        spriteRenderer.sprite = isCarried ? carriedSprite : normalSprite;
    }

    public void RecoverFromEffects()
    {
        // ����NPC�����������Ӱ���лָ����߼�
        Debug.Log("NPC�ָ�������״̬��");
    }

    public void SetPhysicsActive(bool isPhysicsActive) { }
}

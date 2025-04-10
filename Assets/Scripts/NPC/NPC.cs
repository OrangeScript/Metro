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
}

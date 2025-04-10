using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Entity
{
    public Animator anim;
    public Rigidbody2D rb;
    public enum NPCLevel { Tier1, Tier2 }

    [Header("NPC设置")]
    public NPCLevel npcLevel = NPCLevel.Tier1;
    //public NPCState currentState = NPCState.Normal;

    [Header("物品设置")]
    public GameObject collectibleItemPrefab;  // 可拾取道具预制体
    public Transform itemSpawnPoint;          // 道具生成位置

    [Header("对话设置")]
    [TextArea(3, 5)] public string[] normalDialogue;
    [TextArea(3, 5)] public string[] hallucinatingDialogue;
    [TextArea(3, 5)] public string[] soberDialogue;

    [Header("状态持续时间")]
    public float hallucinationDuration = 10f;
    public float unconsciousDuration = 5f;

    [Header("携带状态")]
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

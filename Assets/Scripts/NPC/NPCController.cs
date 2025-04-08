using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public enum NPCState { 
        Normal,//清醒
        Hallucinating, //幻觉
        Unconscious //昏迷
    }
    public enum NPCLevel { Tier1, Tier2 }

    [Header("NPC设置")]
    public NPCLevel npcLevel = NPCLevel.Tier1;
    public NPCState currentState = NPCState.Normal;

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

    private Animator anim;
    private bool hasGivenItem = false;
    private Coroutine stateRoutine;

    void Awake()
    {
        anim = GetComponent<Animator>();
        GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalSprite = spriteRenderer.sprite;
    }

    public void ChangeState(NPCState State)
    {
        if (currentState == State) return;

        currentState = State;
        if (stateRoutine != null) StopCoroutine(stateRoutine);

        switch (State)
        {
            case NPCState.Hallucinating:
                stateRoutine = StartCoroutine(HallucinationRoutine());
                break;
            case NPCState.Unconscious:
                stateRoutine = StartCoroutine(UnconsciousRoutine());
                break;
            case NPCState.Normal:
                ResetNPC();
                break;
        }
        UpdateVisuals();
    }

    private IEnumerator HallucinationRoutine()
    {
        // 幻觉阶段行为
        yield return new WaitForSeconds(hallucinationDuration);
        ChangeState(NPCState.Unconscious);
    }

    private IEnumerator UnconsciousRoutine()
    {
        // 昏迷阶段行为
        yield return new WaitForSeconds(unconsciousDuration);
        ChangeState(NPCState.Normal);
    }

    private void UpdateVisuals()
    {
        anim.SetInteger("State", (int)currentState);
        // 添加粒子效果、材质变化等其他视觉效果
    }

    private void ResetNPC()
    {
        hasGivenItem = false;
        // 重置其他状态相关参数
    }

    // 玩家交互入口
    public void Interact(PlayerController player)
    {
        if (currentState == NPCState.Unconscious) return;

        StartCoroutine(DialogueInteraction(player));
    }

    private IEnumerator DialogueInteraction(PlayerController player)
    {
        // 显示对话UI
        string[] dialogue = GetCurrentDialogue();
        //UIManager.Instance.ShowDialogue(dialogue);

        // 等待对话完成
        while (UIManager.Instance.IsDialogueActive)
        {
            yield return null;
        }

        // 一级NPC在清醒状态给予道具
        if (npcLevel == NPCLevel.Tier1 &&
            currentState == NPCState.Normal &&
            !hasGivenItem)
        {
            GiveCollectibleItem(player);
            hasGivenItem = true;
        }
    }

    private string[] GetCurrentDialogue()
    {
        return currentState switch
        {
            NPCState.Hallucinating => hallucinatingDialogue,
            NPCState.Normal when hasGivenItem => soberDialogue,
            NPCState.Normal => normalDialogue,
            _ => new string[] { "..." }
        };
    }

    private void GiveCollectibleItem(PlayerController player)
    {
        if (collectibleItemPrefab == null) return;

        GameObject item = Instantiate(collectibleItemPrefab,
            itemSpawnPoint.position,
            Quaternion.identity);

       // player.inventory.AddItem(item.GetComponent<InteractableObject>());
    }

    // 昏迷状态物理设置
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
        spriteRenderer.sprite = isCarried ? carriedSprite : normalSprite;    }

    public void RecoverFromEffects()
    {
        // 处理NPC从烟雾或其他影响中恢复的逻辑
        Debug.Log("NPC恢复了正常状态。");
    }

    public void SetPhysicsActive(bool isPhysicsActive) { }
}
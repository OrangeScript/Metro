using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public enum NPCState { 
        Normal,//����
        Hallucinating, //�þ�
        Unconscious //����
    }
    public enum NPCLevel { Tier1, Tier2 }

    [Header("NPC����")]
    public NPCLevel npcLevel = NPCLevel.Tier1;
    public NPCState currentState = NPCState.Normal;

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
        // �þ��׶���Ϊ
        yield return new WaitForSeconds(hallucinationDuration);
        ChangeState(NPCState.Unconscious);
    }

    private IEnumerator UnconsciousRoutine()
    {
        // ���Խ׶���Ϊ
        yield return new WaitForSeconds(unconsciousDuration);
        ChangeState(NPCState.Normal);
    }

    private void UpdateVisuals()
    {
        anim.SetInteger("State", (int)currentState);
        // �������Ч�������ʱ仯�������Ӿ�Ч��
    }

    private void ResetNPC()
    {
        hasGivenItem = false;
        // ��������״̬��ز���
    }

    // ��ҽ������
    public void Interact(PlayerController player)
    {
        if (currentState == NPCState.Unconscious) return;

        StartCoroutine(DialogueInteraction(player));
    }

    private IEnumerator DialogueInteraction(PlayerController player)
    {
        // ��ʾ�Ի�UI
        string[] dialogue = GetCurrentDialogue();
        //UIManager.Instance.ShowDialogue(dialogue);

        // �ȴ��Ի����
        while (UIManager.Instance.IsDialogueActive)
        {
            yield return null;
        }

        // һ��NPC������״̬�������
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
        spriteRenderer.sprite = isCarried ? carriedSprite : normalSprite;    }

    public void RecoverFromEffects()
    {
        // ����NPC�����������Ӱ���лָ����߼�
        Debug.Log("NPC�ָ�������״̬��");
    }

    public void SetPhysicsActive(bool isPhysicsActive) { }
}
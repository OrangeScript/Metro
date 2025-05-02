using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Entity
{
    public Animator anim;
    public Rigidbody2D rb;
    public PlayerController player;
    public enum NPCLevel { Tier1, Tier2 }

    [Header("NPC����")]
    public NPCLevel npcLevel = NPCLevel.Tier1;
    //public NPCState currentState = NPCState.Normal;


    [Header("�Ի�����")]
    public int npcID;


    [Header("״̬����ʱ��")]
    public float hallucinationDuration = 10f;
    public float unconsciousDuration = 5f;

    private Coroutine stateRoutine;

    public NPCStateMachine stateMachine;
    public enum InitialNPCState
    {
        Normal,
        Unconscious,
        Hallucinating
    }

    public InitialNPCState initialState = InitialNPCState.Normal;

    #region State
    public NPCHallucinatingState hallucinatingState;
    public NPCNormalState normalState;
    public NPCUnconsciousState unconsciousState;

    #endregion

    public void Awake()
    {
        
        stateMachine = new NPCStateMachine();
        hallucinatingState = new NPCHallucinatingState(this, stateMachine, "Hallucinating");
        normalState = new NPCNormalState(this, stateMachine, "Normal");
        unconsciousState = new NPCUnconsciousState(this, stateMachine, "Unconscious");

        switch (initialState)
        {
            case InitialNPCState.Normal:
                stateMachine.Initialize(normalState);
                break;
            case InitialNPCState.Unconscious:
                stateMachine.Initialize(unconsciousState);
                break;
            case InitialNPCState.Hallucinating:
                stateMachine.Initialize(hallucinatingState);
                break;
            default:
                Debug.LogWarning("δ֪��ʼ״̬");
                break;
        }
    }


    private void Update()
    {
        stateMachine.currentState.Update();
    }
    

    // ��ҽ������
    public virtual void Interact(PlayerController player)
    {
        DialogManager.Instance.StartDialogue(npcID);
    }

    public void RecoverFromEffects()
    {
        // ����NPC�����������Ӱ���лָ����߼�
        Debug.Log("NPC�ָ�������״̬��");
    }
}

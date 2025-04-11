using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NPC;

public class NPCState
{
    protected NPCStateMachine stateMachine;
    protected NPC npcBase;
    protected Rigidbody2D rb;
    protected bool triggerCalled;
    private string animBoolName;
    protected float stateTimer;
    public NPCState(NPC _npcBase, NPCStateMachine _stateMachine, string _animBoolName)
    {
        npcBase = _npcBase;
        stateMachine = _stateMachine;
        animBoolName = _animBoolName;
    }
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Enter()
    {
        triggerCalled = false;
        npcBase.anim.SetBool(animBoolName, true);
        rb = npcBase.rb;
    }
    public virtual void Exit()
    {
        npcBase.anim.SetBool(animBoolName, false);
        //npcBase.AssignLastAnimBoolName(animBoolName);
    }
    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
    

    
}

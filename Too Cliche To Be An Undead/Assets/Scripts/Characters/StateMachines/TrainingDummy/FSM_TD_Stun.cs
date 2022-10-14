using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_TD_Stun : FSM_Base<FSM_TD_Manager>
{
    private TrainingDummy owner;
    private float stun_TIMER;

    public override void EnterState(FSM_TD_Manager stateManager)
    {
        owner ??= stateManager.Owner;
    }

    public override void UpdateState(FSM_TD_Manager stateManager)
    {
        if (stun_TIMER > 0) stun_TIMER -= Time.deltaTime;
    }

    public override void FixedUpdateState(FSM_TD_Manager stateManager)
    {
    }

    public override void ExitState(FSM_TD_Manager stateManager)
    {
        owner.HideStatusText();
    }

    public override void Conditions(FSM_TD_Manager stateManager)
    {
        if (stun_TIMER <= 0) stateManager.SwitchState(stateManager.idleState);
    }
    public FSM_TD_Stun SetDuration(float duration)
    {
        stun_TIMER = duration;
        return this;
    }
}

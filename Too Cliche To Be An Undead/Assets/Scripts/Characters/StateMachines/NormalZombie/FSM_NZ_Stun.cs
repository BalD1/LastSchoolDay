using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_NZ_Stun : FSM_Base<FSM_NZ_Manager>
{
    private NormalZombie owner;
    private float stun_TIMER;

    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        owner ??= stateManager.Owner;
    }

    public override void UpdateState(FSM_NZ_Manager stateManager)
    {
        if (stun_TIMER > 0) stun_TIMER -= Time.deltaTime;
    }

    public override void FixedUpdateState(FSM_NZ_Manager stateManager)
    {
    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        if (stun_TIMER <= 0) stateManager.SwitchState(stateManager.wanderingState);
    }
    public FSM_NZ_Stun SetDuration(float duration)
    {
        stun_TIMER = duration;
        return this;
    }
}

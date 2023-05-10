using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Boss_Recovering : FSM_Base<FSM_Boss_Manager>
{
    private BossZombie owner;

    private float recovering_TIMER;

    public override void EnterState(FSM_Boss_Manager stateManager)
    {
        owner = stateManager.Owner;
        owner.animationController.SetAnimation(owner.animationData.IdleAnim, true);
    }

    public override void UpdateState(FSM_Boss_Manager stateManager)
    {
        recovering_TIMER -= Time.deltaTime;
    }

    public override void FixedUpdateState(FSM_Boss_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Boss_Manager stateManager)
    {
    }

    public FSM_Boss_Recovering SetTimer(float time)
    {
         recovering_TIMER = time * owner.recoverTimerModifier;
        return this;
    }

    public override void Conditions(FSM_Boss_Manager stateManager)
    {
        if (recovering_TIMER <= 0) stateManager.SwitchState(stateManager.chasingState);
    }

    public override string ToString() => "Recovering";
}

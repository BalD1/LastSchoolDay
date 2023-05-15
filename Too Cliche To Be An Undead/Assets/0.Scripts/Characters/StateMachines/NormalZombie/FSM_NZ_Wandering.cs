using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_NZ_Wandering : FSM_Base<FSM_NZ_Manager>
{
    private NormalZombie owner;
    private bool canSwitchToChase = true;

    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        owner ??= stateManager.Owner;
        owner.ResetVelocity();

        if (owner.allowWander) owner.ChooseRandomPosition();
        else owner.Pathfinding.StopUpdatepath();

        owner.canBePushed = true;
    }

    public override void UpdateState(FSM_NZ_Manager stateManager)
    {
        if (!owner.allowWander) return;
        if (owner.Pathfinding == null) return;
    }

    public override void FixedUpdateState(FSM_NZ_Manager stateManager)
    {
    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        if (owner.CurrentPlayerTarget != null) stateManager.SwitchState(stateManager.chasingState);
    }

    public override string ToString() => "Wandering";
}

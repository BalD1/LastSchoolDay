using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Boss_Chasing : FSM_Base<FSM_Boss_Manager>
{
    private BossZombie owner;
    private Vector2 goalPosition;

    public override void EnterState(FSM_Boss_Manager stateManager)
    {
        owner ??= stateManager.Owner;
        owner.ResetVelocity();

        owner.Pathfinding.StopUpdatepath();
        owner.Pathfinding.StartUpdatePath();

        owner.canBePushed = true;
    }

    public override void UpdateState(FSM_Boss_Manager stateManager)
    {
        if (owner.Pathfinding != null)
            goalPosition = owner.Pathfinding.CheckWayPoint();
    }

    public override void FixedUpdateState(FSM_Boss_Manager stateManager)
    {
        stateManager.Movements(goalPosition);
    }

    public override void ExitState(FSM_Boss_Manager stateManager)
    {
        owner.GetRb.velocity = Vector2.zero;
    }

    public override void Conditions(FSM_Boss_Manager stateManager)
    {
        if (owner.CurrentPlayerTarget == null) return;

        if (stateManager.AttackConditions())
            stateManager.SwitchState(stateManager.attackingState);
    }

    private bool WanderingConditions() => owner.CurrentPlayerTarget == null;
    public override string ToString() => "Chasing";
}

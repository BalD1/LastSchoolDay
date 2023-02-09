using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_NZ_Chasing : FSM_Base<FSM_NZ_Manager>
{
    private NormalZombie owner;
    private Vector2 goalPosition;

    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        owner ??= stateManager.Owner;
        owner.ResetVelocity();

        if (owner.animationController != null)
            owner.animationController.SetAnimation(owner.animationData.WalkAnim, true);

        owner.Pathfinding.StopUpdatepath();
        owner.Pathfinding.StartUpdatePath();

        owner.canBePushed = true;
    }

    public override void UpdateState(FSM_NZ_Manager stateManager)
    {
        if (owner.Pathfinding != null)
            goalPosition = owner.Pathfinding.CheckWayPoint();
    }

    public override void FixedUpdateState(FSM_NZ_Manager stateManager)
    {
        //owner.GetRb.AddForce(owner.Pathfinding.CheckWayPoint() * owner.GetStats.Speed * owner.SpeedMultiplier * Time.fixedDeltaTime);
        stateManager.Movements(goalPosition);

        if (owner.animationController == null) return;
        if (owner.CurrentTransformTarget == null) return;

        if (owner.transform.position.x > owner.CurrentTransformTarget.position.x) owner.animationController.FlipSkeleton(false);
        else owner.animationController.FlipSkeleton(true);
    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
        owner.GetRb.velocity = Vector2.zero;
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        if (WanderingConditions())
            stateManager.SwitchState(stateManager.wanderingState);

        if (owner.CurrentPlayerTarget == null) return;

        if (stateManager.AttackConditions())
            stateManager.SwitchState(stateManager.attackingState);
    }

    private bool WanderingConditions() => owner.CurrentPlayerTarget == null;
    public override string ToString() => "Chasing";
}

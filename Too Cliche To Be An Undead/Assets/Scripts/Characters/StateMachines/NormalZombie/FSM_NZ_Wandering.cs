using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_NZ_Wandering : FSM_Base<FSM_NZ_Manager>
{
    private NormalZombie owner;
    private bool canSwitchToChase = false;
    private Vector2 goalPosition;

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
        
        goalPosition = owner.Pathfinding.CheckWayPoint();
    }

    public override void FixedUpdateState(FSM_NZ_Manager stateManager)
    {
        stateManager.Movements(goalPosition);
    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
        canSwitchToChase = false;
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        if (canSwitchToChase) stateManager.SwitchState(stateManager.chasingState);

        if (Vector2.Distance(owner.transform.position, owner.CurrentPositionTarget) < owner.DistanceBeforeStop * 0.6f)
            stateManager.SwitchState(stateManager.idleState);
    }

    public void SawPlayer() => canSwitchToChase = true;
    public override string ToString() => "Wandering";
}

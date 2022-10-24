using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_NZ_Wandering : FSM_Base<FSM_NZ_Manager>
{
    private NormalZombie owner;
    private bool canSwitchToChase = false;
    private bool moveToBasePos = false;
    private Vector2 goalPosition;

    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        owner ??= stateManager.Owner;
        moveToBasePos = Vector2.Distance(owner.BasePosition, owner.transform.position) > (owner.DistanceBeforeStop / 2);
        if (moveToBasePos)
            owner.SetTarget(owner.BasePosition);
    }

    public override void UpdateState(FSM_NZ_Manager stateManager)
    {
        if (moveToBasePos)
            goalPosition = owner.Pathfinding.CheckWayPoint();

        if (moveToBasePos)
        {
            moveToBasePos = Vector2.Distance(owner.BasePosition, owner.transform.position) > (owner.DistanceBeforeStop / 2);
            if (!moveToBasePos) owner.GetRb.velocity = Vector2.zero;
        }
    }

    public override void FixedUpdateState(FSM_NZ_Manager stateManager)
    {
        if (moveToBasePos)
            stateManager.Movements(goalPosition);

    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
        owner.GetRb.velocity = Vector2.zero;
        canSwitchToChase = false;
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        if (canSwitchToChase) stateManager.SwitchState(stateManager.chasingState);
    }

    public void SawPlayer() => canSwitchToChase = true;
    public override string ToString() => "Wandering";
}

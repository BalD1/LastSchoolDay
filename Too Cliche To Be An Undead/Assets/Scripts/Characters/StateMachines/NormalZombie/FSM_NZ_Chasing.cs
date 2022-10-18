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
    }

    public override void UpdateState(FSM_NZ_Manager stateManager)
    {
        goalPosition = owner.CurrentTransformTarget != null ? owner.CurrentTransformTarget.position : owner.lastSeenPosition;
        owner.transform.position = Vector2.MoveTowards(owner.transform.position, goalPosition, owner.GetStats.Speed * Time.deltaTime);
    }

    public override void FixedUpdateState(FSM_NZ_Manager stateManager)
    {
    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        if (WanderingConditions())
            stateManager.SwitchState(stateManager.wanderingState);
    }

    private bool WanderingConditions() => owner.DetectedPlayers.Count == 0 && 
                                          Vector2.Distance(owner.transform.position, goalPosition) <= owner.DistanceBeforeStop;
}

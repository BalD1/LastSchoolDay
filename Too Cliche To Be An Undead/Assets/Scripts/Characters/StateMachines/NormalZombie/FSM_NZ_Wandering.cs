using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_NZ_Wandering : FSM_Base<FSM_NZ_Manager>
{
    private NormalZombie owner;
    private bool canSwitchToChase = false;

    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        canSwitchToChase = false;
        owner ??= stateManager.Owner;
    }

    public override void UpdateState(FSM_NZ_Manager stateManager)
    {

    }

    public override void FixedUpdateState(FSM_NZ_Manager stateManager)
    {

    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        if (canSwitchToChase) stateManager.SwitchState(stateManager.chasingState);
    }

    public void SawPlayer() => canSwitchToChase = true;
}

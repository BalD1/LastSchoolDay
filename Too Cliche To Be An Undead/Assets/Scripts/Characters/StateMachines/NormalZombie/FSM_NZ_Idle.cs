using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FSM_NZ_Idle : FSM_Entity_Idle<FSM_NZ_Manager>
{ 
    private bool canSwitchToChase;

    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        owner ??= stateManager.Owner;

        base.EnterState(stateManager);
    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
        canSwitchToChase = false;
        base.ExitState(stateManager);
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        if (canSwitchToChase) stateManager.SwitchState(stateManager.chasingState);

        base.Conditions(stateManager);
    }

    public void SawPlayer() => canSwitchToChase = true;
}

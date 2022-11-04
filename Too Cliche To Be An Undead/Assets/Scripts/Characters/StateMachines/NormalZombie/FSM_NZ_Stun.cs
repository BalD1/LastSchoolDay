using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_NZ_Stun : FSM_Entity_Stunned<FSM_NZ_Manager>
{
    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        owner ??= stateManager.Owner;
        base.EnterState(stateManager);

        owner.canBePushed = true;
    }

    public override void UpdateState(FSM_NZ_Manager stateManager)
    {
        base.UpdateState(stateManager);
    }

    public override void FixedUpdateState(FSM_NZ_Manager stateManager)
    {
    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
        base.ExitState(stateManager);
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(stateManager.chasingState);
    }

    public override string ToString() => "Stunned";
}

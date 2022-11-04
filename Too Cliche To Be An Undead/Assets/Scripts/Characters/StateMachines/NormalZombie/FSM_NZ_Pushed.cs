using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_NZ_Pushed : FSM_Entity_Pushed<FSM_NZ_Manager>
{
    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        base.EnterState(stateManager);

        owner.canBePushed = false;
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
    public override string ToString() => "Pushed";
}

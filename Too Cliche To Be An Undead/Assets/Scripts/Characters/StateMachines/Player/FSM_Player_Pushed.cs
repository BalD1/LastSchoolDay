using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Pushed : FSM_Entity_Pushed<FSM_Player_Manager>
{
    public override void Conditions(FSM_Player_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(stateManager.idleState);
    }

    public override string ToString() => "Pushed";
}

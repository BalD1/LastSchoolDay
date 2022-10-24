using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_TD_Pushed : FSM_Entity_Pushed<FSM_TD_Manager>
{
    public override void Conditions(FSM_TD_Manager stateManager)
    {
        base.Conditions(stateManager); 
        if (baseConditionChecked) stateManager.SwitchState(stateManager.idleState);
    }
    public override string ToString() => "Pushed";
}

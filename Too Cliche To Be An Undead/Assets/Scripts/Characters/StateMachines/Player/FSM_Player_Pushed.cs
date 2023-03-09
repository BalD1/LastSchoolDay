using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Pushed : FSM_Entity_Pushed<FSM_Player_Manager>
{
    public override void EnterState(FSM_Player_Manager stateManager)
    {
        base.EnterState(stateManager);
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        base.ExitState(stateManager);
        (owner as PlayerCharacter).ForceUpdateMovementsInput();
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(stateManager.idleState);
    }

    public override string ToString() => "Pushed";
}

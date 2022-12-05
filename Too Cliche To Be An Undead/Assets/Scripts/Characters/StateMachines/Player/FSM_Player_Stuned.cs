using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Stuned : FSM_Entity_Stunned<FSM_Player_Manager>
{
    public override void EnterState(FSM_Player_Manager stateManager)
    {
        owner ??= stateManager.Owner;
        base.EnterState(stateManager);

        owner.canBePushed = true;
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(stateManager.idleState);
    }

    public override string ToString() => "Stunned";
}

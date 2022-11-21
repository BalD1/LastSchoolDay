using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_TD_Stun : FSM_Entity_Stunned<FSM_TD_Manager>
{
    public override void EnterState(FSM_TD_Manager stateManager)
    {
        if (owner == null) owner = stateManager.Owner;
        base.EnterState(stateManager);
    }

    public override void UpdateState(FSM_TD_Manager stateManager)
    {
        base.UpdateState(stateManager);
    }

    public override void FixedUpdateState(FSM_TD_Manager stateManager)
    {
    }

    public override void ExitState(FSM_TD_Manager stateManager)
    {
        (owner as TrainingDummy).HideStatusText();
        base.ExitState(stateManager);
    }

    public override void Conditions(FSM_TD_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(stateManager.idleState);
    }

    public override string ToString() => "Stunned";
}

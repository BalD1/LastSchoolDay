using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_NZ_Pushed : FSM_Entity_Pushed<FSM_NZ_Manager>
{
    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        base.EnterState(stateManager);

        owner.canBePushed = false;

        (owner as NormalZombie).attackTelegraph.CancelTelegraph();
        (owner as NormalZombie).enemiesBlocker.enabled = false;
    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
        (owner as NormalZombie).enemiesBlocker.enabled = true;
        base.ExitState(stateManager);
    }
    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(stateManager.chasingState);
    }

    protected override void TriggerEnter(Collider2D collider)
    {
        //if (collider.GetComponentInParent<PlayerCharacter>() != null) return;
        
        base.TriggerEnter(collider);
    }
    public override string ToString() => "Pushed";
}

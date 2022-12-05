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
        (owner as NormalZombie).UnsetAttackedPlayer();
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(stateManager.chasingState);
    }

    public override string ToString() => "Stunned";
}

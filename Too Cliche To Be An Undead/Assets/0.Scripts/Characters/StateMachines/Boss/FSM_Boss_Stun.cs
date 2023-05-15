using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Boss_Stun : FSM_Entity_Stunned<FSM_Boss_Manager>
{
    public override void EnterState(FSM_Boss_Manager stateManager)
    {
        owner ??= stateManager.Owner;
        base.EnterState(stateManager);

        (owner as BossZombie).UnsetAttackedPlayer();
    }

    public override void Conditions(FSM_Boss_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(stateManager.chasingState);
    }

    public override string ToString() => "Stunned";
}

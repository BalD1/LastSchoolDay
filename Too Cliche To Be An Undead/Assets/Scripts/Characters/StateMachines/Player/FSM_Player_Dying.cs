using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Dying : FSM_Base<FSM_Player_Manager>
{
    public PlayerCharacter owner;
    private float dyingState_TIMER;

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        owner ??= stateManager.Owner;

        Debug.Log("yo");
        if (dyingState_TIMER <= 0) dyingState_TIMER = owner.DyingState_DURATION;

        owner.SetAnimatorArgs("Dying", true);
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        dyingState_TIMER -= Time.deltaTime;

        if (dyingState_TIMER <= 0) owner.DefinitiveDeath();
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        owner.SetAnimatorArgs("Dying", false);
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {

    }

    public override string ToString() => "Dying";
}

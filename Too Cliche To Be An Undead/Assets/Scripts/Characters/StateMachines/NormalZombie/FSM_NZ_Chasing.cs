using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_NZ_Chasing : FSM_Base<FSM_NZ_Manager>
{
    private NormalZombie owner;

    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        owner ??= stateManager.Owner;
    }

    public override void UpdateState(FSM_NZ_Manager stateManager)
    {

    }

    public override void FixedUpdateState(FSM_NZ_Manager stateManager)
    {

    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        // Si la velocité du personnage est à 0, on le passe en Idle

    }
}

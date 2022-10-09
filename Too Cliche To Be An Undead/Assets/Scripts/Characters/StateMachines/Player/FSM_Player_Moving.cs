using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Moving : FSM_Base<FSM_Player_Manager>
{
    public override void EnterState(FSM_Player_Manager stateManager)
    {
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        stateManager.Owner.ReadMovementsInputs();
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
        stateManager.Owner.Movements();
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        // Si la velocité du personnage est à 0, on le passe en Idle
        if (stateManager.Owner.Velocity.Equals(Vector2.zero))
            stateManager.SwitchState(stateManager.idleState);
    }
}

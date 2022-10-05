using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Idle : FSM_Base<FSM_Player_Manager>
{
    public override void EnterState(FSM_Player_Manager stateManager)
    {
        stateManager.Owner.GetRb.velocity = Vector2.zero;
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        if (stateManager.Owner.GetRb.velocity != Vector2.zero ||
            stateManager.Owner.Velocity != Vector2.zero)
        {
            stateManager.SwitchState(stateManager.movingState);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP_Tlol : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            GameManager.Player1Ref.StateManager.SwitchState<FSM_Player_Dashing>(FSM_Player_Manager.E_PlayerState.Dashing);
            GameManager.Player1Ref.StateManager.SwitchState<FSM_Player_Dying>(FSM_Player_Manager.E_PlayerState.Dying);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameManager.Player1Ref.StateManager.SwitchState<FSM_Player_Dying>(FSM_Player_Manager.E_PlayerState.Dying);
            GameManager.Player1Ref.StateManager.SwitchState<FSM_Player_Dashing>(FSM_Player_Manager.E_PlayerState.Dashing);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameManager.Player1Ref.StateManager.SwitchState<FSM_Player_Attacking>(FSM_Player_Manager.E_PlayerState.Attacking);
            GameManager.Player1Ref.StateManager.SwitchState<FSM_Player_Dying>(FSM_Player_Manager.E_PlayerState.Dying);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameManager.Player1Ref.StateManager.SwitchState<FSM_Player_Dying>(FSM_Player_Manager.E_PlayerState.Dying);
            GameManager.Player1Ref.StateManager.SwitchState<FSM_Player_Attacking>(FSM_Player_Manager.E_PlayerState.Attacking);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP_PlayerStatesTest : MonoBehaviour
{
    PlayerCharacter playerTarget;

    private void Start()
    {
        playerTarget = GameManager.Player1Ref;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerTarget.StateManager.ForceSetState<FSM_Player_Pushed>(FSM_Player_Manager.E_PlayerState.Pushed).SetForce(Vector2.up * 50, null, null);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerTarget.StateManager.ForceSetState(FSM_Player_Manager.E_PlayerState.Attacking);
            playerTarget.StateManager.ForceSetState(FSM_Player_Manager.E_PlayerState.Dying);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            playerTarget.StateManager.ForceSetState(FSM_Player_Manager.E_PlayerState.Dying);
            playerTarget.OnAttackInput?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            playerTarget.StateManager.ForceSetState(FSM_Player_Manager.E_PlayerState.Dashing);
            playerTarget.StateManager.ForceSetState(FSM_Player_Manager.E_PlayerState.Dying);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            playerTarget.StateManager.ForceSetState(FSM_Player_Manager.E_PlayerState.Dying);
            playerTarget.OnDashInput?.Invoke();
        }
    }
}

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
            playerTarget.StateManager.ForceSetState<FSM_Player_Pushed>(FSM_Player_Manager.E_PlayerState.Pushed).SetForce(Vector2.up * 10, null, null);
        }
    }
}

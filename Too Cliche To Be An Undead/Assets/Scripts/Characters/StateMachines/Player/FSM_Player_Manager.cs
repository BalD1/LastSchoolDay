using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Manager : FSM_ManagerBase
{
    [SerializeField] private PlayerCharacter owner;
    public PlayerCharacter Owner { get => owner; }

    public FSM_Player_Idle idleState = new FSM_Player_Idle();
    public FSM_Player_Moving movingState = new FSM_Player_Moving();

    private FSM_Base<FSM_Player_Manager> currentState;
    public FSM_Base<FSM_Player_Manager> CurrentState { get => currentState; }

    protected override void Start()
    {
        currentState = idleState;
        currentState.EnterState(this);
    }

    protected override void Update()
    {
        currentState.UpdateState(this);
        currentState.Conditions(this);

    }

    protected override void FixedUpdate()
    {
        currentState.FixedUpdateState(this);
    }

    public void SwitchState(FSM_Base<FSM_Player_Manager> newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    public override string ToString()
    {
        if (currentState == null) return "N/A";
        if (currentState.Equals(idleState)) return "Idle";
        if (currentState.Equals(movingState)) return "Moving";

        return "N/A";
    }
}

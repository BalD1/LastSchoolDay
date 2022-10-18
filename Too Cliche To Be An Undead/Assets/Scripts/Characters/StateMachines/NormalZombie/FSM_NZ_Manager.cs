using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_NZ_Manager : FSM_ManagerBase
{
    [SerializeField] private NormalZombie owner;
    public NormalZombie Owner { get => owner; }

    public FSM_NZ_Wandering wanderingState = new FSM_NZ_Wandering();
    public FSM_NZ_Chasing chasingState = new FSM_NZ_Chasing();

    private FSM_Base<FSM_NZ_Manager> currentState;
    public FSM_Base<FSM_NZ_Manager> CurrentState { get => currentState; }

    protected override void Start()
    {
        currentState = wanderingState;
        currentState.EnterState(this);
    }

    protected override void Update()
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;
        currentState.UpdateState(this);
        currentState.Conditions(this);

    }

    protected override void FixedUpdate()
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;
        currentState.FixedUpdateState(this);
    }

    public void SwitchState(FSM_Base<FSM_NZ_Manager> newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    public override string ToString()
    {
        if (currentState == null) return "N/A";
        if (currentState.Equals(wanderingState)) return "Wandering";
        if (currentState.Equals(chasingState)) return "Chasing";

        return "N/A";
    }
}

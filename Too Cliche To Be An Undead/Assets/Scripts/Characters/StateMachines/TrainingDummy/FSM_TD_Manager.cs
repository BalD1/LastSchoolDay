using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_TD_Manager : FSM_ManagerBase
{
    [SerializeField] private TrainingDummy owner;
    public TrainingDummy Owner { get => owner; }

    public FSM_TD_Idle idleState = new FSM_TD_Idle();
    public FSM_TD_Stun stunState = new FSM_TD_Stun();
    public FSM_TD_Pushed pushedState = new FSM_TD_Pushed(); 

    private FSM_Base<FSM_TD_Manager> currentState;
    public FSM_Base<FSM_TD_Manager> CurrentState { get => currentState; }

    private void Awake()
    {
        pushedState.SetOwner(Owner);
    }

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

    public void SwitchState(FSM_Base<FSM_TD_Manager> newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    public override string ToString()
    {
        if (currentState == null) return "N/A";
        if (currentState.Equals(idleState)) return "Idle";
        if (currentState.Equals(stunState)) return "Stun";

        return "N/A";
    }
}

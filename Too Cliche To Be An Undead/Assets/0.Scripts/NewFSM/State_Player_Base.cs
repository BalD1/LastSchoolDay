using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State_Player_Base : IState
{
    protected FSM_PlayerCharacter ownerFSM;

    public State_Player_Base(FSM_PlayerCharacter fsm)
    {
        ownerFSM = fsm;
    }

    public abstract void Conditions();
    public virtual void EnterState()
        => EventsSubscriber();
    public virtual void ExitState()
        => EventsUnSubscriber();
    public abstract void FixedUpdate();
    public abstract void Update();

    public abstract void EventsSubscriber();
    public abstract void EventsUnSubscriber();
}

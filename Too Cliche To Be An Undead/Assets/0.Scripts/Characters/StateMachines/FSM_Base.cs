using System;
using UnityEngine;

public abstract class FSM_Base<Manager, Key> where Key : Enum
                                             where Manager : FSM_ManagerBase
{
    [field: SerializeField] public Key StateKey { get; protected set; }
    public virtual void EnterState(Manager stateManager) => EventsSubscriber(stateManager);
    public abstract void UpdateState(Manager stateManager);
    public abstract void FixedUpdateState(Manager stateManager);
    public virtual void ExitState(Manager stateManager) => EventsUnsubscriber(stateManager);
    public abstract void Conditions(Manager stateManager);

    protected abstract void EventsSubscriber(Manager stateManager);
    protected abstract void EventsUnsubscriber(Manager stateManager);

    public abstract void Setup(Manager stateManager);
    public abstract override string ToString();

    public Key GetKey()
        => StateKey;
    public void SetKey(Key key)
        => StateKey = key;
}

public abstract class FSM_Base<T>
{
    public virtual void EnterState(T stateManager) => EventsSubscriber(stateManager);
    public abstract void UpdateState(T stateManager);
    public abstract void FixedUpdateState(T stateManager);
    public virtual void ExitState(T stateManager) => EventsUnsubscriber(stateManager);
    public abstract void Conditions(T stateManager);

    protected abstract void EventsSubscriber(T stateManager);
    protected abstract void EventsUnsubscriber(T stateManager);

    public abstract void Setup(T stateManager);
    public abstract override string ToString();
}
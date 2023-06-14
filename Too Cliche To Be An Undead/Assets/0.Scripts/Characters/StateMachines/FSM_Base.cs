
public abstract class FSM_Base<T>
{
    public virtual void EnterState(T stateManager) => EventsSubscriber();
    public abstract void UpdateState(T stateManager);
    public abstract void FixedUpdateState(T stateManager);
    public virtual void ExitState(T stateManager) => EventsUnsubscriber();
    public abstract void Conditions(T stateManager);

    protected abstract void EventsSubscriber();
    protected abstract void EventsUnsubscriber();

    public abstract void Setup(T stateManager);
    public abstract override string ToString();
}
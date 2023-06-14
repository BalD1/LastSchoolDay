using UnityEngine;

public class FSM_Entity_Idle<T> : FSM_Base<T>
{
    protected Entity owner;
    protected bool baseConditionChecked;

    public override void EnterState(T stateManager)
    {
        base.EnterState(stateManager);

        if (owner.GetRb != null)
            owner.GetRb.velocity = Vector2.zero;
    }

    public override void UpdateState(T stateManager)
    {
    }

    public override void FixedUpdateState(T stateManager)
    {
    }

    public override void ExitState(T stateManager)
    {
        base.ExitState(stateManager);
        baseConditionChecked = false;
    }

    public override void Conditions(T stateManager)
    {
        if (owner.GetRb?.velocity != Vector2.zero) baseConditionChecked = true;
    }

    protected override void EventsSubscriber()
    {
    }

    protected override void EventsUnsubscriber()
    {
    }

    public override void Setup(T stateManager)
    {
        throw new System.NotImplementedException();
    }

    public override string ToString() => "Idle";
}

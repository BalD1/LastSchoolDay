using UnityEngine;

public class FSM_Entity_Stunned<T> : FSM_Base<T>
{
    protected Entity owner;
    protected float stun_TIMER;
    protected bool baseConditionChecked;

    public override void EnterState(T stateManager)
    {
        base.EnterState(stateManager);
        owner.GetRb.velocity = Vector3.zero;
    }

    public override void UpdateState(T stateManager)
    {
        if (stun_TIMER > 0) stun_TIMER -= Time.deltaTime;
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
        if (stun_TIMER <= 0) baseConditionChecked = true;   
    }

    protected override void EventsSubscriber()
    {
    }

    protected override void EventsUnsubscriber()
    {
    }

    public void SetOwner(Entity _owner) => owner = _owner;

    public FSM_Entity_Stunned<T> SetDuration(float duration, bool resetAttackTimer = false)
    {
        stun_TIMER = duration;
        if (owner != null && resetAttackTimer) owner.ResetAttackTimer();
        return this;
    }

    public override void Setup(T stateManager)
    {
        throw new System.NotImplementedException();
    }

    public override string ToString() => "Stunned";
}

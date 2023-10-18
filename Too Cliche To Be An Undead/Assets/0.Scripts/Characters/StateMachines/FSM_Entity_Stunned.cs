using System;
using UnityEngine;

public class FSM_Entity_Stunned<Manager, Key> : FSM_Base<Manager, Key> where Manager : FSM_ManagerBase
                                                                       where Key : Enum
{
    protected Entity owner;
    protected float stun_TIMER;
    protected bool baseConditionChecked;

    public override void EnterState(Manager stateManager)
    {
        base.EnterState(stateManager);
        owner.GetRb.velocity = Vector3.zero;
    }

    public override void UpdateState(Manager stateManager)
    {
        if (stun_TIMER > 0) stun_TIMER -= Time.deltaTime;
    }

    public override void FixedUpdateState(Manager stateManager)
    {
    }

    public override void ExitState(Manager stateManager)
    {
        base.ExitState(stateManager);
        baseConditionChecked = false;
    }

    public override void Conditions(Manager stateManager)
    {
        if (stun_TIMER <= 0) baseConditionChecked = true;   
    }

    protected override void EventsSubscriber(Manager stateManager)
    {
        owner.OnAskForStun += OnStunAddition;
    }

    protected override void EventsUnsubscriber(Manager stateManager)
    {
        owner.OnAskForStun -= OnStunAddition;
    }

    private void OnStunAddition(float duration, bool resetAttackTimer, bool showText)
    {
        if (showText) TextPopup.Create("Stunned", owner.transform);
        stun_TIMER += duration;
        if (resetAttackTimer) owner.ResetAttackTimer();
    }

    public void SetOwner(Entity _owner) => owner = _owner;

    public FSM_Entity_Stunned<Manager, Key> SetDuration(float duration, bool resetAttackTimer = false)
    {
        stun_TIMER = duration;
        if (owner != null && resetAttackTimer) owner.ResetAttackTimer();
        return this;
    }

    public override void Setup(Manager stateManager)
    {
        throw new System.NotImplementedException();
    }

    public override string ToString() => "Stunned";
}

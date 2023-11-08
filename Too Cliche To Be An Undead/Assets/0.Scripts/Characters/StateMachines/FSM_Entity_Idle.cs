using System;
using UnityEngine;

public class FSM_Entity_Idle<Manager, KeyName> : FSM_Base<Manager, KeyName>
                                                 where Manager : FSM_ManagerBase
                                                 where KeyName : Enum
{
    protected Entity owner;
    protected bool baseConditionChecked;

    public override void EnterState(Manager stateManager)
    {
        base.EnterState(stateManager);

        //if (owner.GetRb != null)
            //owner.GetRb.velocity = Vector2.zero;
    }

    public override void UpdateState(Manager stateManager)
    {
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
        //if (owner.GetRb?.velocity != Vector2.zero) baseConditionChecked = true;
    }

    protected override void EventsSubscriber(Manager stateManager)
    {
    }

    protected override void EventsUnsubscriber(Manager stateManager)
    {
    }

    public override void Setup(Manager stateManager)
    {
        throw new System.NotImplementedException();
    }

    public override string ToString() => "Idle";
}

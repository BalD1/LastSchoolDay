
public class FSM_NZ_Stun : FSM_Entity_Stunned<FSM_NZ_Manager, FSM_NZ_Manager.E_NZState>
{
    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        base.EnterState(stateManager);

        owner.canBePushed = true;
        (owner as NormalZombie).UnsetAttackedPlayer();
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(FSM_NZ_Manager.E_NZState.Chasing);
    }

    protected override void EventsSubscriber(FSM_NZ_Manager stateManager)
    {
        base.EventsSubscriber(stateManager);
        owner.OnAskForPush += stateManager.SwitchToPushed;
    }

    protected override void EventsUnsubscriber(FSM_NZ_Manager stateManager)
    {
        base.EventsUnsubscriber(stateManager);
        owner.OnAskForPush -= stateManager.SwitchToPushed;
    }

    public override void Setup(FSM_NZ_Manager stateManager)
    {
        owner = stateManager.Owner;
    }

    public override string ToString() => "Stunned";
}

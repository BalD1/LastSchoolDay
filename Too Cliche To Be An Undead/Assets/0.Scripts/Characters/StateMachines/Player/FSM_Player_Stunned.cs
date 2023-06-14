
public class FSM_Player_Stunned : FSM_Entity_Stunned<FSM_Player_Manager>
{
    public FSM_Player_Manager.E_PlayerState StateName { get; private set; }

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        base.EnterState(stateManager);

        owner.canBePushed = true;
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(FSM_Player_Manager.E_PlayerState.Idle);
        this.CheckDying(stateManager);
    }

    public override void Setup(FSM_Player_Manager stateManager)
    {
        owner = stateManager.Owner;
        StateName = FSM_Player_Manager.E_PlayerState.Stunned;
    }

    protected override void EventsSubscriber(FSM_Player_Manager stateManager)
    {
        base.EventsSubscriber(stateManager);
        owner.OnAskForPush += stateManager.PushConditions;
    }

    protected override void EventsUnsubscriber(FSM_Player_Manager stateManager)
    {
        base.EventsUnsubscriber(stateManager);
        owner.OnAskForPush -= stateManager.PushConditions;
    }

    public override string ToString() => StateName.ToString();
}

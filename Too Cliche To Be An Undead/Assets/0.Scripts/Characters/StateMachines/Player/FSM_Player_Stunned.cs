
public class FSM_Player_Stunned : FSM_Entity_Stunned<FSM_Player, FSM_Player.E_PlayerState>
{
    public FSM_Player.E_PlayerState StateName { get; private set; }

    public override void EnterState(FSM_Player stateManager)
    {
        base.EnterState(stateManager);

        //owner.canBePushed = true;
    }

    public override void ExitState(FSM_Player stateManager)
    {
        base.ExitState(stateManager);
        //(owner as PlayerCharacter).PlayerInputsComponent.ForceReadMovements();
    }

    public override void Conditions(FSM_Player stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(FSM_Player.E_PlayerState.Idle);
        this.CheckDying(stateManager);
    }

    public override void Setup(FSM_Player stateManager)
    {
        owner = stateManager.Owner;
        StateName = FSM_Player.E_PlayerState.Stunned;
    }

    protected override void EventsSubscriber(FSM_Player stateManager)
    {
        base.EventsSubscriber(stateManager);
        //owner.OnAskForPush += stateManager.PushConditions;
        CinematicManagerEvents.OnChangeCinematicState += stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue += stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue += stateManager.DialogueEnded;
    }

    protected override void EventsUnsubscriber(FSM_Player stateManager)
    {
        base.EventsUnsubscriber(stateManager);
        //owner.OnAskForPush -= stateManager.PushConditions;
        CinematicManagerEvents.OnChangeCinematicState -= stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue -= stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue -= stateManager.DialogueEnded;
    }

    public override string ToString() => StateName.ToString();
}

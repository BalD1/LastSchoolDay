
public class FSM_Player_Cinematic : FSM_Base<FSM_Player, FSM_Player.E_PlayerState>
{
    public FSM_Player.E_PlayerState StateName { get; private set; }

    public override void EnterState(FSM_Player stateManager)
    {
        base.EnterState(stateManager);
        //stateManager.Owner.AnimationController.SetAnimation(stateManager.Owner.AnimationsData.IdleAnim, true);
    }

    public override void UpdateState(FSM_Player stateManager)
    {
    }

    public override void FixedUpdateState(FSM_Player stateManager)
    {
    }

    public override void ExitState(FSM_Player stateManager)
    {
        base.ExitState(stateManager);
        //stateManager.Owner.PlayerInputsComponent.ForceReadMovements();
    }

    public override void Conditions(FSM_Player stateManager)
    {
    }

    protected override void EventsSubscriber(FSM_Player stateManager)
    {
        CinematicManagerEvents.OnChangeCinematicState += stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue += stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue += stateManager.DialogueEnded;
    }

    protected override void EventsUnsubscriber(FSM_Player stateManager)
    {
        CinematicManagerEvents.OnChangeCinematicState -= stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue -= stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue -= stateManager.DialogueEnded;
    }

    public override void Setup(FSM_Player stateManager)
    {
        StateName = FSM_Player.E_PlayerState.Cinematic;
    }

    public override string ToString() => StateName.ToString();


}


public class FSM_Player_Cinematic : FSM_Base<FSM_Player_Manager>
{
    public FSM_Player_Manager.E_PlayerState StateName { get; private set; }

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        base.EnterState(stateManager);
        stateManager.Owner.AnimationController.SetAnimation(stateManager.Owner.AnimationController.AnimationsData.IdleAnim, true);
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        base.ExitState(stateManager);
        stateManager.Owner.PlayerInputsComponent.ForceReadMovements();
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
    }

    protected override void EventsSubscriber(FSM_Player_Manager stateManager)
    {
        CinematicManagerEvents.OnChangeCinematicState += stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue += stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue += stateManager.DialogueEnded;
    }

    protected override void EventsUnsubscriber(FSM_Player_Manager stateManager)
    {
        CinematicManagerEvents.OnChangeCinematicState -= stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue -= stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue -= stateManager.DialogueEnded;
    }

    public override void Setup(FSM_Player_Manager stateManager)
    {
        StateName = FSM_Player_Manager.E_PlayerState.Cinematic;
    }

    public override string ToString() => StateName.ToString();


}

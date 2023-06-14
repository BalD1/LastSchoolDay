
public class FSM_Player_Cinematic : FSM_Base<FSM_Player_Manager>
{
    public override void EnterState(FSM_Player_Manager stateManager)
    {
        base.EnterState(stateManager);
        stateManager.Owner.AnimationController.SetAnimation(stateManager.Owner.AnimationController.animationsData.IdleAnim, true);
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
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
    }

    protected override void EventsSubscriber()
    {
    }

    protected override void EventsUnsubscriber()
    {
    }

    public override void Setup(FSM_Player_Manager stateManager)
    {
    }

    public override string ToString() => "Cinematic";


}


public class FSM_NZ_Wandering : FSM_Base<FSM_NZ_Manager>
{
    private NormalZombie owner;
    private bool canSwitchToChase = true;

    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        base.EnterState(stateManager);
        owner.ResetVelocity();

        if (owner.allowWander) owner.ChooseRandomPosition();
        else owner.Pathfinding.StopUpdatepath();

        owner.canBePushed = true;
    }

    public override void UpdateState(FSM_NZ_Manager stateManager)
    {
        if (!owner.allowWander) return;
        if (owner.Pathfinding == null) return;
    }

    public override void FixedUpdateState(FSM_NZ_Manager stateManager)
    {
    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
        base.ExitState(stateManager);
    }

    protected override void EventsSubscriber(FSM_NZ_Manager stateManager)
    {
    }

    protected override void EventsUnsubscriber(FSM_NZ_Manager stateManager)
    {
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        if (owner.CurrentPlayerTarget != null) stateManager.SwitchState(stateManager.ChasingState);
    }

    public override void Setup(FSM_NZ_Manager stateManager)
    {
        owner = stateManager.Owner;
    }

    public override string ToString() => "Wandering";
}

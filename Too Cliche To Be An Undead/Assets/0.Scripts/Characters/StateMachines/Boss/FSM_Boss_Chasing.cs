using UnityEngine;

public class FSM_Boss_Chasing : FSM_Base<FSM_Boss_Manager>
{
    private BossZombie owner;
    private Vector2 goalPosition;

    public override void EnterState(FSM_Boss_Manager stateManager)
    {
        base.EnterState(stateManager);
        owner.ResetVelocity();

        owner.animationController.SetAnimation(owner.animationData.WalkAnim, true);

        owner.Pathfinding.StopUpdatepath();
        owner.Pathfinding.StartUpdatePath();

        owner.canBePushed = true;
    }

    public override void UpdateState(FSM_Boss_Manager stateManager)
    {
        if (owner.Pathfinding != null)
            goalPosition = owner.Pathfinding.CheckWayPoint();
    }

    public override void FixedUpdateState(FSM_Boss_Manager stateManager)
    {
        stateManager.Movements(goalPosition);
    }

    public override void ExitState(FSM_Boss_Manager stateManager)
    {
        base.ExitState(stateManager);
        owner.GetRb.velocity = Vector2.zero;
    }

    public override void Conditions(FSM_Boss_Manager stateManager)
    {
        if (owner.CurrentPlayerTarget == null) return;

        if (stateManager.AttackConditions())
            stateManager.SwitchState(stateManager.AttackingState);
    }

    protected override void EventsSubscriber(FSM_Boss_Manager stateManager)
    {
    }

    protected override void EventsUnsubscriber(FSM_Boss_Manager stateManager)
    {
    }

    private bool WanderingConditions() => owner.CurrentPlayerTarget == null;

    public override void Setup(FSM_Boss_Manager stateManager)
    {
        owner = stateManager.Owner;
    }
    public override string ToString() => "Chasing";
}

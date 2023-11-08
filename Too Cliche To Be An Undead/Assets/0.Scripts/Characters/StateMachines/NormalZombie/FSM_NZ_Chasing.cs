using UnityEngine;

public class FSM_NZ_Chasing : FSM_Base<FSM_NZ_Manager, FSM_NZ_Manager.E_NZState>
{
    private BaseZombie owner;
    private Vector2 goalPosition;

    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        base.EnterState(stateManager);
        owner = stateManager.Owner;
        //owner.ResetVelocity();

        //if (owner.animationController != null)
         //   owner.animationController.SetAnimation(owner.AnimationData.WalkAnim, true);

        //owner.Pathfinding.StopUpdatepath();
        //owner.Pathfinding.StartUpdatePath();

        //owner.canBePushed = true;

        //owner.OnStartChasing?.Invoke(owner.CurrentPlayerTarget);
    }

    public override void UpdateState(FSM_NZ_Manager stateManager)
    {
        //if (owner.Pathfinding != null)
        //    goalPosition = owner.Pathfinding.CheckWayPoint();

        //owner.SetSpeedOnDistanceFromTarget();
    }

    public override void FixedUpdateState(FSM_NZ_Manager stateManager)
    {
        stateManager.Movements(goalPosition);

        //if (owner.animationController == null) return;
        //if (owner.CurrentTransformTarget == null) return;

        //if (owner.transform.position.x > owner.CurrentTransformTarget.position.x) owner.animationController.TryFlipSkeleton(false);
        //else owner.animationController.TryFlipSkeleton(true);
    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
        base.ExitState(stateManager);
        //owner.GetRb.velocity = Vector2.zero;
        //owner.OnStoppedChasing?.Invoke();
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        //if (WanderingConditions())
            stateManager.SwitchState(FSM_NZ_Manager.E_NZState.Wandering);

        //if (owner.CurrentPlayerTarget == null) return;

        if (stateManager.AttackConditions())
            stateManager.SwitchState(FSM_NZ_Manager.E_NZState.Attacking);
    }

    protected override void EventsSubscriber(FSM_NZ_Manager stateManager)
    {
        owner.OnAskForStun += stateManager.SwitchToStun;
        //owner.OnAskForPush += stateManager.SwitchToPushed;
    }

    protected override void EventsUnsubscriber(FSM_NZ_Manager stateManager)
    {
        owner.OnAskForStun -= stateManager.SwitchToStun;
        //owner.OnAskForPush -= stateManager.SwitchToPushed;
    }

    //private bool WanderingConditions() => owner.CurrentPlayerTarget == null;

    public override void Setup(FSM_NZ_Manager stateManager)
    {
        owner = stateManager.Owner;
    }

    public override string ToString() => "Chasing";
}

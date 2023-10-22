using UnityEngine;

public class FSM_NZ_Idle : FSM_Entity_Idle<FSM_NZ_Manager, FSM_NZ_Manager.E_NZState>
{ 
    private bool canSwitchToChase;
    private float waitBeforeNewPoint;

    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        base.EnterState(stateManager);

        waitBeforeNewPoint = Random.Range((owner as NormalZombie).MinWaitBeforeNewWanderPoint, (owner as NormalZombie).MaxWaitBeforeNewWanderPoint);

        owner.canBePushed = true;
    }

    public override void UpdateState(FSM_NZ_Manager stateManager)
    {
        if (waitBeforeNewPoint > 0) waitBeforeNewPoint -= Time.deltaTime;
        base.UpdateState(stateManager);
    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
        base.ExitState(stateManager);
        canSwitchToChase = false;
        waitBeforeNewPoint = 0;
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        if (canSwitchToChase) stateManager.SwitchState(FSM_NZ_Manager.E_NZState.Chasing);

        if (waitBeforeNewPoint <= 0) stateManager.SwitchState(FSM_NZ_Manager.E_NZState.Wandering);

        base.Conditions(stateManager);
    }

    protected override void EventsSubscriber(FSM_NZ_Manager stateManager)
    {
        base.EventsSubscriber(stateManager);
        owner.OnAskForStun += stateManager.SwitchToStun;
        owner.OnAskForPush += stateManager.SwitchToPushed;
    }

    protected override void EventsUnsubscriber(FSM_NZ_Manager stateManager)
    {
        base.EventsUnsubscriber(stateManager);
        owner.OnAskForStun -= stateManager.SwitchToStun;
        owner.OnAskForPush -= stateManager.SwitchToPushed;
    }

    public override void Setup(FSM_NZ_Manager stateManager)
    {
        owner = stateManager.Owner;
    }

    public void SawPlayer() => canSwitchToChase = true;
}

using UnityEngine;

public class FSM_Boss_Recovering : FSM_Base<FSM_Boss_Manager>
{
    private BossZombie owner;

    private float recovering_TIMER;

    public override void EnterState(FSM_Boss_Manager stateManager)
    {
        base.EnterState(stateManager);
        owner.animationController.SetAnimation(owner.animationData.IdleAnim, true);
    }

    public override void UpdateState(FSM_Boss_Manager stateManager)
    {
        recovering_TIMER -= Time.deltaTime;
    }

    public override void FixedUpdateState(FSM_Boss_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Boss_Manager stateManager)
    {
        base.ExitState(stateManager);
    }

    public FSM_Boss_Recovering SetTimer(float time)
    {
        recovering_TIMER = time * (owner != null ? owner.recoverTimerModifier : 1);
        return this;
    }

    public override void Conditions(FSM_Boss_Manager stateManager)
    {
        if (recovering_TIMER <= 0) stateManager.SwitchState(stateManager.ChasingState);
    }

    protected override void EventsSubscriber()
    {
    }

    protected override void EventsUnsubscriber()
    {
    }

    public override void Setup(FSM_Boss_Manager stateManager)
    {
        owner = stateManager.Owner;
    }

    public override string ToString() => "Recovering";

}

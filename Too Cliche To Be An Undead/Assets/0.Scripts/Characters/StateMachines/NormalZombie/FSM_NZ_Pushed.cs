using UnityEngine;

public class FSM_NZ_Pushed : FSM_Entity_Pushed<FSM_NZ_Manager>
{
    NormalZombie zombieOwner;

    private float hitStopTimeBase = .10f;
    private bool hitStopWasPerformed = false;

    private LTDescr hitTween;

    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        base.EnterState(stateManager);
        hitStopWasPerformed = false;
        owner.canBePushed = false;

        zombieOwner.UnsetAttackedPlayer();
        zombieOwner.attackTelegraph.CancelTelegraph();
        zombieOwner.enemiesBlocker.enabled = false;
    }

    public override void UpdateState(FSM_NZ_Manager stateManager)
    {
        base.UpdateState(stateManager);
    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
        base.ExitState(stateManager);
        hitStopWasPerformed = false;
        zombieOwner.enemiesBlocker.enabled = true;
    }
    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked && hitStopWasPerformed) stateManager.SwitchState(FSM_NZ_Manager.E_NZState.Chasing);
    }

    protected void CancelTween()
    {
        if (hitTween != null)
                LeanTween.cancel(hitTween.uniqueId);
    }

    protected override void PerformPush(Entity pusher)
    {
        if (pusher is NormalZombie)
        {
            owner.GetRb.AddForce(force, ForceMode2D.Impulse);
            LeanTween.delayedCall(.1f, () => hitStopWasPerformed = true);
            return;
        }

        hitTween = LeanTween.delayedCall(hitStopTimeBase, () =>
        {
            if (owner != null && owner.GetRb != null)
            owner.GetRb.AddForce(force, ForceMode2D.Impulse);
            LeanTween.delayedCall(.1f, () => hitStopWasPerformed = true);
        });
    }

    protected override void TriggerEnter(Collider2D collider)
    {
        base.TriggerEnter(collider);
    }

    protected override void EventsSubscriber(FSM_NZ_Manager stateManager)
    {
        base.EventsSubscriber(stateManager);
        owner.OnReset += CancelTween;
        owner.OnAskForStun += stateManager.SwitchToStun;
        owner.OnAskForPush += stateManager.SwitchToPushed;
    }

    protected override void EventsUnsubscriber(FSM_NZ_Manager stateManager)
    {
        base.EventsUnsubscriber(stateManager);
        owner.OnReset -= CancelTween;
        owner.OnAskForStun -= stateManager.SwitchToStun;
        owner.OnAskForPush -= stateManager.SwitchToPushed;
    }

    public override void Setup(FSM_NZ_Manager stateManager)
    {
        owner = stateManager.Owner;
        zombieOwner = owner as NormalZombie;
    }

    public override string ToString() => "Pushed";
}

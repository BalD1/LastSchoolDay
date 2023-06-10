using System.Collections;
using UnityEngine;

public class FSM_NZ_Pushed : FSM_Entity_Pushed<FSM_NZ_Manager>
{
    NormalZombie zombieOwner;

    private float hitStopTimeBase = .15f;
    private bool hitStopWasPerformed = false;

    private LTDescr hitTween;

    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        base.EnterState(stateManager);
        zombieOwner = owner as NormalZombie;

        hitStopWasPerformed = false;
        owner.canBePushed = false;
        owner.D_OnReset += CancelTween;

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
        hitStopWasPerformed = false;
        owner.D_OnReset -= CancelTween;
        zombieOwner.enemiesBlocker.enabled = true;
        base.ExitState(stateManager);
    }
    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked && hitStopWasPerformed) stateManager.SwitchState(stateManager.chasingState);
    }

    protected void CancelTween()
    {
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
            owner.GetRb.AddForce(force, ForceMode2D.Impulse);
            LeanTween.delayedCall(.1f, () => hitStopWasPerformed = true);
        });
    }

    protected override void TriggerEnter(Collider2D collider)
    {
        base.TriggerEnter(collider);
    }
    public override string ToString() => "Pushed";
}

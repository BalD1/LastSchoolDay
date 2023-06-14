using Spine.Unity;

public class FSM_Boss_Dead : FSM_Base<FSM_Boss_Manager>
{
    BossZombie owner;

    private bool wasAttacking;

    public override void EnterState(FSM_Boss_Manager stateManager)
    {
        base.EnterState(stateManager);

        if (owner.IsAttacking)
        {
            wasAttacking = true;
            owner.D_currentAttackEnded += SetAnimation;
        }
        else SetAnimation();
    }

    private void SetAnimation()
    {
        AnimationReferenceAsset deathAnim = owner.animationData.DeathAnim;

        owner.animationController.SetAnimation(deathAnim, false);

        if (wasAttacking) owner.D_currentAttackEnded -= SetAnimation;
    }

    public override void UpdateState(FSM_Boss_Manager stateManager)
    {
    }

    public override void FixedUpdateState(FSM_Boss_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Boss_Manager stateManager)
    {
        base.ExitState(stateManager);
        wasAttacking = false;
    }

    public override void Conditions(FSM_Boss_Manager stateManager)
    {
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

    public override string ToString()
    {
        return "Dead";
    }
}

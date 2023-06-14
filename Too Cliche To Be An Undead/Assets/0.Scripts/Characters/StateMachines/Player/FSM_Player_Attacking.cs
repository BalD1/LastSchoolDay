using UnityEngine;

public class FSM_Player_Attacking : FSM_Base<FSM_Player_Manager>
{
    public PlayerCharacter owner;

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        base.EnterState(stateManager);

        Vector2 mouseDir = stateManager.Owner.Weapon.GetGeneralDirectionOfMouseOrGamepad();

        owner.canBePushed = true;

        PlayerAnimationController ownerAnims = owner.AnimationController;

        if (ownerAnims.animationsData == null) return;

        switch (owner.Weapon.GetGeneralDirectionOfMouseOrGamepad())
        {
            case Vector2 v when v == Vector2.up:
                ownerAnims.FlipSkeleton(false);
                ownerAnims.SetAnimation(ownerAnims.animationsData.AttackAnim_up, false);
                break;

            case Vector2 v when v == Vector2.down:
                ownerAnims.FlipSkeleton(true);
                ownerAnims.SetAnimation(ownerAnims.animationsData.AttackAnim_down, false);
                break;

            case Vector2 v when v == Vector2.left:
                ownerAnims.FlipSkeleton(false);
                ownerAnims.SetAnimation(ownerAnims.animationsData.AttackAnim_side, false);
                break;

            case Vector2 v when v == Vector2.right:
                ownerAnims.FlipSkeleton(true);
                ownerAnims.SetAnimation(ownerAnims.animationsData.AttackAnim_side, false);
                break;
        }
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

        owner.ForceUpdateMovementsInput();
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        if (owner.IsAlive() == false) return;

        if (owner.Weapon.attackEnded)
        {
            owner.Weapon.SetAttackEnded(false);
            owner.StateManager.SwitchState(owner.StateManager.IdleState);
        }

        if (owner.isDashing)
        {
            owner.CancelAttackAnimation();
            stateManager.SwitchState(stateManager.DashingState);
        }
    }

    protected override void EventsSubscriber()
    {
        owner.Weapon.D_nextAttack += NextAttack;
        owner.OnAttackInput += owner.Weapon.AskForAttack;
        owner.OnDashInput += owner.StartDash;
    }

    protected override void EventsUnsubscriber()
    {
        owner.Weapon.D_nextAttack -= NextAttack;
        owner.OnAttackInput -= owner.Weapon.AskForAttack;
        owner.OnDashInput -= owner.StartDash;
    }

    public void NextAttack(int currentAttackIndex)
    {
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_ATTACKINDEX, currentAttackIndex);
    }

    public override void Setup(FSM_Player_Manager stateManager)
    {
        owner = stateManager.Owner;
    }

    public override string ToString() => "Attacking";
}

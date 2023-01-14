using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Attacking : FSM_Base<FSM_Player_Manager>
{
    public PlayerCharacter owner;

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        owner ??= stateManager.Owner;

        Vector2 mouseDir = stateManager.Owner.Weapon.GetGeneralDirectionOfMouseOrGamepad();

        owner.canBePushed = true;

        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_HORIZONTAL, mouseDir.x);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_VERTICAL, mouseDir.y);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_ATTACKING, true);

        PlayerAnimationController ownerAnims = owner.AnimationController;

        owner.SkeletonAnimation.skeleton.SetToSetupPose();
        switch (owner.Weapon.GetGeneralDirectionOfMouseOrGamepad())
        {
            case Vector2 v when v == Vector2.up:
                ownerAnims.FlipSkeleton(false);
                ownerAnims.SetAnimation(ownerAnims.animationsData.attackAnim_up, false);
                break;

            case Vector2 v when v == Vector2.down:
                ownerAnims.FlipSkeleton(false);
                ownerAnims.SetAnimation(ownerAnims.animationsData.attackAnim_down, false);
                break;

            case Vector2 v when v == Vector2.left:
                ownerAnims.FlipSkeleton(false);
                ownerAnims.SetAnimation(ownerAnims.animationsData.attackAnim_side, false);
                break;

            case Vector2 v when v == Vector2.right:
                ownerAnims.FlipSkeleton(true);
                ownerAnims.SetAnimation(ownerAnims.animationsData.attackAnim_side, false);
                break;
        }

        owner.D_attackInput += owner.Weapon.AskForAttack;
        owner.D_dashInput += owner.StartDash;
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_ATTACKING, false);

        owner.ForceUpdateMovementsInput();

        owner.D_attackInput -= owner.Weapon.AskForAttack;
        owner.D_dashInput -= owner.StartDash;
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        if (owner.isDashing)
        {
            owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_ATTACKING, false);
            owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_ATTACKINDEX, 0);
            owner.CancelAttackAnimation();
            stateManager.SwitchState(stateManager.dashingState);
        }
    }

    public void NextAttack(int currentAttackIndex)
    {
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_ATTACKINDEX, currentAttackIndex);
    }
    public override string ToString() => "Attacking";
}

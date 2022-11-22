using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Attacking : FSM_Base<FSM_Player_Manager>
{
    public PlayerCharacter owner;

    private int currentAttackIdx = 0;
    public int CurrentAttackIdx { get => currentAttackIdx; }

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        owner ??= stateManager.Owner;

        Vector2 mouseDir = stateManager.Owner.Weapon.GetGeneralDirectionOfMouseOrGamepad();

        owner.canBePushed = true;

        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_HORIZONTAL, mouseDir.x);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_VERTICAL, mouseDir.y);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_ATTACKING, true);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_ATTACKINDEX, currentAttackIdx);

        owner.D_attackInput += owner.StoreInput;
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
        owner.D_attackInput -= owner.StoreInput;
        owner.D_dashInput -= owner.StartDash;

        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_ATTACKING, false);

        owner.ForceUpdateMovementsInput();
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
        currentAttackIdx = currentAttackIndex;
    }
    public override string ToString() => "Attacking";
}

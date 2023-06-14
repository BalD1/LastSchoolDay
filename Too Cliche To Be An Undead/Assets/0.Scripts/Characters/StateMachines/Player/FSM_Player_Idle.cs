using BalDUtilities.MouseUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Idle : FSM_Entity_Idle<FSM_Player_Manager>
{
    private new PlayerCharacter owner;

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        owner ??= stateManager.Owner;
        base.owner = stateManager.Owner;

        owner.SetAllVelocity(Vector2.zero);
        base.EnterState(stateManager);

        owner.OnAttackInput += owner.Weapon.AskForAttack;
        owner.OnSkillInput += owner.GetSkillHolder.StartSkill;
        owner.OnDashInput += owner.StartDash;

        owner.canBePushed = true;
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        stateManager.OwnerWeapon.SetRotation();

        owner.AnimationController.FlipSkeletonOnMouseOrGamepad();
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        base.ExitState(stateManager);

        owner.OnAttackInput -= owner.Weapon.AskForAttack;
        owner.OnSkillInput -= owner.GetSkillHolder.StartSkill;
        owner.OnDashInput -= owner.StartDash;
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        // Si la vélocité du personnage n'est pas à 0, on le passe en Moving
        if (owner.GetRb.velocity != Vector2.zero ||
            owner.Velocity != Vector2.zero)
        {
            stateManager.SwitchState(stateManager.movingState);
        }

        if (stateManager.OwnerWeapon.isAttacking)
            stateManager.SwitchState(stateManager.attackingState);

        if (owner.isDashing) stateManager.SwitchState(stateManager.dashingState);
    }
}

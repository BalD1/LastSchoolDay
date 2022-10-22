using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Idle : FSM_Base<FSM_Player_Manager>
{
    private PlayerCharacter owner;

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        owner ??= stateManager.Owner;

        owner.GetRb.velocity = Vector2.zero;
        owner.GetAnimator.SetFloat("Velocity", 0);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_VELOCITY, 0f);
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        owner.ReadMovementsInputs();
        stateManager.OwnerWeapon.FollowMouse();

        if (Input.GetMouseButtonDown(0)) owner.StartAttack();
        if (Input.GetMouseButtonDown(1)) owner.GetSkillHolder.UseSkill();
        if (Input.GetKeyDown(KeyCode.LeftShift)) owner.StartDash();
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
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

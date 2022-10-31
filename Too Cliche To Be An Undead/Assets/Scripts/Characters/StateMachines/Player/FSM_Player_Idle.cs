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

        owner.D_attackInput += owner.StartAttack;
        owner.D_skillInput += owner.GetSkillHolder.StartSkill;
        owner.D_dashInput += owner.StartDash;
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        //owner.ReadMovementsInputs();
        stateManager.OwnerWeapon.FollowMouse();
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        base.ExitState(stateManager);

        owner.D_attackInput -= owner.StartAttack;
        owner.D_skillInput -= owner.GetSkillHolder.StartSkill;
        owner.D_dashInput -= owner.StartDash;
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

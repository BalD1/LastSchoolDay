using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Moving : FSM_Base<FSM_Player_Manager>
{
    private PlayerCharacter owner;

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        owner ??= stateManager.Owner;

        SetAnimator();
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        owner.ReadMovementsInputs();

        SetAnimator();
        stateManager.OwnerWeapon.FollowMouse();
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
        owner.Movements();
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        // Si la velocité du personnage est à 0, on le passe en Idle
        if (owner.Velocity.Equals(Vector2.zero))
            stateManager.SwitchState(stateManager.idleState);

        if (stateManager.OwnerWeapon.isAttacking)
            stateManager.SwitchState(stateManager.attackingState);

        if (owner.isDashing)
            stateManager.SwitchState(stateManager.dashingState);
    }

    private void SetAnimator()
    {
        float x = owner.Velocity.x;
        if (x > 0) x = 1;
        else if (x < 0) x = -1;

        float y = owner.Velocity.y;
        if (y > 0) y = 1;
        else if (y < 0) y = -1;

        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_HORIZONTAL, x);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_VERTICAL, y);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_VELOCITY, owner.Velocity.magnitude);
    }
}

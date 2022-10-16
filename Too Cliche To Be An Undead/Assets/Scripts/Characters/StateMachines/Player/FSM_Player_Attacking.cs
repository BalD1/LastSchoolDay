using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Attacking : FSM_Base<FSM_Player_Manager>
{
    public PlayerCharacter owner;

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        Vector2 mouseDir = stateManager.Owner.Weapon.GetDirectionOfMouse();

        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_HORIZONTAL, mouseDir.x);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_VERTICAL, mouseDir.y);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_ATTACKING, true);
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
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        if (!owner.Weapon.isAttacking)
            stateManager.SwitchState(stateManager.idleState);
    }

    public void NextAttack(int currentAttackIndex)
    {
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_ATTACKINDEX, currentAttackIndex);
    }
}

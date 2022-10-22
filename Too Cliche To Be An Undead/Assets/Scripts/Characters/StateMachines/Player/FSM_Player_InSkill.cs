using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_InSkill : FSM_Base<FSM_Player_Manager>
{
    private PlayerCharacter owner;
    private float timer;

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        owner ??= stateManager.Owner;
        Vector2 mouseDir = stateManager.Owner.Weapon.GetDirectionOfMouse();

        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_HORIZONTAL, mouseDir.x);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_VERTICAL, mouseDir.y);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_INSKILL, true);
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        timer -= Time.deltaTime;

        if (owner.GetSkill.CanMove) owner.ReadMovementsInputs();
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
        if (owner.GetSkill.CanMove) owner.Movements();
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_INSKILL, false);
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        if (timer <= 0) stateManager.SwitchState(stateManager.idleState);
    }

    public FSM_Player_InSkill SetTimer(float _timer)
    {
        timer = _timer;
        return this;
    }
}

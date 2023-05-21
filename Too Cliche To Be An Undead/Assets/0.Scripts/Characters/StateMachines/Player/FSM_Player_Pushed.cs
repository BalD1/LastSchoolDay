    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Pushed : FSM_Entity_Pushed<FSM_Player_Manager>
{
    private PlayerCharacter playerOwner;

    private const string ONHIT_MODIFIER_ID = "PUSHED_";

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        base.EnterState(stateManager);

        playerOwner ??= owner as PlayerCharacter;

        playerOwner.D_attackInput += playerOwner.Weapon.AskForAttack;
        playerOwner.D_skillInput += playerOwner.GetSkillHolder.StartSkill;
        playerOwner.Weapon.AddOnHitEffect(new OnHitEffects
            (_owner: playerOwner,
            _id: ONHIT_MODIFIER_ID,
            _rangeModifier: 1,
            _damagesModifier: 1,
            _knockbackModifier: 1,
            _cameraShakeIntensityModifier: 1,
            null,
            -1,
            -1,
            true
            ));
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        base.UpdateState(stateManager);

        stateManager.OwnerWeapon.SetRotation();
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
        //playerOwner.Movements();
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        base.ExitState(stateManager);
        (owner as PlayerCharacter).ForceUpdateMovementsInput();

        playerOwner.D_attackInput -= playerOwner.Weapon.AskForAttack;
        playerOwner.D_skillInput -= playerOwner.GetSkillHolder.StartSkill;
        playerOwner.Weapon.RemoveOnHitEffect(ONHIT_MODIFIER_ID);
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(stateManager.idleState);
    }

    public override string ToString() => "Pushed";
}

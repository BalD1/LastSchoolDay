using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCallbacks : MonoBehaviour
{
    [SerializeField] private PlayerWeapon ownerWeapon;
    [SerializeField] private PlayerCharacter owner;

    public void StartAttack() => ownerWeapon.isAttacking = true;

    public void PerformAttack(int isLastAttack) => ownerWeapon.StartWeaponAttack(isLastAttack == 1 ? true : false);

    public void SetNextAttackIndex(int idx) => owner.StateManager.attackingState.NextAttack(idx);

    public void EndAnim(int allowNextAttack)
    {
        ownerWeapon.isAttacking = false;
        owner.StateManager.SwitchState(owner.StateManager.idleState);

        if (ownerWeapon.prepareNextAttack && allowNextAttack == 0)
        {
            ownerWeapon.prepareNextAttack = false;
            owner.StateManager.SwitchState(owner.StateManager.attackingState);
        }
        else
        {
            ownerWeapon.prepareNextAttack = false;
            owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_ATTACKING, false);
        }
        ownerWeapon.StartResetTimer();
    }
}

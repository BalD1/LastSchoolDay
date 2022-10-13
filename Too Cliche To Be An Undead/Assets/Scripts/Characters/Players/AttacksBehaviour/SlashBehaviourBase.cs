using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashBehaviourBase : StateMachineBehaviour
{
    [SerializeField] protected int attackIndex;
    protected PlayerWeapon weapon;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (weapon == null) weapon = animator.GetComponentInParent<PlayerWeapon>();
        weapon.isAttacking = true;

        weapon.DamageEnemiesInRange();
        // Play attack sound based on attack index
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        weapon.isAttacking = false;
        weapon.Owner.GetRb.velocity = Vector2.zero;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashBehaviourBase : StateMachineBehaviour
{
    [SerializeField] protected int attackIndex;
    [SerializeField] protected bool isLastAttack;
    protected PlayerWeapon weapon;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (weapon == null) weapon = animator.GetComponentInParent<PlayerWeapon>();
        weapon.isAttacking = true;

        weapon.slashParticles.Play();

        weapon.StartWeaponAttack(isLastAttack);
        // Play attack sound based on attack index
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        weapon.isAttacking = false;
        weapon.Owner.GetRb.velocity = Vector2.zero;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class TransitionBehaviourBase : StateMachineBehaviour
{
    [SerializeField] protected string animationToPlay;
    protected PlayerWeapon weapon;
    private bool switchToNextAttack;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (weapon == null) weapon = animator.GetComponentInParent<PlayerWeapon>();
        switchToNextAttack = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (weapon.prepareNextAttack || weapon.inputStored)
        {
            switchToNextAttack = true;

            if (animationToPlay != null && animationToPlay != "")
                animator.Play(animationToPlay);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        weapon.attackEnded = !switchToNextAttack;
        weapon.prepareNextAttack = false;
        weapon.inputStored = false;
    }
}

using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Boss_Dead : FSM_Base<FSM_Boss_Manager>
{
    BossZombie owner;

    private bool wasAttacking;

    public override void EnterState(FSM_Boss_Manager stateManager)
    {
        owner = stateManager.Owner;

        if (owner.IsAttacking)
        {
            wasAttacking = true;
            owner.D_currentAttackEnded += SetAnimation;
        }
        else SetAnimation();
    }

    private void SetAnimation()
    {
        AnimationReferenceAsset deathAnim = owner.animationData.DeathAnim;

        owner.animationController.SetAnimation(deathAnim, false);

        if (wasAttacking) owner.D_currentAttackEnded -= SetAnimation;
    }

    public override void UpdateState(FSM_Boss_Manager stateManager)
    {
    }

    public override void FixedUpdateState(FSM_Boss_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Boss_Manager stateManager)
    {
        wasAttacking = false;
    }

    public override void Conditions(FSM_Boss_Manager stateManager)
    {
    }

    public override string ToString()
    {
        return "Dead";
    }
}

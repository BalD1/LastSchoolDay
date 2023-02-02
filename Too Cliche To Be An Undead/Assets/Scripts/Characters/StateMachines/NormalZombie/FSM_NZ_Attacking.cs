using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_NZ_Attacking : FSM_Base<FSM_NZ_Manager>
{
    private NormalZombie owner;

    private bool attack_flag;

    private float waitBeforeAttack_TIMER;
    private float attack_TIMER;

    private enum E_AttackOrientation {  Up, Down, Left, Right }
    private E_AttackOrientation attackOrientation;

    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        owner ??= stateManager.Owner;

        if (owner.Attack.DamageOnCollision) owner.d_EnteredTrigger += OnTrigger;

        owner.StartMaterialFlash("_Attacking", .1f);

        TextPopup.Create("!", owner.transform).transform.localPosition += (Vector3)owner.GetHealthPopupOffset;

        float durationBeforeAttack = Random.Range(owner.MinDurationBeforeAttack, owner.MaxDurationBeforeAttack);

        GameObject renderer = owner.spineHolder;
        if (renderer == null) renderer = owner.GetSprite.gameObject;

        LeanTween.scale(renderer, owner.MaxScaleOnAttack, durationBeforeAttack / 2).setEase(owner.InType).setOnComplete(
            () =>
            {
                owner.SetAttackedPlayer(owner.CurrentPlayerTarget);
                LeanTween.scale(renderer, Vector2.one, durationBeforeAttack / 2).setEase(owner.OutType);
            });

        waitBeforeAttack_TIMER = durationBeforeAttack;
        attack_TIMER = owner.Attack_DURATION;
        attack_flag = false;

        SCRPT_EnemyAttack enemyAttack = owner.Attack;

        Vector2 dir = (owner.PivotOffset.transform.position - owner.CurrentPlayerTarget.PivotOffset.transform.position).normalized;
        float lookAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        lookAngle += enemyAttack.telegraphRotationOffset; 
        Quaternion telegraphRotation = Quaternion.AngleAxis(lookAngle, Vector3.forward);

        float animRot = telegraphRotation.eulerAngles.z;

        if (owner.animationController != null)
        {

            AnimationReferenceAsset attackAnimation;

            switch(animRot)
            {
                case float f when (f > 45 && f <= 135):
                    attackOrientation = E_AttackOrientation.Down;
                    attackAnimation = owner.animationData.attackAnticip_Down;
                    owner.animationController.FlipSkeleton(true);
                    break;

                case float f when (f > 135 && f <= 225):
                    attackOrientation = E_AttackOrientation.Right;
                    attackAnimation = owner.animationData.attackAnticip_Side;
                    owner.animationController.FlipSkeleton(true);
                    break;

                case float f when (f > 225 && f <= 315):
                    attackOrientation = E_AttackOrientation.Up;
                    attackAnimation = owner.animationData.attackAnticip_Up;
                    owner.animationController.FlipSkeleton(true);
                    break;

                default:
                    attackOrientation = E_AttackOrientation.Left;
                    attackAnimation = owner.animationData.attackAnticip_Side;
                    owner.animationController.FlipSkeleton(false);
                    break;
            }

            owner.animationController.SetAnimation(attackAnimation, false);
        }


        owner.AttackDirection = -dir;

        Vector2 telegraphSize = enemyAttack.telegraphVectorSize != Vector2.zero ? enemyAttack.telegraphVectorSize : new Vector2(enemyAttack.AttackDistance, enemyAttack.AttackDistance);

        owner.attackTelegraph.Setup(telegraphSize, owner.Attack.attackOffset, telegraphRotation, owner.Attack.telegraphSprite, durationBeforeAttack);

        owner.canBePushed = true;
    }

    public override void UpdateState(FSM_NZ_Manager stateManager)
    {
        if (waitBeforeAttack_TIMER > 0) waitBeforeAttack_TIMER -= Time.deltaTime;
        else if (waitBeforeAttack_TIMER <= 0 && !attack_flag)
        {
            owner.Attack.OnStart(owner);

            if (owner.animationController != null) 
            { 

                AnimationReferenceAsset attackAnimation;

                switch (attackOrientation)
                {
                    case E_AttackOrientation.Down:
                        attackAnimation = owner.animationData.attackAnim_down;
                    owner.animationController.FlipSkeleton(true);
                        break;

                    case E_AttackOrientation.Right:
                        attackAnimation = owner.animationData.attackAnim_side;
                        owner.animationController.FlipSkeleton(true);
                        break;

                    case E_AttackOrientation.Up:
                        attackAnimation = owner.animationData.attackAnim_up;
                        owner.animationController.FlipSkeleton(true);
                        break;

                    default:
                        attackAnimation = owner.animationData.attackAnim_side;
                        owner.animationController.FlipSkeleton(false);
                        break;
                }

                owner.animationController.SetAnimation(attackAnimation, false);
            }

            attack_flag = true;
        }

        if (attack_flag && attack_TIMER > 0) attack_TIMER -= Time.deltaTime;
    }

    public override void FixedUpdateState(FSM_NZ_Manager stateManager)
    {

    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
        owner.Attack.OnExit(owner);
        if (owner.Attack.DamageOnCollision) owner.d_EnteredTrigger -= OnTrigger;

        owner.UnsetAttackedPlayer();
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        if (attack_TIMER <= 0) stateManager.SwitchState(stateManager.chasingState);
    }

    private void OnTrigger(Collider2D collider)
    {
        if (!owner.attackStarted) return;
        if (collider == null) return;
        if (collider.transform.parent == null) return;

        PlayerCharacter p = collider.transform.parent.GetComponent<PlayerCharacter>();

        if (p == null) return;

        p.OnTakeDamages(owner.maxDamages_M, owner.RollCrit());
    }

    public override string ToString() => "Attacking";
}

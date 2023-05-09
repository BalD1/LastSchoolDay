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

    private const int upsideMinMargin = 270;
    private const int upsideMaxMargin = 315;

    private const int downsideMinMargin = 45;
    private const int downsideMaxMargin = 90;

    private const int leftAngleOrientation = 270;

    private int currentAttackIdx;

    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        owner ??= stateManager.Owner;

        owner.SetAttackedPlayer(owner.CurrentPlayerTarget);

        owner.enemiesBlocker.enabled = false;

        TextPopup.Create("!", owner.transform).transform.localPosition += (Vector3)owner.GetHealthPopupOffset;

        float durationBeforeAttack = Random.Range(owner.MinDurationBeforeAttack, owner.MaxDurationBeforeAttack);

        GameObject renderer = owner.spineHolder;

        LeanTween.scale(renderer, owner.MaxScaleOnAttack.Fluctuate(), durationBeforeAttack / 2).setEase(owner.InType).setOnComplete(
            () =>
            {
                LeanTween.scale(renderer, Vector2.one, durationBeforeAttack / 2).setEase(owner.OutType);
            });

        waitBeforeAttack_TIMER = durationBeforeAttack;
        attack_TIMER = owner.Attack_DURATION.Fluctuate();
        attack_flag = false;

        SCRPT_EnemyAttack[] enemyAttacksArray = owner.AttacksArray;

        currentAttackIdx = Random.Range(0, enemyAttacksArray.Length);

        SCRPT_EnemyAttack enemyAttack = enemyAttacksArray[currentAttackIdx];

        if (enemyAttack.DamageOnTrigger) owner.d_EnteredTrigger += OnTrigger;
        if (enemyAttack.DamageOnCollision) owner.d_EnteredCollider += OnCollision;

        Vector2 dir = (owner.PivotOffset.transform.position - owner.CurrentPlayerTarget.PivotOffset.transform.position).normalized;
        float lookAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        lookAngle += enemyAttack.telegraphRotationOffset; 
        Quaternion telegraphRotation = Quaternion.AngleAxis(lookAngle, Vector3.forward);

        float animRot = telegraphRotation.eulerAngles.z;

        if (owner.animationController != null)
        {
            owner.lookAtObject.LookAt(owner.CurrentTransformTarget, Vector3.forward);

            Vector2 orientation = owner.lookAtObject.eulerAngles;

            AnimationReferenceAsset attackAnimation;
            switch (orientation)
            {
                // upside
                case Vector2 v when upsideMinMargin < v.x && v.x < upsideMaxMargin:
                    attackOrientation = E_AttackOrientation.Up;
                    attackAnimation = owner.animationData.GetAttackAnimUp(currentAttackIdx);
                    owner.animationController.FlipSkeleton(true);
                    break;

                // downside
                case Vector2 v when downsideMinMargin < v.x && v.x < downsideMaxMargin:
                    attackOrientation = E_AttackOrientation.Down;
                    attackAnimation = owner.animationData.GetAttackAnimDown(currentAttackIdx);
                    owner.animationController.FlipSkeleton(true);
                    break;

                // left & right
                default:
                    // left
                    if (orientation.y == leftAngleOrientation)
                    {
                        attackOrientation = E_AttackOrientation.Left;
                        attackAnimation = owner.animationData.GetAttackAnimSide(currentAttackIdx);
                        owner.animationController.FlipSkeleton(false);
                    }
                    // right
                    else
                    {
                        attackOrientation = E_AttackOrientation.Right;
                        attackAnimation = owner.animationData.GetAttackAnimSide(currentAttackIdx);
                        owner.animationController.FlipSkeleton(true);
                    }
                    break;
            }

            owner.animationController.SetAnimation(attackAnimation, false);
        }

        owner.AttackDirection = -dir;

        Vector2 telegraphSize = enemyAttack.telegraphVectorSize != Vector2.zero ? enemyAttack.telegraphVectorSize : new Vector2(enemyAttack.AttackDistance, enemyAttack.AttackDistance);

        owner.attackTelegraph.Setup(telegraphSize, enemyAttack.attackOffset, telegraphRotation, enemyAttack.telegraphSprite, durationBeforeAttack);

        owner.canBePushed = true;
    }

    public override void UpdateState(FSM_NZ_Manager stateManager)
    {
        if (waitBeforeAttack_TIMER > 0) waitBeforeAttack_TIMER -= Time.deltaTime;
        else if (waitBeforeAttack_TIMER <= 0 && !attack_flag)
        {
            owner.AttacksArray[currentAttackIdx].OnStart(owner);
            owner.D_onAttack?.Invoke();

            if (owner.animationController != null) 
            { 

                AnimationReferenceAsset attackAnimation;

                switch (attackOrientation)
                {
                    case E_AttackOrientation.Down:
                        attackAnimation = owner.animationData.GetAnticipAttackAnimDown(currentAttackIdx);
                    owner.animationController.FlipSkeleton(true);
                        break;

                    case E_AttackOrientation.Right:
                        attackAnimation = owner.animationData.GetAnticipAttackAnimSide(currentAttackIdx);
                        owner.animationController.FlipSkeleton(true);
                        break;

                    case E_AttackOrientation.Up:
                        attackAnimation = owner.animationData.GetAnticipAttackAnimUp(currentAttackIdx);
                        owner.animationController.FlipSkeleton(true);
                        break;

                    default:
                        attackAnimation = owner.animationData.GetAnticipAttackAnimDown(currentAttackIdx);
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
        owner.AttacksArray[currentAttackIdx].OnExit(owner);
        if (owner.AttacksArray[currentAttackIdx].DamageOnTrigger) owner.d_EnteredTrigger -= OnTrigger;

        owner.UnsetAttackedPlayer();
        owner.Vision.TargetClosestPlayer();

        owner.enemiesBlocker.enabled = true;
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        if (attack_TIMER <= 0) stateManager.SwitchState(stateManager.chasingState);
    }

    private void OnCollision(Collision2D collision)
    {
        if (!owner.attackStarted) return;
        if (collision == null) return;

        if (collision.transform.parent == null) return;

        PlayerCharacter p = collision.gameObject.GetComponent<PlayerCharacter>();
        if (p == null) return;

        p.OnTakeDamages(owner.MaxDamages_M.Fluctuate(), owner, owner.RollCrit());
    }
    private void OnEntityCollision(Entity entity)
    {
        if (!owner.attackStarted) return;
        if (entity == null) return;

        entity.OnTakeDamages(owner.MaxDamages_M.Fluctuate(), owner, owner.RollCrit());
    }
    private void OnTrigger(Collider2D collider)
    {
        if (!owner.attackStarted) return;
        if (collider == null) return;
        if (collider.transform.parent == null) return;

        PlayerCharacter p = collider.transform.parent.GetComponent<PlayerCharacter>();

        if (p == null) return;

        p.OnTakeDamages(owner.MaxDamages_M.Fluctuate(), owner, owner.RollCrit());
    }

    public override string ToString() => "Attacking";
}

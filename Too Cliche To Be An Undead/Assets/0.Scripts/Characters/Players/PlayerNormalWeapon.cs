using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNormalWeapon : PlayerWeapon
{
    [SerializeField] private float maxRange = 2f;
    [SerializeField] private float lastAttackDamagesMultiplier = 1.5f;
    [SerializeField] private float lastAttackPushPercentage = .3f;

    [SerializeField] private float onAttackMovementForce = 1f;

    [SerializeField] private float normalShakeIntensity = 1f;
    [SerializeField] private float normalShakeDuration = .1f;

    [SerializeField] private float bigShakeIntensity = 2f;
    [SerializeField] private float bigShakeDuration = .2f;

    [SerializeField] private float distanceForAutoAim = 1f;

    [SerializeField] private GameObject effectObject;

    private Transform closestEnemy;
    private float closestEnemyDist = -1;
    private float speedModifier;

    protected override void Awake()
    {
        base.Awake();
        effectObject ??= this.transform.GetComponentInChildren<GameObject>();
    }

    public override void StartWeaponAttack(bool isLastAttack)
    {
        base.StartWeaponAttack(isLastAttack);

        owner.OnAttack?.Invoke(isLastAttack);

        float onAttackMovementSpeed = onAttackMovementForce;

        // If the player was moving, add its speed to the attack movements
        if (owner.PlayerMotor.Velocity != Vector2.zero) speedModifier = owner.MaxSpeed_M * .5f;
        else speedModifier = 0;

        onAttackMovementSpeed += speedModifier;
        
        Vector2 attackMovementDirection = owner.Weapon.GetPreciseDirectionOfMouseOrGamepad().normalized;

        owner.GetRb.AddForce(onAttackMovementSpeed * attackMovementDirection, ForceMode2D.Impulse);

        hitEntities = Physics2D.OverlapCircleAll(effectObject.transform.position, owner.MaxAttRange_M * rangeModifier_M, damageablesLayer);

        bool successfulhit = false;
        bool connectedEntity = false;
        bool hadBigHit = false;

        if (performHitStop)
        {
            owner.SkeletonAnimation.timeScale = 0;
            owner.StartTimeStop();
        }

        List<EnemyBase> hitEnemies = new List<EnemyBase>();

        foreach (var item in hitEntities)
        {
            var damageable = item.GetComponentInParent<IDamageable>();
            if (damageable == null) continue;

            Entity e = item.GetComponentInParent<Entity>();

            float damages = owner.MaxDamages_M * damagesModifier_M;

            bool performKnockback = true;
            if (isLastAttack)
            {
                damages *= lastAttackDamagesMultiplier;
                performKnockback = false;
            }

            bool isCrit = owner.RollCrit();

            // If the entity can't be damaged, continue to the next hit entity
            if (damageable.OnTakeDamages(damages, owner, isCrit) == false)
                continue;

            successfulhit = true;

            if (connectedEntity == false) connectedEntity = e != null;

            // Check if the entity is the closest one from player
            float dist = Vector2.Distance(item.transform.position, effectObject.transform.position);
            if ((closestEnemy == null || closestEnemyDist == -1) || (dist <= distanceForAutoAim && dist < closestEnemyDist))
            {
                closestEnemy = item.GetComponentInParent<EnemyBase>()?.PivotOffset;
                closestEnemyDist = dist;
            }

            float shakeIntensity = normalShakeIntensity * cameraShakeIntensityModifier_M;
            float shakeDuration = normalShakeDuration;

            hadBigHit = isLastAttack || isCrit;

            foreach (var effect in onHitEffects) if (effect.IsBigHit) hadBigHit = true;

            if (hadBigHit)
            {
                if (!performHitStop)
                    e?.AskPush(owner.PlayerDash.PushForce * lastAttackPushPercentage * knockbackModifier_M, owner, owner);
                shakeIntensity = bigShakeIntensity * cameraShakeIntensityModifier_M;
                shakeDuration = bigShakeDuration;
            }

            if (performHitStop && e != null)
            {
                e.Stun(1 + (hitStopTime), true);
                e.SkeletonAnimation.timeScale = 0;
            }

            EnemyBase enemy = e as EnemyBase;
            if (enemy != null) hitEnemies.Add(enemy);

            SuccessfulHit(item.transform.position, e, performKnockback, speedModifier, (hadBigHit));
            CameraManager.Instance.ShakeCamera(shakeIntensity, shakeDuration);
        }
        if (performHitStop)
        {
            LeanTween.delayedCall(hitStopTime, () =>
            {
                foreach (var item in hitEnemies)
                {
                    item.AskPush(owner.PlayerDash.PushForce * knockbackModifier_M, owner, owner);
                    item.SkeletonAnimation.timeScale = 1;
                }
                owner.SkeletonAnimation.timeScale = 1;
                owner.StopTimeStop();
            });
        }
        if (!successfulhit) owner.OnSwiff?.Invoke();
        if (connectedEntity) owner.OnSuccessfulAttack?.Invoke(hadBigHit);

        SetRotationTowardTarget(closestEnemy);
        closestEnemy = null;
        closestEnemyDist = -1;

        if (performHitStop)
        {
            performHitStop = false;
            if (inputStored)
            {
                inputStored = false;

                PlayerAnimationController ownerAnimsCtrl = owner.AnimationController;

                AskForAttack();
                switch (owner.Weapon.GetGeneralDirectionOfMouseOrGamepad())
                {
                    case Vector2 v when v == Vector2.up:
                        ownerAnimsCtrl.TryFlipSkeleton(false);
                        ownerAnimsCtrl.SetAnimation(owner.AnimationsData.AttackAnim_up, false);
                        break;

                    case Vector2 v when v == Vector2.down:
                        ownerAnimsCtrl.TryFlipSkeleton(true);
                        ownerAnimsCtrl.SetAnimation(owner.AnimationsData.AttackAnim_down, false);
                        break;

                    case Vector2 v when v == Vector2.left:
                        ownerAnimsCtrl.TryFlipSkeleton(false);
                        ownerAnimsCtrl.SetAnimation(owner.AnimationsData.AttackAnim_side, false);
                        break;

                    case Vector2 v when v == Vector2.right:
                        ownerAnimsCtrl.TryFlipSkeleton(true);
                        ownerAnimsCtrl.SetAnimation(owner.AnimationsData.AttackAnim_side, false);
                        break;
                }
            }
            else
            {
                LeanTween.delayedCall(attack_TIMER, () =>
                {
                    isAttacking = false;
                    SetAttackEnded(true);
                });
            }
        }
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!debugMode || owner == null) return;

        Gizmos.DrawWireSphere(effectObject.transform.position, owner.MaxAttRange_M);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(owner.transform.position, maxRange);
#endif
    }
}

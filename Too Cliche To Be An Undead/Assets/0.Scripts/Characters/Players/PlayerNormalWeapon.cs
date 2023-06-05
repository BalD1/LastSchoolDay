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

        owner.D_onAttack?.Invoke(isLastAttack);

        float onAttackMovementSpeed = onAttackMovementForce;

        // If the player was moving, add its speed to the attack movements
        if (owner.Velocity != Vector2.zero) speedModifier = owner.MaxSpeed_M * .5f;
        else speedModifier = 0;

        onAttackMovementSpeed += speedModifier;
        
        Vector2 attackMovementDirection = owner.Weapon.GetPreciseDirectionOfMouseOrGamepad().normalized;

        owner.GetRb.AddForce(onAttackMovementSpeed * attackMovementDirection, ForceMode2D.Impulse);

        hitEntities = Physics2D.OverlapCircleAll(effectObject.transform.position, owner.MaxAttRange_M * rangeModifier_M, damageablesLayer);

        bool successfulhit = false;
        bool connectedEntity = false;
        bool hadBigHit = false;

        foreach (var item in hitEntities)
        {
            var damageable = item.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
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
                if ((closestEnemy == null || closestEnemyDist == -1) || (dist <= distanceForAutoAim && dist < closestEnemyDist) )
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
                    e?.Push(owner.transform.position, owner.PlayerDash.PushForce * lastAttackPushPercentage * knockbackModifier_M, owner, owner);
                    shakeIntensity = bigShakeIntensity * cameraShakeIntensityModifier_M;
                    shakeDuration = bigShakeDuration;
                }

                SuccessfulHit(item.transform.position, e, performKnockback, speedModifier, (hadBigHit));
                CameraManager.Instance.ShakeCamera(shakeIntensity, shakeDuration);
            }
        }
        if (!successfulhit) owner.D_swif?.Invoke();
        if (connectedEntity) owner.D_successfulAttack?.Invoke(hadBigHit);

        if (isLastAttack) owner.StartGamepadShake(bigHitGamepadShake);
        else owner.StartGamepadShake(bigHitGamepadShake);

        SetRotationTowardTarget(closestEnemy);
        closestEnemy = null;
        closestEnemyDist = -1;
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

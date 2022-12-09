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

        float onAttackMovementSpeed = onAttackMovementForce;

        // If the player was moving, add its speed to the attack movements
        if (owner.Velocity != Vector2.zero) speedModifier = owner.maxSpeed_M;
        else speedModifier = 0;

        onAttackMovementSpeed += speedModifier;
        
        //Vector2 attackMovementDirection = (effectObject.transform.position - owner.transform.position).normalized;
        Vector2 attackMovementDirection = owner.Weapon.GetPreciseDirectionOfMouseOrGamepad().normalized;

        owner.GetRb.AddForce(onAttackMovementSpeed * attackMovementDirection, ForceMode2D.Impulse);

        hitEntities = Physics2D.OverlapCircleAll(effectObject.transform.position, owner.maxAttRange_M, damageablesLayer);
        foreach (var item in hitEntities)
        {
            var damageable = item.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                float damages = owner.maxDamages_M;

                bool performKnockback = true;
                if (isLastAttack)
                {
                    damages *= lastAttackDamagesMultiplier;
                    performKnockback = false;
                }

                bool isCrit = owner.RollCrit();

                // If the entity can't be damaged, continue to the next hit entity
                if (damageable.OnTakeDamages(damages, owner.GetStats.Team, isCrit) == false)
                    continue;

                // Check if the entity is the closest one from player
                float dist = Vector2.Distance(item.transform.position, effectObject.transform.position);
                if ((closestEnemy == null || closestEnemyDist == -1) || (dist <= distanceForAutoAim && dist < closestEnemyDist) )
                {
                    closestEnemy = item.GetComponentInParent<EnemyBase>().PivotOffset;
                    closestEnemyDist = dist;
                }
                    

                float shakeIntensity = normalShakeIntensity;
                float shakeDuration = normalShakeDuration;
                
                if (isLastAttack || isCrit)
                {
                    item.GetComponentInParent<Entity>().Push(owner.transform.position, owner.PlayerDash.PushForce * lastAttackPushPercentage, owner);
                    shakeIntensity = bigShakeIntensity;
                    shakeDuration = bigShakeDuration;
                }

                SuccessfulHit(item.transform.position, item.GetComponentInParent<Entity>(), performKnockback, speedModifier);
                CameraManager.Instance.ShakeCamera(shakeIntensity, shakeDuration);
            }
        }

        SetRotationTowardTarget(closestEnemy);
        closestEnemy = null;
        closestEnemyDist = -1;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!debugMode || owner == null) return;

        Gizmos.DrawWireSphere(effectObject.transform.position, owner.maxAttRange_M);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(owner.transform.position, maxRange);
#endif
    }
}

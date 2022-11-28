using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerBaseWeapon : PlayerWeapon
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


    protected override void Awake()
    {
        base.Awake();
        effectObject ??= this.transform.GetComponentInChildren<GameObject>();
    }

    public override void StartWeaponAttack(bool isLastAttack)
    {
        base.StartWeaponAttack(isLastAttack);

        owner.GetRb.AddForce(onAttackMovementForce * (effectObject.transform.position - owner.transform.position).normalized, ForceMode2D.Impulse);

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

                if (damageable.OnTakeDamages(damages, owner.GetStats.Team, isCrit) == false)
                    continue;

                float dist = Vector2.Distance(item.transform.position, effectObject.transform.position);
                if ((closestEnemy == null || closestEnemyDist == -1) || (dist <= distanceForAutoAim && dist < closestEnemyDist) )
                {
                    closestEnemy = item.GetComponentInParent<EnemyBase>().PivotOffset;
                    closestEnemyDist = dist;
                }
                    

                float shakeIntensity = normalShakeIntensity;
                float shakeDuration = normalShakeDuration;
                
                if (isLastAttack)
                {
                    item.GetComponentInParent<Entity>().Push(owner.transform.position, owner.PlayerDash.PushForce * lastAttackPushPercentage, owner);
                    shakeIntensity = bigShakeIntensity;
                    shakeDuration = bigShakeDuration;
                }
                if (isCrit)
                {
                    shakeIntensity = bigShakeIntensity;
                    shakeDuration = bigShakeDuration;
                }

                SuccessfulHit(item.transform.position, item.GetComponentInParent<Entity>(), performKnockback);
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

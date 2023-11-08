using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMotor : EntityMotor<AnimationControllerSingle>
{
    [field: SerializeField] public int MinDistanceForNormalSpeed { get; private set; }
    protected float speedMultiplierOnDistance = 1;

    [SerializeField] private bool slowdownOnApproach;
    [SerializeField] private float distanceBeforeCompleteStop;

    [SerializeField] private Collider2D enemiesBlocker;

    private BaseEnemyAI enemyAI;
    private bool allowSlowdown;

    protected override void Awake()
    {
        base.Awake();

        owner.HolderTryGetComponent(IComponentHolder.E_Component.EnemyAI, out enemyAI);
    }

    public virtual void SetSpeedOnDistanceFromTarget()
    {
        float distanceFromTarget = Vector2.Distance(this.transform.position, enemyAI.GetTargetPosition());
        if (distanceFromTarget < MinDistanceForNormalSpeed + Camera.main.orthographicSize)
        {
            speedMultiplierOnDistance = 1;
            enemiesBlocker.enabled = true;
            return;
        }

        enemiesBlocker.enabled = false;
        speedMultiplierOnDistance = distanceFromTarget;
    }
}

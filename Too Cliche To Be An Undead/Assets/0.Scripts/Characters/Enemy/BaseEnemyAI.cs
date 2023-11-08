using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyAI : MonoBehaviourEventsHandler
{
    [SerializeField] private EnemyBase owner;

    [field: SerializeField] public SO_EnemyAttack[] EnemyAttacks { get; private set; }

    [field: SerializeField] public ITargetable CurrentTarget { get; private set; }

    [field: SerializeField] public EnemyPathfinding Pathfinding { get; private set; }

    public float GetDistanceFromTarget()
        => Vector2.Distance(owner.transform.position, CurrentTarget.GetPosition());

    public Vector2 GetTargetPosition()
        => CurrentTarget == null ? Vector2.zero : CurrentTarget.GetPosition();

    protected override void EventsSubscriber()
    {
        if (owner.HolderTryGetComponent(IComponentHolder.E_Component.HealthSystem, out HealthSystem healthSystem) == IComponentHolder.E_Result.Success)
            healthSystem.OnDeath += OnOwnerDeath;
    }

    protected override void EventsUnSubscriber()
    {
        if (owner.HolderTryGetComponent(IComponentHolder.E_Component.HealthSystem, out HealthSystem healthSystem) == IComponentHolder.E_Result.Success)
            healthSystem.OnDeath -= OnOwnerDeath;
    }

    private void OnOwnerDeath()
    {

    }
}

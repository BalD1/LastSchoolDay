using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEntity : MonoBehaviourEventsHandler, INewDamageable
{
    [field: SerializeField] public float CurrentHP { get; private set; }

    public void Death()
    {
        throw new System.NotImplementedException();
    }

    public void Heal(float amount, Entity healer, bool isCrit)
    {
        throw new System.NotImplementedException();
    }

    public bool IsAlive()
    {
        throw new System.NotImplementedException();
    }

    public bool TakeDamages(float amount, Entity damager, bool isCrit)
    {
        throw new System.NotImplementedException();
    }

    protected override void EventsSubscriber()
    {
    }

    protected override void EventsUnSubscriber()
    {
    }
}

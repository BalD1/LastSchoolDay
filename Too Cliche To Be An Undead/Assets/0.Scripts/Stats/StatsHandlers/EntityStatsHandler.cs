using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityStatsHandler<EntityType> : StatsHandler
                                    where EntityType : Entity
{
    [field: SerializeField] public EntityType Owner { get; protected set; }

    protected override void EventsSubscriber()
    {
        base.EventsSubscriber();
        Owner.OnFullReset += base.RemoveAllModifiers;
    }

    protected override void EventsUnSubscriber()
    {
        base.EventsUnSubscriber();
        Owner.OnFullReset -= base.RemoveAllModifiers;
    }
}

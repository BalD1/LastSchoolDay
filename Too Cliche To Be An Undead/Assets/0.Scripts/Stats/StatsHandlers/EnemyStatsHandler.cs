using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatsHandler : EntityStatsHandler<EnemyBase>
{
    protected override void EventsSubscriber()
    {
        base.EventsSubscriber();
        ZombiesScalingManagerEvents.OnSendModifiers += OnScalingModifiers;
    }

    protected override void EventsUnSubscriber()
    {
        base.EventsUnSubscriber();
        ZombiesScalingManagerEvents.OnSendModifiers -= OnScalingModifiers;
    }

    private void OnScalingModifiers(SO_StatModifierData[] modifiers)
    {
        foreach (var item in modifiers)
        {
            base.TryAddModifier(item, out var result);
        }
    }
}

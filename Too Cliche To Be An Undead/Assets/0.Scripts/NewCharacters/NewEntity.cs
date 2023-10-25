using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class NewEntity : MonoBehaviourEventsHandler
{
    [field: SerializeField] public StatsHandler EntityStats;
}

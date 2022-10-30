using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrainingDummy_NoText : EnemyBase
{
    [Header("Single")]
    [Header("Components")]

#if UNITY_EDITOR
    [ReadOnly][SerializeField] private string state = "N/A";
#endif


    public override void OnDeath(bool forceDeath = false)
    {
        this.OnHeal(this.GetStats.MaxHP(StatsModifiers));
    }
}

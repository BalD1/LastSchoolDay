using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrainingDummy_NoText : EnemyBase
{
    public override void OnDeath(bool forceDeath = false)
    {
        this.OnHeal(this.MaxHP_M);
    }
}

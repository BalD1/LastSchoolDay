using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombie : EnemyBase
{
    public override void OnDeath(bool forceDeath = false)
    {
        base.OnDeath(forceDeath);
        Destroy(this.gameObject);
    }
}

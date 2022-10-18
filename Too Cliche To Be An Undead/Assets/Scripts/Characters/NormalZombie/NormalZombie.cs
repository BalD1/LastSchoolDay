using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombie : EnemyBase
{

    protected override void Start()
    {
        base.Start();
        Pathfinding.StartUpdatePath();
    }

    protected override void Update()
    {
        base.Update();
        Debug.Log(this.GetRb.velocity);
    }

    public override void OnDeath(bool forceDeath = false)
    {
        base.OnDeath(forceDeath);
        Destroy(this.gameObject);
    }
}

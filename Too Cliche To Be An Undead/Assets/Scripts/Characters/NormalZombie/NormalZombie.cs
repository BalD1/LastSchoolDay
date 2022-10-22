using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombie : EnemyBase
{
    [SerializeField] private FSM_NZ_Manager stateManager;

    protected override void Start()
    {
        base.Start();
        Pathfinding.StartUpdatePath();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void OnDeath(bool forceDeath = false)
    {
        base.OnDeath(forceDeath);
        Destroy(this.gameObject);
    }

    public override Vector2 Push(Vector2 pusherPosition, float pusherForce)
    {
        if (stateManager.ToString().Equals("Pushed")) return Vector2.zero;

        Vector2 v = base.Push(pusherPosition, pusherForce);
        stateManager.SwitchState(stateManager.pushedState.SetForce(v));

        return v;
    }
}

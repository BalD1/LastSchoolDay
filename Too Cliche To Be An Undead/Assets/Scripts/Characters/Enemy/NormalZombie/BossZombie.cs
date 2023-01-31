using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZombie : EnemyBase
{
    protected override void Start()
    {
        base.Start();
        Pathfinding?.StartUpdatePath();
    }

    protected override void Update()
    {
        base.Update();
    }
    public override void OnDeath(bool forceDeath = false)
    {
        base.OnDeath(forceDeath);
    }

    public override void Stun(float duration, bool resetAttackTimer = false)
    {
        //stateManager.SwitchState(stateManager.stunnedState.SetDuration(duration, resetAttackTimer));
        this.attackTelegraph.CancelTelegraph();
    }

    public override void SetAttackedPlayer(PlayerCharacter target)
    {
        base.SetAttackedPlayer(target);

        Vector2 targetPosition = this.CurrentPlayerTarget == null ? this.storedTargetPosition : this.CurrentPlayerTarget.transform.position;
        //AttackDirection = (targetPosition - (Vector2)this.transform.position).normalized;
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, Attack.AttackDistance);
    }

}

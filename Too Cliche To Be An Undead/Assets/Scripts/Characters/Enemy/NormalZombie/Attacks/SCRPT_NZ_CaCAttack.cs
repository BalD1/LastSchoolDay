using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Scriptable/Entity/Enemy/CaC Attack NZ")]
public class SCRPT_NZ_CaCAttack : SCRPT_EnemyAttack
{
    public override void OnStart(EnemyBase owner)
    {
        (owner as NormalZombie).attackStarted = true;

        Collider2D[] hitEntities = Physics2D.OverlapCircleAll((Vector2)owner.attackTelegraph.transform.position, AttackDistance, entitiesToAffect);

        foreach (var item in hitEntities)
        {
            Entity e = item.GetComponentInParent<Entity>();

            if (e == null) continue;

            e.OnTakeDamages(owner.maxDamages_M, owner.RollCrit());
        }
    }

    public override void OnUpdate(EnemyBase owner)
    {
    }

    public override void OnExit(EnemyBase owner)
    {
        owner.GetRb.velocity = Vector2.zero;
        owner.StartAttackTimer(0, true);
        (owner as NormalZombie).attackStarted = false;
    }
}

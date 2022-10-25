using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Scriptable/Entity/Enemy/Attack NZ")]
public class SCRPT_NZ_Attack : SCRPT_EnemyAttack
{
    [SerializeField] private float attackForce;

    public override void OnStart(EnemyBase owner)
    {
        Vector2 target = owner.CurrentPlayerTarget == null ? owner.storedTargetPosition : owner.CurrentPlayerTarget.transform.position;
        Vector2 dir = (target - (Vector2)owner.transform.position).normalized;
        owner.GetRb.AddForce(dir * attackForce, ForceMode2D.Impulse);
    }

    public override void OnUpdate(EnemyBase owner)
    {
    }

    public override void OnExit(EnemyBase owner)
    {
        owner.GetRb.velocity = Vector2.zero;
        owner.StartAttackTimer();
    }
}

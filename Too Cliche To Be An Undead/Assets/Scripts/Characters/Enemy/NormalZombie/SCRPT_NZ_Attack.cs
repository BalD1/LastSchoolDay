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
        Vector2 dir = (owner as NormalZombie).AttackDirection;
        owner.GetRb.AddForce(dir * attackForce, ForceMode2D.Impulse);
        (owner as NormalZombie).attackStarted = true;
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

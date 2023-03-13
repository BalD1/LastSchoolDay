using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Scriptable/Entity/Enemy/Dash Attack NZ")]
public class SCRPT_NZ_DashAttack : SCRPT_EnemyAttack
{
    [SerializeField] private float attackForce;

    public override void OnStart(EnemyBase owner)
    {
        NormalZombie _owner = owner as NormalZombie;

        owner.enemiesBlocker.enabled = false;
        Vector2 dir = _owner.AttackDirection;

        owner.GetRb.AddForce(dir * attackForce, ForceMode2D.Impulse);

        _owner.attackStarted = true;

        _owner.attackTrigger.enabled = false;
        _owner.attackTrigger.enabled = true;
    }

    public override void OnUpdate(EnemyBase owner)
    {
    }

    public override void OnExit(EnemyBase owner)
    {
        owner.enemiesBlocker.enabled = true;
        owner.GetRb.velocity = Vector2.zero;
        owner.StartAttackTimer(0, true);
        (owner as NormalZombie).attackStarted = false;
    }
}

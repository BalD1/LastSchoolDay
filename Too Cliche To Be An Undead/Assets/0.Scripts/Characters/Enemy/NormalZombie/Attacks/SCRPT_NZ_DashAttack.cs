using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Scriptable/Entity/Enemy/Dash Attack NZ")]
public class SCRPT_NZ_DashAttack : SO_EnemyAttack
{
    [SerializeField] private float attackForce;

    public override void OnStart(EnemyBase owner)
    {
        BaseZombie _owner = owner as BaseZombie;

        owner.enemiesBlocker.enabled = false;
        //Vector2 dir = _owner.AttackDirection;

        //_owner.attackTrigger.enabled = false;
        //_owner.attackTrigger.enabled = true;

        //owner.GetRb.AddForce(dir * attackForce.Fluctuate(), ForceMode2D.Impulse);

        //_owner.attackStarted = true;
    }

    public override void OnUpdate(EnemyBase owner)
    {
    }

    public override void OnExit(EnemyBase owner)
    {
        //owner.enemiesBlocker.enabled = true;
        //owner.GetRb.velocity = Vector2.zero;
        //owner.StartAttackTimer(0, true);
        //(owner as BaseZombie).attackStarted = false;
    }
}

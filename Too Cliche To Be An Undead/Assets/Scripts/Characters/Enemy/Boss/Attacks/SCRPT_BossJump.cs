using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossJump", menuName = "Scriptable/Entity/Enemy/Boss/Jump Attack")]
public class SCRPT_BossJump : SCRPT_EnemyAttack
{
    private BossZombie boss;

    public override void OnStart(EnemyBase owner)
    {
        boss = owner as BossZombie;
    }

    public override void OnUpdate(EnemyBase owner)
    {
    }

    public override void OnExit(EnemyBase owner)
    {
        owner.GetRb.velocity = Vector2.zero;
        owner.StartAttackTimer(0, true);
        boss.attackStarted = false;
    }
}
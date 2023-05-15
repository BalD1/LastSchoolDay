using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "BossMinionsSpawn", menuName = "Scriptable/Entity/Enemy/Boss/Minions Spawn")]
public class SCRPT_BossMinionsSpawn : SCRPT_EnemyAttack
{
    [field: SerializeField] public AnimationReferenceAsset AttackAnim;

    [SerializeField] private IntMinMax amountToSpawn;

    private BossZombie boss;

    public override void OnStart(EnemyBase owner)
    {
        boss = owner as BossZombie;
        boss.attackStarted = true;

        boss.animationController.SetAnimation(AttackAnim, false);
        boss.animationController.AddAnimation(boss.animationData.IdleAnim, true, AttackAnim.Animation.Duration, 1);

        int spawnAmount = amountToSpawn.Random();
        for (int i = 0; i < spawnAmount; i++)
        {
            NormalZombie.Create(owner.transform.position, true, false);
        }
    }

    public override void OnUpdate(EnemyBase owner)
    {
    }

    public override void OnExit(EnemyBase owner)
    {
    }
}

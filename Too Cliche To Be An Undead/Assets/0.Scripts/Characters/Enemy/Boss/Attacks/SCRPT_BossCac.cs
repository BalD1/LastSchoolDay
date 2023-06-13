using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "BossCac", menuName = "Scriptable/Entity/Enemy/Boss/Cac Attack")]
public class SCRPT_BossCac : SCRPT_EnemyAttack
{
    [field: SerializeField] public AnimationReferenceAsset AttackAnim;

    [SerializeField] private SCRPT_EntityAudio.S_AudioClips audioClip;

    private BossZombie boss;

    public override void OnStart(EnemyBase owner)
    {
        boss = owner as BossZombie;
        boss.attackStarted = true;

        boss.animationController.SetAnimation(AttackAnim, false);
        boss.animationController.AddAnimation(boss.animationData.IdleAnim, true);

        Collider2D[] hitEntities = Physics2D.OverlapCircleAll((Vector2)owner.attackTelegraph.transform.position, AttackDistance, entitiesToAffect);

        foreach (var item in hitEntities)
        {
            Entity e = item.GetComponentInParent<Entity>();

            if (e == null) continue;

            boss.OnHitEntity(e, false);
        }

        owner.GetAudioSource.pitch = Random.Range(1 - audioClip.pitchRange, 1 + audioClip.pitchRange);
        owner.GetAudioSource.PlayOneShot(audioClip.clip);
        owner.GetAudioSource.pitch = 1;

        if (hitEntities.Length > 0) boss.HitStop(boss.HitStop_DURATION);
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
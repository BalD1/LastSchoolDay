using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "BossMinionsSpawn", menuName = "Scriptable/Entity/Enemy/Boss/Minions Spawn")]
public class SCRPT_BossMinionsSpawn : SO_EnemyAttack
{
    [SerializeField] private AnimationControllerBase.S_AnimationSequenceSingle[] animatioSequence;

    [SerializeField] private IntMinMax amountToSpawn;

    [SerializeField] private SCRPT_EntityAudio.S_AudioClips audioClip;

    private BossZombie boss;

    public override void OnStart(EnemyBase owner)
    {
        boss = owner as BossZombie;
        boss.attackStarted = true;

        boss.AnimationController.SetAnimationSequence(animatioSequence, true);
        //boss.animationController.SetAnimation(AttackAnim, false);
        //boss.animationController.AddAnimation(boss.animationController.AnimationData.IdleAnim, true, AttackAnim.Animation.Duration, 1);

        //owner.GetAudioSource.pitch = Random.Range(1 - audioClip.pitchRange, 1 + audioClip.pitchRange);
        //owner.GetAudioSource.PlayOneShot(audioClip.clip);

        int spawnAmount = amountToSpawn.Random();
        for (int i = 0; i < spawnAmount; i++)
        {
            //BaseZombie minion = BaseZombie.Create(owner.transform.position, true, false);
            //boss.OnMinionSpawned(minion);
            //minion.D_onDeathOf += boss.OnMinionDied;
        }
    }

    public override void OnUpdate(EnemyBase owner)
    {
    }

    public override void OnExit(EnemyBase owner)
    {
    }
}

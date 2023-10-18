using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossDash", menuName = "Scriptable/Entity/Enemy/Boss/Dash Attack")]
public class SCRPT_BossDash : SCRPT_EnemyAttack
{
    [SerializeField] private AnimationReferenceAsset upAnim;
    [SerializeField] private AnimationReferenceAsset downAnim;
    [SerializeField] private AnimationReferenceAsset sideAnim;

    [SerializeField] private SCRPT_EntityAudio.S_AudioClips audioClip;

    [SerializeField] private float attackForce;
    private BossZombie boss;

    public override void OnStart(EnemyBase owner)
    {
        boss = (owner as BossZombie);
        Vector2 dir = boss.AttackDirection;

        if (-5f < dir.x && dir.x < .5f)
        {
            if (dir.y > 0) boss.animationController.SetAnimation(upAnim,false);
            else boss.animationController.SetAnimation(downAnim,false);
        }
        else
        {
            if (dir.x < 0) boss.animationController.TryFlipSkeleton(false);
            boss.animationController.SetAnimation(sideAnim,false);
        }

        owner.GetAudioSource.pitch = Random.Range(1 - audioClip.pitchRange, 1 + audioClip.pitchRange);
        owner.GetAudioSource.PlayOneShot(audioClip.clip);
        owner.GetAudioSource.pitch = 1;

        owner.GetRb.AddForce(dir * attackForce, ForceMode2D.Impulse);
        boss.attackStarted = true;
    }

    public override void OnUpdate(EnemyBase owner)
    {
    }

    public override void OnExit(EnemyBase owner)
    {
        owner.enemiesBlocker.enabled = true;
        owner.GetRb.velocity = Vector2.zero;
        owner.StartAttackTimer(0, true);
        if (boss != null)
        boss.attackStarted = false;
    }
}
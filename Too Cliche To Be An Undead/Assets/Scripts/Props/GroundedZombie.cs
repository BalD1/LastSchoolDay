using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedZombie : Entity
{
    [SerializeField] private float stunDuration = 1.0f;

    [SerializeField] private SCRPT_ZombieAudio audioData;

    [SerializeField][SpineAnimation] private string idleAnim;
    [SerializeField][SpineAnimation] private string attackAnim;

    protected override void Awake()
    {
        base.Awake();

        this.D_onTakeDamagesFromEntity += (bool crit, Entity damager) => PlayAudio(audioData.GetRandomHurtClip());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        return;
        if (attack_TIMER > 0) return;

        if (collision.transform.parent == null) return;

        PlayerCharacter player = collision.GetComponentInParent<PlayerCharacter>();

        if (player == null) return;
        if (player.StateManager.ToString() == "Pushed") return;

        SkeletonAnimation.AnimationState.SetAnimation(0, attackAnim, false);
        SkeletonAnimation.AnimationState.AddAnimation(0, idleAnim, true, .25f);

        PlayAudio(audioData.GetRandomAttackClip());

        player.Stun(stunDuration, false, true);
        player.OnTakeDamages(MaxDamages_M, this, RollCrit());

        attack_TIMER = MaxAttCD_M;
    }

    private void PlayAudio(SCRPT_EntityAudio.S_AudioClips audio)
    {
        source.pitch = Random.Range(1 - audio.pitchRange, 1 + audio.pitchRange);
        source.PlayOneShot(audio.clip);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (attack_TIMER > 0) return;

        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();

        if (player == null) return;
        if (player.StateManager.ToString() == "Pushed") return;

        SkeletonAnimation.AnimationState.SetAnimation(0, attackAnim, false);
        SkeletonAnimation.AnimationState.AddAnimation(0, idleAnim, true, .25f);

        player.Stun(stunDuration, false, true);
        player.OnTakeDamages(MaxDamages_M, this, RollCrit());

        PlayAudio(audioData.GetRandomAttackClip());

        attack_TIMER = MaxAttCD_M;
    }

    public override void OnDeath(bool forceDeath = false)
    {
        base.OnDeath(forceDeath);

        AudioclipPlayer.Create(this.transform.position, audioData.GetRandomDeathClip());

        Destroy(this.gameObject);
    }
}

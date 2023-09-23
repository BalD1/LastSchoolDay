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

    public static GroundedZombie Create(Vector2 pos, bool reverse = false)
    {
        GroundedZombie res = Instantiate(GameAssets.Instance.GroundedZombiesPF.RandomElement(), pos, Quaternion.identity).GetComponent<GroundedZombie>();

        if (reverse)
        {
            Vector2 scale = res.skeletonAnimation.transform.localScale;
            scale.x = -1;
            res.skeletonAnimation.transform.localScale = scale;
        }

        return res;
    }

    protected override void EventsSubscriber()
    {
        base.EventsSubscriber();
        GymnasiumCinematicEvents.OnGymnasiumCinematicStarted += ForceKill;

    }

    protected override void EventsUnSubscriber()
    {
        base.EventsUnSubscriber();
        GymnasiumCinematicEvents.OnGymnasiumCinematicStarted -= ForceKill;
    }

    protected override void Awake()
    {
        base.Awake();

        this.OnTakeDamageFromEntity += (bool crit, Entity damager, bool tickDamages) => PlayAudio(audioData.GetRandomHurtClip());
    }

    private void PlayAudio(SCRPT_EntityAudio.S_AudioClips audio)
    {
        source.pitch = Random.Range(1 - audio.pitchRange, 1 + audio.pitchRange);
        source.PlayOneShot(audio.clip);
    }

    private void OnTriggerEnter2D(Collider2D collision)
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

    public void ForceKill() => Death(true);
    public override void Death(bool forceDeath = false)
    {
        base.Death(forceDeath);

        AudioclipPlayer.Create(this.transform.position, audioData.GetRandomDeathClip());
        BloodParticles.GetNext(this.transform.position);

        Destroy(this.gameObject);
    }
}

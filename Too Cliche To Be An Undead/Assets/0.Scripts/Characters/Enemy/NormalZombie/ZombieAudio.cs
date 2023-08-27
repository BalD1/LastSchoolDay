using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAudio : MonoBehaviour
{
    [SerializeField] private NormalZombie owner;
    private AudioSource ownerSource;

    [SerializeField] private float delayBetweenHurtAudio_COOLDOWN = .2f;
    private float delayBetweenHurtAudio_TIMER;

    [SerializeField] private ParticleSystem.MinMaxCurve miscAudioPlayByChasingZombies_COOLDOWN;
    private float miscAudioPlay_TIMER;
    private bool isChasing = false;

    private bool playedHurtAudio = false;
    private bool isDying = false;

    public static int currentZombiesHurtCount = 0;
    public const int maxZombiesHurtCount = 5;

    private void Reset()
    {
        owner = this.GetComponentInParent<NormalZombie>();
    }

    private void Start()
    {
        delayBetweenHurtAudio_TIMER = 0;
        ownerSource = owner.GetAudioSource;

        owner.D_onHurt += PlayHurtSound;
        owner.OnDeath += PlayDeathSound;
        owner.D_onAttack += PlayAttackSound;
        owner.D_onRespawn += ResetAudio;
        owner.OnStartChasing += OnStartChasing;
    }

    private void ResetAudio()
    {
        isDying = false;
        playedHurtAudio = false;
        delayBetweenHurtAudio_TIMER = 0;
    }

    private void Update()
    {
        if (delayBetweenHurtAudio_TIMER > 0)
        {
            delayBetweenHurtAudio_TIMER -= Time.deltaTime;
            if (delayBetweenHurtAudio_TIMER <= 0)
            {
                playedHurtAudio = false;
                currentZombiesHurtCount--;
            }
        }

        if (miscAudioPlay_TIMER > 0 && isChasing)
        {
            miscAudioPlay_TIMER -= Time.deltaTime;
            if (miscAudioPlay_TIMER <= 0)
            {
                miscAudioPlay_TIMER = GetMiscAudioCooldown();
                PlayAudioWithPitch(owner.AudioData.GetRandomMiscAudio());
            }
        }
    }

    private void OnDestroy()
    {
        owner.D_onHurt -= PlayHurtSound; ;
        owner.OnDeath -= PlayDeathSound;
        owner.D_onAttack -= PlayAttackSound;
        owner.D_onRespawn -= ResetAudio;
        owner.OnStartChasing -= OnStartChasing;
    }

    private float GetMiscAudioCooldown()
    {
        return miscAudioPlayByChasingZombies_COOLDOWN.Evaluate(NormalZombie.CurrentlyChasingZombiesCOunt, Random.value);
    }

    private void OnStartChasing(PlayerCharacter target)
    {
        PlayAudioWithPitch(owner.AudioData.GetRandomMiscAudio());
        isChasing = true;
        miscAudioPlay_TIMER = GetMiscAudioCooldown();
    }

    private void PlayHurtSound()
    {
        if (playedHurtAudio || isDying || currentZombiesHurtCount >= maxZombiesHurtCount) return;

        delayBetweenHurtAudio_TIMER = delayBetweenHurtAudio_COOLDOWN;
        playedHurtAudio = true;

        currentZombiesHurtCount++;

        PlayAudioWithPitch(owner.AudioData.GetRandomHurtClip());
    }
    private void PlayDeathSound()
    {
        isDying = true;
        isChasing = false;

        if (playedHurtAudio)
        {
            currentZombiesHurtCount--;
            delayBetweenHurtAudio_TIMER = 0;
            playedHurtAudio = false;
        }

        AudioclipPlayer.Create(this.transform.position, owner.AudioData.GetRandomDeathClip());
    }
    private void PlayAttackSound() => PlayAudioWithPitch(owner.AudioData.GetRandomAttackClip());
    private void PlayMiscSound() => PlayAudioWithPitch(owner.AudioData.GetRandomMiscAudio());

    private void PlayAudioWithPitch(AudioClip clip, float pitchRange)
    {
        ownerSource.pitch = Random.Range(1 - pitchRange, 1 + pitchRange);
        ownerSource.PlayOneShot(clip);
    }
    private void PlayAudioWithPitch(SCRPT_PlayerAudio.S_AudioClips clipData)
    {
        ownerSource.pitch = Random.Range(1 - clipData.pitchRange, 1 + clipData.pitchRange);
        ownerSource.PlayOneShot(clipData.clip);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAudio : MonoBehaviour
{
    [SerializeField] private NormalZombie owner;
    private AudioSource ownerSource;

    private void Reset()
    {
        owner = this.GetComponentInParent<NormalZombie>();
    }

    private void Start()
    {
        ownerSource = owner.GetAudioSource;

        owner.D_onHurt += PlayHurtSound; ;
        owner.d_OnDeath += PlayDeathSound;
        owner.D_onAttack += PlayAttackSound;
    }

    private void OnDestroy()
    {
        owner.D_onHurt -= PlayHurtSound; ;
        owner.d_OnDeath -= PlayDeathSound;
        owner.D_onAttack -= PlayAttackSound;
    }

    private void PlayHurtSound() => PlayAudioWithPitch(owner.AudioData.GetRandomHurtClip());
    private void PlayDeathSound() => PlayAudioWithPitch(owner.AudioData.GetRandomDeathClip());
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

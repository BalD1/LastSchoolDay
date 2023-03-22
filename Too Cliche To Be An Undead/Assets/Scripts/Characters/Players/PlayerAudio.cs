using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] protected PlayerCharacter owner;

    protected AudioSource ownerSource;
    protected SCRPT_PlayerAudio ownerAudioClips;

    protected virtual void Awake()
    {
        ownerSource = owner.GetAudioSource;

        SetAudioClips();
        owner.D_switchCharacter += SetAudioClips;

        owner.D_onTakeDamagesFromEntity += PlayHurtAudio;

        owner.d_OnDeath += PlayDeathAudio;

        owner.D_successfulAttack += PlayAttackAudio;
        owner.D_swif += PlaySwifAudio;

        owner.D_startSkill += PlaySkillStartAudio;
        owner.D_endSkill += PlaySkillEndAudio;

        owner.D_onDash += PlayDashAudio;
    }

    private void SetAudioClips() => ownerAudioClips = owner.GetAudioClips;

    private void PlayHurtAudio(bool isCrit)
    {
        ownerSource.PlayOneShot(ownerAudioClips.GetRandomHurtClip());
    }
    private void PlayHurtAudio(bool isCrit, Entity damager) => PlayHurtAudio(isCrit);

    private void PlayDeathAudio() => ownerSource.PlayOneShot(ownerAudioClips.GetRandomDeathClip());

    private void PlaySwifAudio() => ownerSource.PlayOneShot(ownerAudioClips.GetRandomSwifClip());

    private void PlayAttackAudio(bool isBigHit)
    {
        if (!isBigHit) ownerSource.PlayOneShot(ownerAudioClips.GetRandomAttackClip());
        else ownerSource.PlayOneShot(ownerAudioClips.GetRandomBigAttackClip());
    }

    private void PlaySkillStartAudio(bool holdAudio)
    {
        AudioClip startClip = ownerAudioClips.GetRandomSkillStartClip();
        if (startClip != null)
            ownerSource.PlayOneShot(startClip);

        if (holdAudio == false) return;

        ownerSource.loop = true;
        ownerSource.clip = ownerAudioClips.GetRandomSkillHoldClip();

        if (startClip != null)
        {
            double clipDuration = startClip.length;
            ownerSource.PlayScheduled(clipDuration);
        }
        else ownerSource.Play();
    }

    private void PlaySkillEndAudio(bool holdAudio)
    {
        if (holdAudio)
        {
            ownerSource.Stop();
            ownerSource.loop = false;
            ownerSource.clip = null;
        }

        ownerSource.PlayOneShot(ownerAudioClips.GetRandomSkillEndClip());
    }

    private void PlayDashAudio() => ownerSource.PlayOneShot(ownerAudioClips.GetRandomDashClip());
}

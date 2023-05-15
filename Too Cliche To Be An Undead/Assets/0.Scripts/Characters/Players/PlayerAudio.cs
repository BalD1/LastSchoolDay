using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] protected PlayerCharacter owner;

    [SerializeField] private AudioSource skillSource;

    protected AudioSource ownerSource;
    protected SCRPT_PlayerAudio ownerAudioClips;

    private LTDescr onSkillAudioLoopPoint;

    protected virtual void Awake()
    {
        ownerSource = owner.GetAudioSource;

        SetAudioClips();
        owner.D_switchCharacter += SetAudioClips;

        owner.D_onTakeDamagesFromEntity += PlayHurtAudio;

        owner.d_OnDeath += PlayDeathAudio;

        owner.D_onAttack += PlayAttackAudio;
        owner.D_successfulAttack += PlayAttackConnectedAudio;

        owner.D_earlySkillStart += PlayVoiceSkillAudio;
        owner.D_startSkill += PlaySkillStartAudio;
        owner.D_endSkill += PlaySkillEndAudio;

        owner.D_onDash += PlayDashAudio;

        owner.D_onFootPrint += PlayFootPrintAudio;
    }

    private void OnDestroy()
    {
        owner.D_switchCharacter -= SetAudioClips;

        owner.D_onTakeDamagesFromEntity -= PlayHurtAudio;

        owner.d_OnDeath -= PlayDeathAudio;

        owner.D_onAttack -= PlayAttackAudio;
        owner.D_successfulAttack -= PlayAttackConnectedAudio;

        owner.D_startSkill -= PlaySkillStartAudio;
        owner.D_endSkill -= PlaySkillEndAudio;

        owner.D_onDash -= PlayDashAudio;

        owner.D_onFootPrint -= PlayFootPrintAudio;
    }

    private void SetAudioClips() => ownerAudioClips = owner.GetAudioClips;

    private void PlayHurtAudio(bool isCrit)
    {
        PlayAudioWithPitch(ownerAudioClips.GetRandomHurtClip());
    }
    private void PlayHurtAudio(bool isCrit, Entity damager, bool tickDamages = false) => PlayHurtAudio(isCrit);

    private void PlayDeathAudio() => PlayAudioWithPitch(ownerAudioClips.GetRandomDeathClip());

    private void PlayAttackAudio(bool isBigHit)
    {
        if (!isBigHit)
        {
            PlayAudioWithPitch(ownerAudioClips.GetRandomAttackClip());
            PlayAudioWithPitch(ownerAudioClips.GetRandomVoiceAttackClip());
        }
        else
        {
            PlayAudioWithPitch(ownerAudioClips.GetRandomBigAttackClip());
            PlayAudioWithPitch(ownerAudioClips.GetRandomVoiceAttackClip());
        }
    }

    private void PlayAttackConnectedAudio(bool isBigHit)
    {
        if (!isBigHit) PlayAudioWithPitch(ownerAudioClips.GetRandomAttackConnectedClip());
        else PlayAudioWithPitch(ownerAudioClips.GetRandomBigAttackConnectedClip());
    }

    private void PlayVoiceSkillAudio()
    {
        PlayAudioWithPitch(ownerAudioClips.GetRandomVoiceSkillStartClip());
    }

    private void PlaySkillStartAudio(bool holdAudio)
    {
        // Play the start clip audio if exists
        AudioClip startClip = ownerAudioClips.GetRandomSkillStartClip().clip;
        if (startClip != null)
            skillSource.PlayOneShot(startClip);

        // if we do not have a hold audio, exit
        if (holdAudio == false) return;

        // get the hold audio clip
        SCRPT_EntityAudio.S_AudioClips clipData = ownerAudioClips.GetRandomSkillHoldClip();
        AudioClip holdClip = clipData.clip;

        if (holdClip == null) return;

        skillSource.loop = true;
        skillSource.clip = ownerAudioClips.GetRandomSkillHoldClip().clip;

        SetLoopPoint(holdClip, clipData);

        // if we have a start clip, 
        // wait for it to end
        if (startClip != null)
        {
            double clipDuration = startClip.length;
            skillSource.PlayScheduled(clipDuration);
        }
        else skillSource.Play();
    }

    private void SetLoopPoint(AudioClip holdClip, SCRPT_EntityAudio.S_AudioClips clipData)
    {
        onSkillAudioLoopPoint = LeanTween.value(0, 1, holdClip.length).setOnComplete(() =>
        {
            skillSource.pitch = Random.Range(1 - clipData.pitchRange, 1 + clipData.pitchRange);
            SetLoopPoint(holdClip, clipData);
        });
    }

    private void PlaySkillEndAudio(bool holdAudio)
    {
        if (holdAudio)
        {
            if (onSkillAudioLoopPoint != null) LeanTween.cancel(onSkillAudioLoopPoint.uniqueId);

            skillSource.Stop();
            skillSource.loop = false;
            skillSource.clip = null;
            skillSource.pitch = 1;
        }

        PlayAudioWithPitch(ownerAudioClips.GetRandomSkillEndClip());
    }

    private void PlayDashAudio()
    {
        PlayAudioWithPitch(ownerAudioClips.GetRandomDashClip());
        PlayAudioWithPitch(ownerAudioClips.GetRandomVoiceDashClip());
    }

    private void PlayFootPrintAudio() => PlayAudioWithPitch(ownerAudioClips.GetRandomIndoorFootsteps());


    private void PlayAudioWithPitch(AudioClip clip, float pitchRange)
    {
        ownerSource.pitch = Random.Range(1 - pitchRange, 1 + pitchRange);
        ownerSource.PlayOneShot(clip);
    }
    private void PlayAudioWithPitch(SCRPT_PlayerAudio.S_AudioClips clipData)
    {
        if (clipData.clip == null) return;

        ownerSource.pitch = Random.Range(1 - clipData.pitchRange, 1 + clipData.pitchRange);
        ownerSource.PlayOneShot(clipData.clip);
    }
}

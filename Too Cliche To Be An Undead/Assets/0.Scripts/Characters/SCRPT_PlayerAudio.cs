using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "Scriptable/Entity/Audio/Player")]
public class SCRPT_PlayerAudio : SCRPT_EntityAudio
{
    [SerializeField] private S_AudioClips[] attackConnectedClips;

    [SerializeField] private S_AudioClips[] bigAttackConnectedClips;


    [SerializeField] private S_AudioClips[] skillStartClips;

    [SerializeField] private S_AudioClips[] skillHoldClips;

    [SerializeField] private S_AudioClips[] skillEndClips;

    [SerializeField] private S_AudioClips[] dashClips;

    [SerializeField] private S_AudioClips[] footsteps_inSchool;
    [SerializeField] private S_AudioClips[] footsteps_outdoor;

    [SerializeField] private S_AudioClips[] voice_skillStartClips;
    [SerializeField] private S_AudioClips[] voice_dashStartClips;
    [SerializeField] private S_AudioClips[] voice_attackClips;

    public S_AudioClips GetRandomSkillStartClip() => skillStartClips.RandomElement();
    public S_AudioClips GetRandomSkillHoldClip() => skillHoldClips.RandomElement();
    public S_AudioClips GetRandomSkillEndClip() => skillEndClips.RandomElement();
    public S_AudioClips GetRandomDashClip() => dashClips.RandomElement();
    public S_AudioClips GetRandomBigAttackConnectedClip() => bigAttackConnectedClips.RandomElement();
    public S_AudioClips GetRandomAttackConnectedClip() => attackConnectedClips.RandomElement();

    public S_AudioClips GetRandomIndoorFootsteps() => footsteps_inSchool.RandomElement();
    public S_AudioClips GetRandomOOutdoorFootsteps() => footsteps_outdoor.RandomElement();

    public S_AudioClips GetRandomVoiceSkillStartClip() => voice_skillStartClips.RandomElement();
    public S_AudioClips GetRandomVoiceDashClip() => voice_dashStartClips.RandomElement();
    public S_AudioClips GetRandomVoiceAttackClip() => voice_attackClips.RandomElement();

}
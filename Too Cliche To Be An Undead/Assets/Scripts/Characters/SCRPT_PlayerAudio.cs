using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "Scriptable/Entity/Audio/Player")]
public class SCRPT_PlayerAudio : SCRPT_EntityAudio
{
    [SerializeField] private AudioClip[] swifClips;

    [SerializeField] private AudioClip[] skillStartClips;

    [SerializeField] private AudioClip[] skillHoldClips;

    [SerializeField] private AudioClip[] skillEndClips;

    [SerializeField] private AudioClip[] dashClips;

    public AudioClip GetRandomSwifClip() => swifClips.RandomElement();

    public AudioClip GetRandomSkillStartClip() => skillStartClips.RandomElement();
    public AudioClip GetRandomSkillHoldClip() => skillHoldClips.RandomElement();
    public AudioClip GetRandomSkillEndClip() => skillEndClips.RandomElement();
    public AudioClip GetRandomDashClip() => dashClips.RandomElement();
}
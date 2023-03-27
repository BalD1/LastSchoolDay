using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "Scriptable/Entity/Audio")]
public class SCRPT_EntityAudio : ScriptableObject
{
    [System.Serializable]
    public struct S_AudioClips
    {
        public AudioClip clip;
        public float pitchRange;
    }

    [SerializeField] private S_AudioClips[] attackClips;
    public S_AudioClips[] AttackClips { get => attackClips; }

    [SerializeField] private S_AudioClips[] bigAttackClips;
    public S_AudioClips[] BigAttackClips { get => bigAttackClips; }

    [SerializeField] private S_AudioClips[] hurtClips;
    public S_AudioClips[] HurtClips { get => hurtClips; }

    [SerializeField] private S_AudioClips[] deathClips;
    public S_AudioClips[] DeathClips { get => deathClips; }

    public S_AudioClips GetRandomAttackClip() => attackClips[Random.Range(0, attackClips.Length)];
    public S_AudioClips GetRandomBigAttackClip() => bigAttackClips[Random.Range(0, bigAttackClips.Length)];
    public S_AudioClips GetRandomHurtClip() => hurtClips[Random.Range(0, hurtClips.Length)];
    public S_AudioClips GetRandomDeathClip() => deathClips[Random.Range(0, deathClips.Length)];
}

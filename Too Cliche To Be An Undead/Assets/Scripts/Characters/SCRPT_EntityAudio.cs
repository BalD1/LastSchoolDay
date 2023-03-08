using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "Scriptable/Entity/Audio")]
public class SCRPT_EntityAudio : ScriptableObject
{
    [SerializeField] private AudioClip[] attackClips;
    public AudioClip[] AttackClips { get => attackClips; }

    [SerializeField] private AudioClip[] bigAttackClips;
    public AudioClip[] BigAttackClips { get => bigAttackClips; }

    [SerializeField] private AudioClip[] hurtClips;
    public AudioClip[] HurtClips { get => hurtClips; }

    [SerializeField] private AudioClip[] deathClips;
    public AudioClip[] DeathClips { get => deathClips; }

    public AudioClip GetRandomAttackClip() => attackClips[Random.Range(0, attackClips.Length)];
    public AudioClip GetRandomBigAttackClip() => bigAttackClips[Random.Range(0, bigAttackClips.Length)];
    public AudioClip GetRandomHurtClip() => hurtClips[Random.Range(0, hurtClips.Length)];
    public AudioClip GetRandomDeathClip() => deathClips[Random.Range(0, deathClips.Length)];
}

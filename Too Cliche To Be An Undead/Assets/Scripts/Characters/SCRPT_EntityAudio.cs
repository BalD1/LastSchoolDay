using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "Scriptable/Entity/Audio")]
public class SCRPT_EntityAudio : ScriptableObject
{
    [SerializeField] private AudioClip[] attackClips;
    public AudioClip[] AttackClips { get => attackClips; }

    [SerializeField] private AudioClip[] hurtClips;
    public AudioClip[] HurtClips { get => hurtClips; }

    [SerializeField] private AudioClip[] deathClips;
    public AudioClip[] DeathClips { get => deathClips; }

    public AudioClip GetRandomAttackClip() => attackClips[Random.Range(0, attackClips.Length)];
    public AudioClip GetRandomHurtClip() => attackClips[Random.Range(0, attackClips.Length)];
    public AudioClip GetRandomDeathClip() => attackClips[Random.Range(0, attackClips.Length)];
}

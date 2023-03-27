using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Zombie Audio", menuName = "Scriptable/Entity/Audio/Zombie")]
public class SCRPT_ZombieAudio : SCRPT_EntityAudio
{
    [SerializeField] private S_AudioClips[] miscAudio;

    public S_AudioClips GetRandomMiscAudio() => miscAudio.RandomElement();
}

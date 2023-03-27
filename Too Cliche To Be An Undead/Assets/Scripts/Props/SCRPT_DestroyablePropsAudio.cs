using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AUDIO_DestroyableProps", menuName = "Scriptable/Entity/DestroyableProps/Audio")]
public class SCRPT_DestroyablePropsAudio : ScriptableObject
{
    [field: SerializeField] public AudioClip[] HurtClips { get; private set; }
    [field: SerializeField] public AudioClip[] DeathClips { get; private set; }
}
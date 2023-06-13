using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossAudio", menuName = "Scriptable/Entity/Enemy/Boss/Audio")]
public class SCRPT_BossAudio : ScriptableObject
{
    [field: SerializeField] public SCRPT_EntityAudio.S_AudioClips CaCClips { get; private set; }
    [field: SerializeField] public SCRPT_EntityAudio.S_AudioClips DashClips { get; private set; }
    [field: SerializeField] public SCRPT_EntityAudio.S_AudioClips HowlingClips { get; private set; }
    [field: SerializeField] public SCRPT_EntityAudio.S_AudioClips FallClips { get; private set; }
}
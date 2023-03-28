using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "Scriptable/Entity/Audio/Shop")]
public class SCRPT_ShopAudio : ScriptableObject
{
    [field: SerializeField] public SCRPT_EntityAudio.S_AudioClips openShopAudio { get; private set; }
    [field: SerializeField] public SCRPT_EntityAudio.S_AudioClips closeShopAudio { get; private set; }
    [field: SerializeField] public SCRPT_EntityAudio.S_AudioClips buyAudio { get; private set; }
    [field: SerializeField] public SCRPT_EntityAudio.S_AudioClips notEnoughMoneyAudio { get; private set; }
}

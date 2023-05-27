using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Speaker Data", menuName = "Scriptable/Dialogue/SpeakerData")]
public class SCRPT_DialogueSpeakerData : ScriptableObject
{
    [field: SerializeField] public Sprite speakerNameImage { get; private set; }
    [field: SerializeField] public SCRPT_PortraitsWithRect speakerPortraitImage { get; private set; }
    [field: SerializeField] public GameManager.E_CharactersNames characterName { get; private set; }

}
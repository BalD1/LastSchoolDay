using UnityEngine;

[CreateAssetMenu(fileName = "CharacterComponents", menuName = "Scriptable/Entity/CharacterComponents")]
public class SO_CharactersComponents : ScriptableObject
{
    [field: SerializeField] public GameManager.E_CharactersNames Character { get; private set; }
    [field: SerializeField] public SO_WeaponData WeaponData { get; private set; }
    [field: SerializeField] public SCRPT_Dash Dash { get; private set; }
    [field: SerializeField] public SCRPT_Skill Skill { get; private set; }
    [field: SerializeField] public SO_BaseStats Stats { get; private set; }
    [field: SerializeField] public SCRPT_PlayerAudio AudioClips { get; private set; }
    [field: SerializeField] public SO_PlayersAnimData AnimData { get; private set; }
}
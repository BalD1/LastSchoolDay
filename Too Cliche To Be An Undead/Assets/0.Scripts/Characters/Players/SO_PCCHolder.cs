using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PCCHolder", menuName = "Scriptable/Entity/PCCHolder")]
public class SO_PCCHolder : ScriptableObject
{
    [field: SerializeField] public SO_CharactersComponents ShirleyComponents { get; private set; }
    [field: SerializeField] public SO_CharactersComponents WhitneyComponents { get; private set; }
    [field: SerializeField] public SO_CharactersComponents JasonComponents { get; private set; }
    [field: SerializeField] public SO_CharactersComponents NelsonComponents { get; private set; }

    public SO_CharactersComponents GetComponents(GameManager.E_CharactersNames character)
    {
        switch (character)
        {
            case GameManager.E_CharactersNames.Shirley:
                return ShirleyComponents;
            case GameManager.E_CharactersNames.Whitney:
                return WhitneyComponents;
            case GameManager.E_CharactersNames.Jason:
                return JasonComponents;
            case GameManager.E_CharactersNames.Nelson:
                return NelsonComponents;
        }

        return ShirleyComponents;
    }
}
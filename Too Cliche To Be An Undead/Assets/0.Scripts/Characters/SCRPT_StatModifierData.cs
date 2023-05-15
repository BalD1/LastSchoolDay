using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModifierData", menuName = "Scriptable/Entity/Stats/ModifierData")]
public class SCRPT_StatModifierData : ScriptableObject
{
    [field: SerializeField] public string ModifierID { get; private set; }

    [field: SerializeField] public float Value { get; private set; }

    [field: SerializeField] public float Lifetime { get; private set; }

    [field: SerializeField] public StatsModifier.E_StatType StatType { get; private set; }
}
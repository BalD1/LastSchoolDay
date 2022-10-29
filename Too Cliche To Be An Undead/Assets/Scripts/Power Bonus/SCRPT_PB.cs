using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Power Bonus", menuName = "Scriptable/Power Bonus")]
public class SCRPT_PB : ScriptableObject
{
    [SerializeField] private Sprite thumbnail;
    public Sprite Thumbnail { get => thumbnail; }

    [SerializeField] private string customID;
    public string ID
    {
        get
        {
            if (customID != null && customID != "") return customID;

            return $"PB_{StatsModifier.TypeToString(statType)}_{amount}";
        }
    }

    [SerializeField] private StatsModifier.E_StatType statType;
    public StatsModifier.E_StatType StatType { get => statType; }

    [SerializeField] private float amount;
    public float Amount { get => amount; }

    [SerializeField] private string description;
    public string Description { get => description; }
}

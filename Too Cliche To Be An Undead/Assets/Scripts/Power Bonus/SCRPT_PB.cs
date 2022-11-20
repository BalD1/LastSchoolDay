using System.Collections;
using System.Collections.Generic;
using System.Text;
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

            StringBuilder sb = new StringBuilder("PB_");
            sb.Append(StatsModifier.TypeToString(statType));
            sb.Append("_");
            sb.Append(amount);

            return sb.ToString();
        }
    }

    [SerializeField] private StatsModifier.E_StatType statType;
    public StatsModifier.E_StatType StatType { get => statType; }

    [SerializeField] private bool isUnique;
    public bool IsUnique { get => isUnique; }

    [SerializeField] private GameManager.E_CharactersNames associatedCharacter;
    public GameManager.E_CharactersNames AssociatedCharacter { get => associatedCharacter; }

    [SerializeField] private float amount;
    public float Amount { get => amount; }

    [SerializeField] private float AC_amount;
    public float AC_Amount { get => AC_amount; }

    [SerializeField] private string description;
    public string Description { get => description; }
}

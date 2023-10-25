using System;
using System.Collections.Generic;
using UnityEngine;
using static IStatContainer;

public class StatsHandler : MonoBehaviour
{
    [field: SerializeField] public SO_BaseStats BaseStats { get; private set; }
    public Dictionary<E_StatType, float> PermanentBonusStats { get; protected set; } = new Dictionary<E_StatType, float>();
    public Dictionary<E_StatType, float> TemporaryBonusStats { get; protected set; } = new Dictionary<E_StatType, float>();
    public Dictionary<E_StatType, float> BrutFinalStats { get; protected set; } = new Dictionary<E_StatType, float>();

    public Dictionary<string, NewStatsModifier> UniqueStatsModifiers { get; protected set; } = new Dictionary<string, NewStatsModifier>();
    public Dictionary<string, List<NewStatsModifier>> StackableStatsModifiers { get; protected set; } = new Dictionary<string, List<NewStatsModifier>>();



    public enum E_ModifierAddResult
    {
        Success,
        StatAlreadyMaxed,
        Unstackable,
    }

    public event Action<StatChangeEventArgs> OnStatChange;
    public class StatChangeEventArgs : EventArgs
    {
        public StatChangeEventArgs(E_StatType type, float modifier, float finalVal)
        {
            this.Type = type;
            this.ModifierValue = modifier;
            this.FinalValue = finalVal;
        }
        public E_StatType Type { get; private set; }
        public float ModifierValue { get; private set; }
        public float FinalValue { get; private set; }
    }

    private void Awake()
    {
        SetupStats();
    }

    protected void SetupStats()
    {
        BrutFinalStats = new Dictionary<E_StatType, float>();
        foreach (var item in BaseStats.GetAllStats())
        {
            BrutFinalStats.Add(item.Key, item.Value.Value);
            PermanentBonusStats.Add(item.Key, 0);
            TemporaryBonusStats.Add(item.Key, 0);
        }
    }

    public bool TryGetBaseStat(E_StatType type, out float value)
    {
        return BaseStats.TryGetStatValue(type, out value);
    }

    public bool TryGetFinalStat(E_StatType type, out float value)
    {
        bool res = BrutFinalStats.TryGetValue(type, out value);
        if (!res) return false;

        float higherAllowedValue = BaseStats.Stats[type].HigherAllowedValue;

        if (value > higherAllowedValue)
            value = higherAllowedValue;

        return true;
    }

    public bool TryAddModifier(SO_StatModifierData data, out E_ModifierAddResult result)
    {
        if (data.Stackable)
            return TryAddStackableModifier(data, out result);
        else
            return TryAddUniqueModifier(data, out result);
    }

    private bool TryAddStackableModifier(SO_StatModifierData data, out E_ModifierAddResult result)
    {
        if (!TrySetModifier(data))
        {
            result = E_ModifierAddResult.StatAlreadyMaxed;
            return false;
        }

        //add the modifier to the list
        if (!StackableStatsModifiers.ContainsKey(data.ID))
            StackableStatsModifiers.Add(data.ID, new List<NewStatsModifier>());
        StackableStatsModifiers[data.ID].Add(new NewStatsModifier(data, this));
         
        result = E_ModifierAddResult.Success;
        ModifyBrutFinalStat(data.StatType, data.Amount);
        return true;
    }

    private bool TryAddUniqueModifier(SO_StatModifierData data, out E_ModifierAddResult result)
    {
        // if the unique modifier already exists, return
        if (UniqueStatsModifiers.ContainsKey(data.ID))
        {
            result = E_ModifierAddResult.Unstackable;
            return false;
        }
        if (!TrySetModifier(data))
        {
            result = E_ModifierAddResult.StatAlreadyMaxed;
            return false;
        }

        UniqueStatsModifiers.Add(data.ID, new NewStatsModifier(data, this));
        ModifyBrutFinalStat(data.StatType, data.Amount);
        result = E_ModifierAddResult.Success;
        return true;
    }

    private bool TrySetModifier(SO_StatModifierData data)
    {
        if (data.Temporary)
        {
            if (TemporaryBonusStats.ContainsKey(data.StatType))
                TemporaryBonusStats[data.StatType] += data.Amount;
            else
                TemporaryBonusStats.Add(data.StatType, data.Amount);
        }
        else
        {
            // else, check if the sum of all permanent bonus is >= of max, return.
            // else, add it to permanent bonuses
            if (PermanentBonusStats[data.StatType] >= (BaseStats.Stats[data.StatType].HigherAllowedValue - BaseStats.Stats[data.StatType].Value))
                return false;
            if (PermanentBonusStats.ContainsKey(data.StatType))
                PermanentBonusStats[data.StatType] += data.Amount;
            else
                PermanentBonusStats.Add(data.StatType, data.Amount);
        }
        return true;
    }

    private void ModifyBrutFinalStat(E_StatType type, float value)
    {
        if (BrutFinalStats.ContainsKey(type))
        {
            BrutFinalStats[type] += value;
            OnStatChange?.Invoke(new StatChangeEventArgs(type, value, BrutFinalStats[type]));
        }
    }

    public void RemoveStatModifier(NewStatsModifier modifier)
    {
        if (modifier.Data.Temporary)
            TemporaryBonusStats[modifier.Data.StatType] -= modifier.Data.Amount;
        else
            PermanentBonusStats[modifier.Data.StatType] -= modifier.Data.Amount;

        if (modifier.Data.Stackable)
        {
            ModifyBrutFinalStat(modifier.Data.StatType, -modifier.Data.Amount);
            StackableStatsModifiers[modifier.Data.ID].Remove(modifier);

            return;
        }

        ModifyBrutFinalStat(modifier.Data.StatType, -modifier.Data.Amount);
        UniqueStatsModifiers.Remove(modifier.Data.ID);
    }
}

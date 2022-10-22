using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "Scriptable/Entity/Stats")]
public class SCRPT_EntityStats : ScriptableObject
{
    [SerializeField] private string entityName;
    [SerializeField] private float maxHP;
    [SerializeField] private float baseDamages;
    [SerializeField] private float attackRange;
    [SerializeField] private float attack_COOLDOWN;
    [SerializeField] private float invincibility_COOLDOWN;
    [SerializeField] private float speed;
    [SerializeField] private int critChances;
    [SerializeField] private float weight;
    [SerializeField] private E_Team team;

    public enum E_Team
    {
        Players,
        Zombies,
        Neutral,
    }

    public string EntityType { get => name; }
    public float MaxHP(List<StatsModifier> modifiers) { return GetValueWithModifier(maxHP, StatsModifier.E_StatType.MaxHP, modifiers); } 

    public float BaseDamages(List<StatsModifier> modifiers) { return GetValueWithModifier(baseDamages, StatsModifier.E_StatType.Damages, modifiers); }
    public float AttackRange(List<StatsModifier> modifiers) { return GetValueWithModifier(attackRange, StatsModifier.E_StatType.AttackRange, modifiers); }
    public float Attack_COOLDOWN(List<StatsModifier> modifiers) { return GetValueWithModifier(attack_COOLDOWN, StatsModifier.E_StatType.Attack_CD, modifiers); }
    public float Invincibility_COOLDOWN { get => invincibility_COOLDOWN; }
    public float Speed(List<StatsModifier> modifiers) { return GetValueWithModifier(speed, StatsModifier.E_StatType.Speed, modifiers); }
    public int CritChances(List<StatsModifier> modifiers) { return GetValueWithModifier(critChances, StatsModifier.E_StatType.CritChances, modifiers); }
    public float Weight { get => weight; }
    public E_Team Team { get => team; }

    private float GetValueWithModifier(float baseVal, StatsModifier.E_StatType type, List<StatsModifier> modifiers)
    {
        float m = 0;
        foreach (var item in modifiers)
        {
            if (item.StatType == type)
                m += item.Modifier;
        }

        return baseVal + m;
    }
    private int GetValueWithModifier(int baseVal, StatsModifier.E_StatType type, List<StatsModifier> modifiers)
    {
        int m = 0;
        foreach (var item in modifiers)
        {
            if (item.StatType == type)
                m += (int)item.Modifier;
        }

        return baseVal + m;
    }

    public override string ToString()
    {
        return entityName;
    }

    public void Log(GameObject owner = null)
    {
#if UNITY_EDITOR
        string col = GetMarkdownColor();

        Debug.LogFormat("Entity of type <b>\"{0}\"</b> : MaxHP = {1}       Speed = {2}       Team = <color={3}>{4}</color>", entityName, maxHP, speed, col, team, owner);
#endif
    }

    public string GetMarkdownColor()
    {
        string col = "white";

        switch (team)
        {
            case E_Team.Players:
                col = "blue";
                break;

            case E_Team.Zombies:
                col = "red";
                break;

            case E_Team.Neutral:
                col = "grey";
                break;
        }

        return col;
    }
}

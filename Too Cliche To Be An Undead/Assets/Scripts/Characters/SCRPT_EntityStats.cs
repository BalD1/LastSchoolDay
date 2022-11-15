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
    public float MaxHP { get => maxHP; } 

    public float BaseDamages { get => baseDamages; }
    public float AttackRange { get => attackRange; }
    public float Attack_COOLDOWN { get => attack_COOLDOWN; }
    public float Invincibility_COOLDOWN { get => invincibility_COOLDOWN; }
    public float Speed { get => speed; }
    public int CritChances { get => critChances; }
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

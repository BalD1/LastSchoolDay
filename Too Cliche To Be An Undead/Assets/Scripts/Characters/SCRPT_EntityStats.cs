using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "Scriptable/Entity/Stats")]
public class SCRPT_EntityStats : ScriptableObject
{
    [SerializeField] private string entityName;
    [SerializeField] private float maxHP;
    [SerializeField] private float maxHP_MAX = float.MaxValue;
    [Space]

    [SerializeField] private float baseDamages;
    [SerializeField] private float baseDamages_MAX = float.MaxValue;
    [Space]

    [SerializeField] private float attackRange;
    [SerializeField] private float attackRange_MAX = float.MaxValue;
    [Space]

    [SerializeField] private float attack_COOLDOWN;
    [SerializeField] private float attack_COOLDOWN_MAX = float.MaxValue;
    [Space]

    [SerializeField] private float invincibility_COOLDOWN;
    [SerializeField] private float invincibility_COOLDOWN_MAX = float.MaxValue;
    [Space]

    [SerializeField] private float speed;
    [SerializeField] private float speed_MAX = float.MaxValue;
    [Space]

    [SerializeField] private int critChances;
    [SerializeField] private int critChances_MAX = int.MaxValue;
    [Space]

    [SerializeField] private float weight;
    [SerializeField] private E_Team team;

    public enum E_Team
    {
        Players,
        Zombies,
        Neutral,
    }

    public string EntityName { get => entityName; }
    public string EntityType { get => name; }
    public float MaxHP { get => maxHP; } 
    public float MaxHP_MAX { get => maxHP_MAX; }

    public float BaseDamages { get => baseDamages; }
    public float BaseDamages_MAX { get => baseDamages_MAX; }

    public float AttackRange { get => attackRange; }
    public float AttackRange_MAX { get => attackRange_MAX; }

    public float Attack_COOLDOWN { get => attack_COOLDOWN; }
    public float Attack_COOLDOWN_MAX { get => attack_COOLDOWN_MAX; }

    public float Invincibility_COOLDOWN { get => invincibility_COOLDOWN; }
    public float Invincibility_MAX { get => invincibility_COOLDOWN_MAX; }

    public float Speed { get => speed; }
    public float Speed_MAX { get => speed_MAX; }

    public int CritChances { get => critChances; }
    public int CritChances_MAX { get => critChances_MAX; }
    public float Weight { get => weight; }
    public E_Team Team { get => team; }

    public override string ToString()
    {
        return entityName;
    }

    public void Log(string objectName = "", GameObject owner = null)
    {
#if UNITY_EDITOR
        string col = GetMarkdownColor();

        Debug.LogFormat("Entity of type <color={8}><b>[{0}], {1}</b></color> \n" +
            "MaxHP = {2} \n" +
            "Base Damages = {3}\n" +
            "Attack Range = {4}\n" +
            "Attack CD = {5}\n" +
            "Crit Chances = {6}\n" +
            "Speed = {7}\n" +
            "Team = <color={8}>{9}</color>", 
            entityName, objectName, maxHP, baseDamages, attackRange, attack_COOLDOWN, critChances, speed, col, team, owner);
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

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
    public E_Team Team { get => team; }

    public override string ToString()
    {
        return entityName;
    }

    public void Log(GameObject owner = null)
    {
        string col = GetMarkdownColor();

        Debug.LogFormat("Entity of type <b>\"{0}\"</b> : MaxHP = {1}       Speed = {2}       Team = <color={3}>{4}</color>", entityName, maxHP, speed, col, team, owner);
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

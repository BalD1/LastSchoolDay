using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Whitney", menuName = "Scriptable/Entity/Dash/Whitney")]
public class SCRPT_Dash_Whitney : SCRPT_Dash
{
    [SerializeField] private GameObject particles;

    [SerializeField] private int critBoostValue = 20;
    [SerializeField] private float critBoostTime = 1;

    public const string MODIF_ID = "WH_SKILL_CRIT_20";

    public override void OnDashStart(PlayerCharacter owner)
    {
    }

    public override void OnDashUpdate(PlayerCharacter owner)
    {
    }

    public override void OnDashStop(PlayerCharacter owner)
    {
        owner.AddModifierUnique(MODIF_ID, critBoostValue, critBoostTime, StatsModifier.E_StatType.CritChances);

        if (particles != null)
            Instantiate(particles, owner.transform.position, Quaternion.identity);
    }
}

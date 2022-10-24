using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Whitney", menuName = "Scriptable/Entity/Dash/Whitney")]
public class SCRPT_Dash_Whitney : SCRPT_Dash
{
    public const string MODIF_ID = "WH_SKILL_CRIT_20";

    public override void OnDashStart(PlayerCharacter owner)
    {
    }

    public override void OnDashUpdate(PlayerCharacter owner)
    {
    }

    public override void OnDashStop(PlayerCharacter owner)
    {
        owner.AddModifier(MODIF_ID, value: 20, time: 1, type: StatsModifier.E_StatType.CritChances);
    }
}

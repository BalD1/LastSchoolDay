using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Whitney", menuName = "Scriptable/Entity/Dash/Whitney")]
public class SCRPT_Dash_Whitney : SCRPT_Dash
{
    public override void OnDashStart(PlayerCharacter owner)
    {
    }

    public override void OnDashUpdate(PlayerCharacter owner)
    {
    }

    public override void OnDashStop(PlayerCharacter owner)
    {
        owner.AddModifier(time: 1, value: 20, type: StatsModifier.E_StatType.CritChances);
    }
}

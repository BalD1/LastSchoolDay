using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Jason", menuName = "Scriptable/Entity/Dash/Jason")]
public class SCRPT_Dash_Jason : SCRPT_Dash
{
    public override void OnDashStart(PlayerCharacter owner)
    {
        base.OnDashStart(owner);
        owner.SetInvincibility(true);
    }

    public override void OnDashStop(PlayerCharacter owner)
    {
        base.OnDashStop(owner);
        owner.SetInvincibility(false);
    }
}

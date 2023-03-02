using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nelson", menuName = "Scriptable/Entity/Dash/Nelson")]
public class SCRPT_Dash_Nelson : SCRPT_Dash
{
    [SerializeField] private GameObject trapPF;

    [SerializeField] private float damagesPercentageModifier = 1;

    [SerializeField] private float trapsDuration = 5f;

    [SerializeField] private bool destroyTrapOnTrigger = true;

    public override void OnDashStart(PlayerCharacter owner)
    {
        base.OnDashStart(owner);

        TrapBase tb = Instantiate(trapPF, owner.transform.position, Quaternion.identity).GetComponent<TrapBase>();

        float finalDamages = owner.maxDamages_M * damagesPercentageModifier;

        tb.Setup(finalDamages, trapsDuration, SCRPT_EntityStats.E_Team.Players, destroyTrapOnTrigger);
    }
}

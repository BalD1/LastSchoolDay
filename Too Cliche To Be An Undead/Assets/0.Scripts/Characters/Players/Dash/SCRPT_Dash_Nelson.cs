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

    public override void OnDashStart(PlayerCharacter owner, Vector2 direction)
    {
        base.OnDashStart(owner, direction);

        TrapBase tb = Instantiate(trapPF, owner.transform.position, Quaternion.identity).GetComponent<TrapBase>();

        float finalDamages = owner.MaxDamages_M * damagesPercentageModifier;

        tb.Setup(finalDamages, trapsDuration, owner, destroyTrapOnTrigger);
    }
}
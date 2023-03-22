using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shirley", menuName = "Scriptable/Entity/Dash/Shirley")]
public class SCRPT_Dash_Shirley : SCRPT_Dash
{
    [SerializeField] private float damagesPercentageModifier = .75f;

    private List<Entity> hitEntities;

    private SCRPT_EntityStats.E_Team team;

    private float finalDamages;
    private int critChances;

    private PlayerCharacter player;

    public override void OnDashStart(PlayerCharacter owner, Vector2 direction)
    {
        player = owner;
        base.OnDashStart(owner, direction);

        finalDamages = owner.MaxDamages_M * damagesPercentageModifier;
        critChances = owner.MaxCritChances_M;
        team = owner.GetStats.Team;

        hitEntities = new List<Entity>();

        owner.d_EnteredTrigger += EnteredTrigger;
    }

    public override void OnDashStop(PlayerCharacter owner)
    {
        base.OnDashStop(owner);
        owner.d_EnteredTrigger -= EnteredTrigger;
    }

    private void EnteredTrigger(Collider2D c)
    {
        Entity e = c.GetComponentInParent<Entity>();

        if (e == null) return;

        if (!hitEntities.Contains(e))
        {
            e.OnTakeDamages(finalDamages, player, Random.Range(0, 100) <= critChances);
            hitEntities.Add(e);
        }
    }
}

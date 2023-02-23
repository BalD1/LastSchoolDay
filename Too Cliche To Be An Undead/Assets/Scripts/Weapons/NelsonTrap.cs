using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NelsonTrap : TrapBase
{
    private const string TRAP_TICK_ID = "NELSON_TICK_TRAP";
    private const string TRAP_MODIFIER_ID = "NELSON_MODIFIER_TRAP";

    [SerializeField] private float speedModifier = -1;

    [SerializeField] private float timeBetweenTicks = .5f;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        NormalZombie nz = collision.GetComponent<NormalZombie>();

        if (nz == null) return;

        nz.AddModifier(TRAP_MODIFIER_ID, speedModifier, StatsModifier.E_StatType.Speed);
        nz.AddTickDamages(TRAP_TICK_ID, damages, timeBetweenTicks, 999);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        NormalZombie nz = collision.GetComponent<NormalZombie>();

        if (nz == null) return;

        nz.RemoveModifier(TRAP_MODIFIER_ID);
        nz.RemoveTickDamages(TRAP_TICK_ID);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBloodSpiller : BloodSpiller
{
    protected override void Awake()
    {
        (owner as BossZombie).onReceiveAttack += ReceiveAttack;
        //owner.OnDeath += SpillBloodOnDeath;
    }

    private void ReceiveAttack(Entity entity, bool tickDamages) => SpillBloodOnDamages(false, entity, tickDamages);
}

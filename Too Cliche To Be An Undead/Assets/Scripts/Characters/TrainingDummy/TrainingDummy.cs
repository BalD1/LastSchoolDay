using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : Entity
{
    [SerializeField] private float regen_TIME;
    private float regen_TIMER;

    private bool isInCombat;

    protected override void Update()
    {
        base.Update();
        if (regen_TIMER > 0) regen_TIMER -= Time.deltaTime;
        else if (regen_TIMER <= 0 && isInCombat) Regen();
    }

    public override bool OnTakeDamages(float amount, bool isCrit = false)
    {
        bool res = base.OnTakeDamages(amount, isCrit);
        if (!res) return res;

        regen_TIMER = regen_TIME;
        isInCombat = true;

        return true;
    }

    public override void OnDeath(bool forceDeath = false)
    {
        this.OnHeal(this.GetStats.MaxHP);
    }

    private void Regen()
    {
        isInCombat = false;
        this.OnHeal(this.GetStats.MaxHP);
    }


}

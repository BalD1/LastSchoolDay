using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : Entity
{
    [SerializeField] private float regen_TIME;

        protected override void Update()
    {
        base.Update();
    }

    public override bool OnTakeDamages(float amount, bool isCrit = false)
    {
        bool res = base.OnTakeDamages(amount, isCrit);
        if (!res) return res;

        StopCoroutine(Regen());
        StartCoroutine(Regen());

        return true;
    }

    public override void OnDeath(bool forceDeath = false)
    {
        this.OnHeal(this.GetStats.MaxHP);
    }

    private IEnumerator Regen()
    {
        yield return new WaitForSeconds(regen_TIME);

        this.OnHeal(this.GetStats.MaxHP);
    }


}

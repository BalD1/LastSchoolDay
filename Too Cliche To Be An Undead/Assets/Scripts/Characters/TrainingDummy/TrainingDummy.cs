using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrainingDummy : Entity
{
    [SerializeField] private TextMeshPro damagesText;
    [SerializeField] private TextMeshPro timeText;

    [SerializeField] private float regen_TIME;
    private float regen_TIMER;

    private bool isInCombat;

    private float combatTime = 1;
    private float totalReceived;
    private float receivedDPS;

    private int receivedAttacks;

    protected override void Update()
    {
        base.Update();
        if (regen_TIMER > 0)
        {
            combatTime += Time.deltaTime;
            regen_TIMER -= Time.deltaTime;

            UpdateTimeText();
        }
        else if (regen_TIMER <= 0 && isInCombat) Regen();
    }

    public override bool OnTakeDamages(float amount, bool isCrit = false)
    {
        bool res = base.OnTakeDamages(amount, isCrit);
        if (!res) return res;

        if (regen_TIMER <= 0)
        {
            totalReceived = 0;
            receivedDPS = 0;
            receivedAttacks = 0;
        }

        totalReceived += amount;

        receivedAttacks++;

        receivedDPS = (totalReceived / combatTime);

        UpdateDamagesText();

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
        combatTime = 1;
        totalReceived = 0;
        receivedDPS = 0;
        receivedAttacks = 0;

        this.OnHeal(this.GetStats.MaxHP);
    }

    private void UpdateTimeText()
    {
        timeText.text = (combatTime - 1).ToString("F2") + "s";
    }
    private void UpdateDamagesText()
    {
        damagesText.text = "DPS : " + receivedDPS.ToString("F2") + "\n" +
                        "Total : " + totalReceived + "\n" +
                        "Attacks : " + receivedAttacks + "\n";
    }
}

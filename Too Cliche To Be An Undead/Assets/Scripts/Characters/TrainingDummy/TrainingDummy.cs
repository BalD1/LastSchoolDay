using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrainingDummy : EnemyBase
{
    [SerializeField] private FSM_TD_Manager stateManager;

    [SerializeField] private TextMeshPro damagesText;
    [SerializeField] private TextMeshPro timeText;
    [SerializeField] private TextMeshPro statusText;

    [SerializeField] private float regen_TIME;
    private float regen_TIMER;

    private bool isInCombat;

    private float combatTime = 1;
    private float totalReceived;
    private float receivedDPS;

    private int receivedAttacks;

#if UNITY_EDITOR
    [ReadOnly] [SerializeField] private string state = "N/A";
#endif

    public static TrainingDummy Create(Vector2 pos, float _regen_TIME)
    {
        GameObject gO = Instantiate(GameAssets.Instance.TrainingDummyPF, pos, Quaternion.identity);

        TrainingDummy tD = gO.GetComponent<TrainingDummy>();
        tD.regen_TIME = _regen_TIME;

        return tD;
    }

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

#if UNITY_EDITOR
        if (debugMode) state = stateManager.ToString();
#endif
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

    public override void Stun(float duration)
    {
        base.Stun(duration);
        stateManager.SwitchState(stateManager.stunState.SetDuration(duration));
        statusText.text = "STUN";
        statusText.enabled = true;
    }

    public void HideStatusText() => statusText.enabled = false;
}

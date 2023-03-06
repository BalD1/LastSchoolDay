using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class TrainingDummy : EnemyBase
{
    [Header("Single")]
    [Header("Components")]

    [SerializeField] private FSM_TD_Manager stateManager;

    [SerializeField] private TextMeshPro damagesText;
    [SerializeField] private TextMeshPro timeText;
    [SerializeField] private TextMeshPro statusText;

    [Header("Stats", order = 0)]

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

    protected override void Start()
    {
        canBePushed = false;
        base.Start();
    }

    protected override void Update()
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;
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

    public override bool OnTakeDamages(float amount, bool isCrit = false, bool fakeDamages = false, bool callDelegate = true)
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
        this.OnHeal(this.maxHP_M);
    }

    private void Regen()
    {
        isInCombat = false;
        combatTime = 1;
        totalReceived = 0;
        receivedDPS = 0;
        receivedAttacks = 0;

        this.OnHeal(this.maxHP_M);
    }

    private void UpdateTimeText()
    {
        timeText.text = (combatTime - 1).ToString("F2") + "s";
    }
    private void UpdateDamagesText()
    {
        StringBuilder sb = new StringBuilder("DPS : ");
        sb.AppendLine(receivedDPS.ToString("F2"));
        
        sb.Append("Total : ");
        sb.AppendLine(totalReceived.ToString());
        
        sb.Append("Attacks : ");
        sb.AppendLine(receivedAttacks.ToString());
        
        damagesText.text = sb.ToString();
    }

    public override void Stun(float duration, bool resetAttackTimer = false, bool showStuntext = false)
    {
        stateManager.SwitchState(stateManager.stunState.SetDuration(duration, resetAttackTimer));
        statusText.text = "STUN";
        statusText.enabled = true;
    }

    public override Vector2 Push(Vector2 pusherPosition, float pusherForce, Entity originalPusher)
    {
        if (stateManager.ToString().Equals("Pushed")) return Vector2.zero;

        Vector2 v = base.Push(pusherPosition, pusherForce, originalPusher);
        stateManager.SwitchState(stateManager.pushedState.SetForce(v, originalPusher));

        return v;
    }

    public void HideStatusText() => statusText.enabled = false;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZombie : EnemyBase
{
    [SerializeField] private FSM_Boss_Manager stateManager;

    [SerializeField] private SCRPT_EnemyAttack[] attacks;

    [field: SerializeField] public Transform SkeletonHolder { get; private set; }

    [field: SerializeField] public BossAnimationsController animationController { get; private set; }
    [field: SerializeField] public SCRPT_BossAnimData animationData { get; private set; }

    [SerializeField] private SCRPT_BossPatern.S_AttackAndCooldown currentAttack;
    public SCRPT_BossPatern.S_AttackAndCooldown CurrentAttack
    {
        get
        {
            return currentAttack;
        }
        set => currentAttack = value;
    }

    [field: SerializeField] public AttacksPatern attacksPatern { get; private set; }

    public bool attackStarted;

    [field: SerializeField] public Vector2 AttackDirection { get; set; }

    [field: SerializeField] public float hpThresholdBeforeNextStage { get; set; }

    [field: SerializeField] public StatsModifier[] secondStageModifiers;

    protected override void Start()
    {
        TargetClosestPlayer();
        base.Start();
        Pathfinding?.StartUpdatePath();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override bool OnTakeDamages(float amount, bool isCrit = false, bool fakeDamages = false, bool callDelegate = true)
    {
        bool res = base.OnTakeDamages(amount, isCrit, fakeDamages);

        if (this.currentHP <= (this.maxHP_M * hpThresholdBeforeNextStage) && attacksPatern.currentStage == 0) AdvanceToNextStage();

        return res;
    }

    private void AdvanceToNextStage()
    {
        attacksPatern.NextStage();
        foreach (var item in secondStageModifiers)
        {
            AddModifier(item.IDName, item.Modifier, item.StatType);
        }
    }

    public override void OnDeath(bool forceDeath = false)
    {
        base.OnDeath(forceDeath);
    }

    public override void Stun(float duration, bool resetAttackTimer = false, bool showStuntext = false)
    {
        stateManager.SwitchState(stateManager.stunnedState.SetDuration(duration, resetAttackTimer));
        this.attackTelegraph.CancelTelegraph();
    }

    public void TargetClosestPlayer()
    {
        PlayerCharacter closerTarget = null;
        float closerDistance = float.MaxValue;
        foreach (var item in GameManager.Instance.playersByName)
        {
            float currentDist = Vector2.Distance(this.transform.position, item.playerScript.transform.position);
            if (currentDist < closerDistance)
            {
                closerDistance = currentDist;
                closerTarget = item.playerScript;
            }
        }

        this.SetTarget(closerTarget);
    }

}

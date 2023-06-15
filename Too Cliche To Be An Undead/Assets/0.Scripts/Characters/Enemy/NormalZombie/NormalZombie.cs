using Spine.Unity;
using System;
using UnityEngine;

public class NormalZombie : EnemyBase, IDistanceChecker
{
    [SerializeField] private FSM_NZ_Manager stateManager;
    public FSM_NZ_Manager StateManager { get => stateManager; }

    [field: SerializeField] public GameObject spineHolder { get; private set; }
    [field: SerializeField] public ZombiesAnimationController animationController { get; private set; }
    [field: SerializeField] public SCRPT_ZombiesAnimDatga animationData { get; private set; }

    [field: SerializeField] public Transform lookAtObject { get; private set; }

    [field: SerializeField] public BoxCollider2D attackTrigger;

    [field: SerializeField] public EnemyVision Vision { get; private set; }
    [field: SerializeField] public bool isIdle = false;

    [field: SerializeField] public BoxCollider2D ValidnessCheckerTrigger { get; private set; }

    public delegate void D_OnAttack();
    public D_OnAttack D_onAttack;

    public delegate void D_OnHurt();
    public D_OnHurt D_onHurt;

    public delegate void D_OnRespawn();
    public D_OnRespawn D_onRespawn;

    public Action<PlayerCharacter> OnStartChasing;
    public Action OnStoppedChasing;

    [field: SerializeField] public SCRPT_ZombieAudio AudioData { get; private set; }

    [field:  SerializeField] public bool allowWander { get; private set; }

    [SerializeField] private float farTeleportationCooldown = 3;
    private float lastTeleportationAttemptTime;

    private int closePlayersCount = 0;

    [field: SerializeField] public bool tutorialZombie { get; private set; }

    [SerializeField] private float attack_DURATION = .3f;
    public float Attack_DURATION { get => attack_DURATION; }

    [SerializeField] private float push_COOLDOWN = .15f;
    private float push_TIMER;

    public bool attackStarted;

    [SerializeField, ReadOnly] private int maxReceivedStamp;

    public Vector2 AttackDirection { get; set; }

    [SerializeField] private float targetClosest_COOLDOWN = 3;
    private float targetClosest_TIMER;

    public bool isVisible;

    private static int currentlyChasingZombiesCount = 0;
    public static int CurrentlyChasingZombiesCOunt { get => currentlyChasingZombiesCount; }

    public static NormalZombie Create(Vector2 pos, bool seeAtStart, bool _allowScaleOnStamp = true)
    {
        NormalZombie res = Instantiate(GameAssets.Instance.GetRandomZombie, pos, Quaternion.identity).GetComponent<NormalZombie>();
        res.isIdle = !seeAtStart;
        res.Vision.targetPlayerAtStart = seeAtStart;
        if (!seeAtStart) res.ResetTarget();
        res.AllowScaleOnStamp = _allowScaleOnStamp;
        res.ReceiveStampModifiers();
        return res;
    }

    protected override void Start()
    {
        if (tutorialZombie && !GameManager.Instance.IsInTutorial) Destroy(this.gameObject);
        base.Start();
        Pathfinding?.StartUpdatePath();

        if (ZombiesScalingManager.Instance != null)
        ZombiesScalingManager.Instance.D_onSendModifiers += ReceiveStampModifiers;

        if (tutorialZombie || isIdle) return;

        targetClosest_TIMER = targetClosest_COOLDOWN;

        if (SpawnersManager.Instance != null)
            d_OnDeath += SpawnersManager.Instance.RemoveZombie;

        OnStartChasing += (PlayerCharacter target) => currentlyChasingZombiesCount++;
        OnStoppedChasing += () => currentlyChasingZombiesCount--;
    }

    protected override void Update()
    {
        base.Update();

        if (isIdle) return;

        if (targetClosest_TIMER > 0) targetClosest_TIMER-= Time.deltaTime;
        else
        {
            this.Vision.TargetClosestPlayer();
            targetClosest_TIMER = targetClosest_COOLDOWN;
        }

        if (CurrentPlayerTarget == null)
        {
            this.Vision.TargetClosestPlayer();
            return;
        }

        if (push_TIMER > 0) push_TIMER -= Time.deltaTime;
    }

    public override void SetSpeedOnDistanceFromTarget()
    {
        if (closePlayersCount > 0) return;

        float requiredDistance = MinDistanceForNormalSpeed + Camera.main.orthographicSize;
        float squaredReqDist = requiredDistance * requiredDistance;
        float distanceFromTarget = ((Vector2)this.transform.position - CurrentPositionTarget).sqrMagnitude;
        if (distanceFromTarget < squaredReqDist) return;

        if (Time.time - lastTeleportationAttemptTime > farTeleportationCooldown.Fluctuate())
        {
            lastTeleportationAttemptTime = Time.time;
            SpawnersManager.Instance.TeleportZombie(this);
        }
    }

    private void ReceiveStampModifiers()
    {
        if (!AllowScaleOnStamp) return; 
        if (maxReceivedStamp >= ZombiesScalingManager.Instance.ModifiersByStamps.Length) return;

        float speedAddition = 0;

        ZombiesScalingManager.S_ModifiersByStamp stampModifiers = ZombiesScalingManager.Instance.ModifiersByStamps[maxReceivedStamp];

        foreach (var item in stampModifiers.Modifiers)
        {
            this.AddModifier(item.ModifierID, item.Value, item.StatType);

            if (item.StatType == StatsModifier.E_StatType.Speed) speedAddition += item.Value;
        }

        float totalSpeedAddition = ZombiesScalingManager.Instance.TotalSpeedAddition;
        float maxSteeringMaxForce = ZombiesScalingManager.Instance.MaxSteeringMaxForce;
        float maxSteeringMass = ZombiesScalingManager.Instance.MaxSteeringMass;

        for (int i = 0; i < speedAddition; i++)
        {
            this.maxForce += (maxSteeringMaxForce - BaseMaxForce) / (totalSpeedAddition );
            this.movementMass -= (BaseMovementMass - maxSteeringMass) / (totalSpeedAddition );
        }

        stampModifiers.Actions?.Invoke(this);

        maxForce = Mathf.Clamp(maxForce, BaseMaxForce, maxSteeringMaxForce);
        movementMass = Mathf.Clamp(movementMass, maxSteeringMass, BaseMovementMass);

        D_receivedStampModifier?.Invoke();

        maxReceivedStamp++;
        int trueStamp = ZombiesScalingManager.Instance.CurrentStamp;

        if (maxReceivedStamp <= trueStamp) ReceiveStampModifiers();
    }

    public void ForceKill()
    {
        if (!IsAlive()) return;
        Death(true);
    }

    public override bool OnTakeDamages(float amount, Entity damager, bool isCrit = false, bool fakeDamages = false, bool callDelegate = true, bool tickDamages = false)
    {
        PlayerCharacter playerDamager = damager as PlayerCharacter;

        if (playerDamager != null)
        d_OnDeath += playerDamager.AddKillCount;

        bool res = base.OnTakeDamages(amount, damager, isCrit, fakeDamages, callDelegate);

        if (res) D_onHurt?.Invoke();

        if (playerDamager != null)
            d_OnDeath -= playerDamager.AddKillCount;

        return res;
    }

    public override void Death(bool forceDeath = false)
    {
        base.Death(forceDeath);

        BloodParticles.GetNext(this.PivotOffset.position);

        if (tutorialZombie)
        {
            Destroy(this.gameObject);
            return;
        }
        SpawnersManager.Enqueue(this);

        if (isIdle)
        {
            isIdle = false;
            if (SpawnersManager.Instance != null)
                d_OnDeath += SpawnersManager.Instance.RemoveZombie;
        }
        this.gameObject.SetActive(false);
        ResetAll();
    }

    public override bool IsAlive()
    {
        return base.IsAlive();
    }

    private void ResetAll()
    {
        StopAllCoroutines();
        LeanTween.cancel(this.gameObject);

        if (spineHolder != null) spineHolder.transform.localScale = Vector3.one;

        this.RemoveModifiersAll();
        this.RemoveAllTickDamages();
        this.ResetStats();
        this.ResetTarget();
        OnReset?.Invoke();
    }

    public void Reenable(Vector2 pos, bool addToSpawner = true)
    {
        this.transform.position = pos;
        this.ResetStats();
        this.stateManager.CheckStates();
        this.stateManager.SwitchState(FSM_NZ_Manager.E_NZState.Chasing);

        if (addToSpawner) SpawnersManager.Instance.AddZombie();

        if (this.skeletonAnimation != null)
        {
            this.skeletonAnimation.skeleton.SetColor(Color.white);
            this.skeletonAnimation.timeScale = 1;
        }

        this.gameObject.SetActive(true);
        this.Vision.TargetClosestPlayer();

        D_onRespawn?.Invoke();
    }

    public override void Stun(float duration, bool resetAttackTimer = false, bool showStuntext = false)
    {
        AskForStun(duration, resetAttackTimer, showStuntext);
    }

    public override void SetAttackedPlayer(PlayerCharacter target)
    {
        base.SetAttackedPlayer(target);

        Vector2 targetPosition = this.CurrentPlayerTarget == null ? this.storedTargetPosition : this.CurrentPlayerTarget.transform.position;
    }

    protected override void ResetStats()
    {
        this.MaxHP_M = GetStats.MaxHP.Fluctuate(.2f);
        this.MaxDamages_M = GetStats.BaseDamages.Fluctuate(.2f);

        this.MaxAttRange_M = GetStats.AttackRange.Fluctuate();

        this.MaxAttCD_M = GetStats.Attack_COOLDOWN.Fluctuate();

        this.MaxSpeed_M = GetStats.Speed.Fluctuate(.2f);

        this.MaxCritChances_M = GetStats.CritChances.Fluctuate();

        this.currentHP = MaxHP_M;
    }

    public void OnEnteredFarCheck()
    {
    }

    public void OnExitedFarCheck()
    {
    }

    public void OnEnteredCloseCheck()
    {
        if (isIdle)
        {
            this.Vision.TargetClosestPlayer();
            isIdle = false;
        }
        closePlayersCount++;
    }

    public void OnExitedCloseCheck()
    {
        closePlayersCount--;
    }
}

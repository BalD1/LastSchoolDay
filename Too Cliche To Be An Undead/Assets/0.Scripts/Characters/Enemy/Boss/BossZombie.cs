using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossZombie : EnemyBase
{
    [field: SerializeField] public FSM_Boss_Manager StateManager { get; private set; }

    [SerializeField] private SO_EnemyAttack[] attacks;

    [field: SerializeField] public Transform SkeletonHolder { get; private set; }

    [field: SerializeField] public Collider2D hudTrigger { get; private set; }

    [field: SerializeField] public BossAnimationsController AnimationController { get; private set; }
    [field: SerializeField] public SCRPT_BossAnimData AnimationData { get; private set; }

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

    [field: SerializeField]
    private StatsModifier[] secondStageModifiers = new StatsModifier[]
    {
        new StatsModifier("2NDSTAGE_DAMAGES", 5, StatsModifier.E_StatType.Damages),
    };

    [field: SerializeField] private float onSpawnAttackCooldown = 1.5f;

    [field: SerializeField] public bool IsAttacking = false;

    [field: SerializeField] public AudioClip OnHitSFX { get; private set; }

    [field: SerializeField] public AudioClip howl { get; private set; }
    [field: SerializeField] public AudioClip fall { get; private set; }

    public float recoverTimerModifier;

    public event Action<Entity, bool> onReceiveAttack;
    private void CallReceiveAttack(Entity damager, bool tickDamages) => onReceiveAttack?.Invoke(damager, tickDamages);

    public bool IsJumping { get; protected set; }

    public event Action OnJumpStarted;
    public void CallJumpStarted() => OnJumpStarted?.Invoke();
    public event Action OnJumpEnded;
    public void CallJumpEnded() => OnJumpStarted?.Invoke();

    public delegate void D_StartedAttack();
    public D_StartedAttack D_startedAttack;

    public delegate void D_CurrentAttackEnded();
    public D_CurrentAttackEnded D_currentAttackEnded;

    public Action OnAppearAnimationEnded;

    public event Action<FSM_Boss_Manager.E_BossState> OnBossStateChange;
    public void CallStateChange(FSM_Boss_Manager.E_BossState newState)
        => OnBossStateChange?.Invoke(newState);

    [field: SerializeField] public float HitStop_DURATION { get; private set; } = .3f;
    private float hitStop_TIMER;
    public float HitStop_TIMER { get => hitStop_TIMER; }

    private bool deathFlag = false;

    private List<Entity> spawnedZombies = new List<Entity>();
    public List<Entity> SpawnedZombies { get => spawnedZombies; }

    protected override void EventsSubscriber()
    {
        base.EventsSubscriber();
        OnJumpStarted += JumpStart;
        OnJumpEnded += JumpEnd;
        CinematicManagerEvents.OnChangeCinematicState += OnChangeCinematicState;
    }

    protected override void EventsUnSubscriber()
    {
        base.EventsUnSubscriber();
        OnJumpStarted -= JumpStart;
        OnJumpEnded -= JumpEnd;
        CinematicManagerEvents.OnChangeCinematicState -= OnChangeCinematicState;
    }

    private void JumpStart() => IsJumping = true;
    private void JumpEnd() => IsJumping = false;

    protected override void Awake()
    {
        base.Awake();
        D_startedAttack += () => IsAttacking = true;
        D_currentAttackEnded += () => IsAttacking = false;
    }

    public void Setup(bool withCinematic)
    {
        if (!withCinematic) OnStart();
        else
        {
            OnAppearAnimationEnded += OnAppearCinematicEnded;

            AnimationController.SetSkeletonAlpha(0);
            this.StateManager.SwitchState(FSM_Boss_Manager.E_BossState.Appear);
        }
    }

    private void OnAppearCinematicEnded()
    {
        OnAppearAnimationEnded -= OnAppearCinematicEnded;
        this.BossSpawn();
    }

    private void OnStart()
    {
        this.BossSpawn();
        //attack_TIMER = onSpawnAttackCooldown;
        this.StateManager.SwitchState(FSM_Boss_Manager.E_BossState.Chasing);
        TargetClosestPlayer();
    }

    private void OnChangeCinematicState(bool isInCinematic)
    {
        if (!isInCinematic)
        {
            this.StateManager.SwitchState(FSM_Boss_Manager.E_BossState.Chasing);
            TargetClosestPlayer();
        }
    }

    public void OnHitEntity(Entity e, bool hitStopSelf = true)
    {
        //Vector2 baseVel = this.GetRb.velocity;
        //this.GetRb.velocity = Vector2.zero;

        //e.InflinctDamages(this.MaxDamages_M, this, this.RollCrit());
        //HitStop(e);

        //foreach (var item in spawnedZombies)
        //{
        //    HitStop(item, false);
        //}

        //void HitStop(Entity e, bool pushAtEnd = true)
        //{
        //    if (e is PlayerCharacter) (e as PlayerCharacter).SetTimedInvincibility(HitStop_DURATION);
        //    e.Stun(this.HitStop_DURATION);
        //    e.SkeletonAnimation.timeScale = 0;
        //    LeanTween.delayedCall(this.HitStop_DURATION, () =>
        //    {
        //        e.SkeletonAnimation.timeScale = 1;
        //        if (pushAtEnd)
        //            e.AskPush(10, this, this);
        //        this.GetRb.velocity = baseVel;
        //    });
        //}

        LeanTween.delayedCall(this.HitStop_DURATION, () =>
        {
            //this.GetRb.velocity = baseVel;
        });

        BossHitFX.GetNext(e.transform.position);
        AudioclipPlayer.Create(this.transform.position, OnHitSFX);

        if (hitStopSelf) this.HitStop(this.HitStop_DURATION);
    }

    private void AdvanceToNextStage()
    {
        attacksPatern.NextStage();
        foreach (var item in secondStageModifiers)
        {
            //AddModifier(item.IDName, item.Modifier, item.StatType);
        }
    }

    //public override void Death(bool forceDeath = false)
    //{
    //    if (deathFlag) return;
    //    deathFlag = true;
    //    this.BossDeath();
    //    CallOnDeath();
    //}

    public override void Stun(float duration, bool resetAttackTimer = false, bool showStuntext = false)
    {
        //this.AddModifierUnique("SLOW", -1, duration, StatsModifier.E_StatType.Speed);
    }

    public void TargetClosestPlayer()
    {
        PlayerCharacter closerTarget = null;
        float closerDistance = float.MaxValue;
        foreach (var item in IGPlayersManager.Instance.PlayersList)
        {
            float currentDist = Vector2.Distance(this.transform.position, item.transform.position);
            if (currentDist < closerDistance)
            {
                closerDistance = currentDist;
                closerTarget = item;
            }
        }
        //this.SetTarget(closerTarget);
    }

    public void OnMinionSpawned(Entity minion) => spawnedZombies.Add(minion);
    public void OnMinionDied(Entity minion)
    {
        //minion.D_onDeathOf -= OnMinionDied;

        //if (this.IsAlive())
        //    spawnedZombies.Remove(minion);
    }

    public void HitStop(float duration)
    {
        //this.SkeletonAnimation.timeScale = 0;
        hitStop_TIMER = duration;
    }
}

using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZombie : EnemyBase
{
    [SerializeField] private FSM_Boss_Manager stateManager;

    [SerializeField] private SCRPT_EnemyAttack[] attacks;

    [field: SerializeField] public Transform SkeletonHolder { get; private set; }

    [field: SerializeField] public Collider2D hudTrigger;

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

    [field: SerializeField]
    private StatsModifier[] secondStageModifiers = new StatsModifier[]
    {
        new StatsModifier("2NDSTAGE_DAMAGES", 5, StatsModifier.E_StatType.Damages),
    };

    [field: SerializeField] public bool isAppeared { get; private set; }

    [field: SerializeField] private float onSpawnAttackCooldown = 1.5f;

    [field: SerializeField] public bool IsAttacking = false;

    public float recoverTimerModifier;

    public delegate void D_StartedAttack();
    public D_StartedAttack D_startedAttack;

    public delegate void D_CurrentAttackEnded();
    public D_CurrentAttackEnded D_currentAttackEnded;

    private bool deathFlag = false;

    protected override void Awake()
    {
        base.Awake();
        D_startedAttack += () => IsAttacking = true;
        D_currentAttackEnded += () => IsAttacking = false;
    }

    protected override void Start()
    {
        if (isAppeared) OnStart();
        else
        {
            Skeleton sk = skeletonAnimation.skeleton;

            Color goal = sk.GetColor();
            goal.a = 0;

            sk.SetColor(goal);
        }
    }

    private void OnStart()
    {
        attack_TIMER = onSpawnAttackCooldown;
        TargetClosestPlayer();
        base.Start();
        Pathfinding?.StartUpdatePath();

        GameManager.Instance.D_bossFightStarted?.Invoke();
    }

    protected override void Update()
    {
        if (!isAppeared) return;
        if (!IsAlive()) return;

        base.Update();
    }

    public override bool OnTakeDamages(float amount, Entity damager, bool isCrit = false, bool fakeDamages = false, bool callDelegate = true, bool tickDamages = false)
    {
        if (deathFlag)
        {
            StartCoroutine(MaterialFlash());
            return false;
        }

        bool res = base.OnTakeDamages(amount, damager, isCrit, fakeDamages, callDelegate);

        if (this.currentHP <= (this.MaxHP_M * hpThresholdBeforeNextStage) && attacksPatern.currentStage == 0) AdvanceToNextStage();

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
        if (deathFlag) return;

        deathFlag = true;

        stateManager.SwitchState(stateManager.deadState);

        d_OnDeath?.Invoke();
        GameManager.Instance.D_bossFightEnded?.Invoke();

        SoundManager.Instance.ChangeMusicMixerPitch(1);
        UIManager.Instance.RemoveBossCollider(this.hudTrigger);
    }

    public override void Stun(float duration, bool resetAttackTimer = false, bool showStuntext = false)
    {
        this.AddModifierUnique("SLOW", -1, duration, StatsModifier.E_StatType.Speed);
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

    public void PlayAppearAnimation(Action actionToPlayAtEnd)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, animationData.JumpStartAnim, false);

        this.SkeletonHolder.AddToLocalPositionY(5);
        Skeleton sk = skeletonAnimation.skeleton;

        Color goalColor = sk.GetColor();
        goalColor.a = 1;
        LeanTween.value(this.SkeletonAnimation.gameObject, sk.GetColor(), goalColor, .25f).setOnUpdate((Color c) =>
        {
            sk.SetColor(c);
        }).setIgnoreTimeScale(true);

        LeanTween.delayedCall(.4f, () =>
        {
            skeletonAnimation.AnimationState.SetAnimation(0, animationData.JumpEndAnim, false);
            skeletonAnimation.AnimationState.AddAnimation(0, animationData.IdleAnim, true, animationData.JumpEndAnim.Animation.Duration + .2f);
        });
        LeanTween.moveLocalY(this.SkeletonHolder.gameObject, 0, .5f).setOnComplete(() =>
        {
            this.SkeletonHolder.SetLocalPositionY(0);
            sk.SetColor(goalColor);

            float cameraShakeDuration = 2;

            SoundManager.Instance.PlayBossMusic();

            CameraManager.Instance.ShakeCamera(2.5f, cameraShakeDuration);

            LeanTween.delayedCall(cameraShakeDuration, () =>
            {
                actionToPlayAtEnd?.Invoke();
                skeletonAnimation.AnimationState.SetAnimation(0, animationData.YellAnim, false);
                skeletonAnimation.AnimationState.AddAnimation(0, animationData.IdleAnim, true, animationData.YellAnim.Animation.Duration + .2f);

                BossHUDManager.Instance.AddBoss(this);
                BossHUDManager.Instance.LeanContainer(true);

                CameraManager.Instance.ZoomCamera(-.25f, .5f, () =>
                {
                    CameraManager.Instance.ShakeCamera(2.5f, 1);
                });
            });

            
        }).setIgnoreTimeScale(true);
    }

    public void SetIsAppeared()
    {
        isAppeared = true;
        OnStart();
        stateManager.OnStart();
        UIManager.Instance.AddBossCollider(this.hudTrigger);
    }

}

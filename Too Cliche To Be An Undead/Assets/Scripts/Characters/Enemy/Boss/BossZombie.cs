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

    private bool deathFlag = false;

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
    }

    protected override void Update()
    {
        if (!isAppeared) return;
        if (!IsAlive()) return;

        base.Update();
    }

    public override bool OnTakeDamages(float amount, Entity damager, bool isCrit = false, bool fakeDamages = false, bool callDelegate = true)
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
        d_OnDeath?.Invoke();
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
    }

}

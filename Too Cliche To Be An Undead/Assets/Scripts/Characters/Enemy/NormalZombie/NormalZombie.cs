using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombie : EnemyBase
{
    [SerializeField] private FSM_NZ_Manager stateManager;

    [field: SerializeField] public GameObject spineHolder { get; private set; }
    [field: SerializeField] public ZombiesAnimationController animationController { get; private set; }
    [field: SerializeField] public SCRPT_ZombiesAnimDatga animationData { get; private set; }

    [field: SerializeField] public Transform lookAtObject { get; private set; }

    [field: SerializeField] public BoxCollider2D attackTrigger;

    [field: SerializeField] public EnemyVision Vision { get; private set; }
    [field: SerializeField] public bool isIdle = false;

    public delegate void D_OnAttack();
    public D_OnAttack D_onAttack;

    public delegate void D_OnHurt();
    public D_OnHurt D_onHurt;

    public delegate void D_OnRespawn();
    public D_OnRespawn D_onRespawn;

    [field: SerializeField] public SCRPT_ZombieAudio AudioData { get; private set; }

    [field:  SerializeField] public bool allowWander { get; private set; }

    [field: SerializeField] public bool tutorialZombie { get; private set; }

    [SerializeField] private float attack_DURATION = .3f;
    public float Attack_DURATION { get => attack_DURATION; }

    public bool attackStarted;

    public Vector2 AttackDirection { get; set; }

    public const int maxDistanceFromPlayer = 15;

    [SerializeField] private float targetClosest_COOLDOWN = 3;
    private float targetClosest_TIMER;

    [field: SerializeField] public float timeOfDeath;

    public bool isVisible;

    public static NormalZombie Create(Vector2 pos, bool seeAtStart)
    {
        NormalZombie res = Instantiate(GameAssets.Instance.GetRandomZombie, pos, Quaternion.identity).GetComponent<NormalZombie>();

        res.isIdle = !seeAtStart;
        res.Vision.targetPlayerAtStart = seeAtStart;

        if (!seeAtStart) res.ResetTarget();

        return res;
    }

    protected override void Start()
    {
        base.Start();
        Pathfinding?.StartUpdatePath();

        ZombiesScalingManager.Instance.D_onSendModifiers += ReceiveStampModifiers;

        if (tutorialZombie || isIdle) return;

        SpawnersManager.Instance.AddZombie();

        targetClosest_TIMER = targetClosest_COOLDOWN;

        d_OnDeath += SpawnersManager.Instance.RemoveZombie;
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

        if (Vector2.Distance(this.transform.position, CurrentPlayerTarget.transform.position) > maxDistanceFromPlayer) ForceKill();
    }

    private void ReceiveStampModifiers(List<ZombiesScalingManager.S_ModifierData> modifiers)
    {
        foreach (var item in modifiers)
        {
            this.AddModifier(item.ModifierID, item.Value, item.StatType);
        }
    }

    public void ForceKill()
    {
        if (isIdle) return;

        attackedPlayer?.RemoveAttacker(this);
        this.gameObject.SetActive(false);
        SpawnersManager.Instance.TeleportZombie(this);
        ResetAll();

    }

    public override bool OnTakeDamages(float amount, Entity damager, bool isCrit = false, bool fakeDamages = false, bool callDelegate = true, bool tickDamages = false)
    {
        d_OnDeath += (damager as PlayerCharacter).AddKillCount;

        bool res = base.OnTakeDamages(amount, damager, isCrit, fakeDamages, callDelegate);

        if (res) D_onHurt?.Invoke();

        d_OnDeath -= (damager as PlayerCharacter).AddKillCount;

        return res;
    }

    public override void OnDeath(bool forceDeath = false)
    {
        base.OnDeath(forceDeath);

        if (tutorialZombie)
        {
            Destroy(this.gameObject);
            return;
        }
        SpawnersManager.Instance.ZombiesPool.Enqueue(this);
        timeOfDeath = Time.timeSinceLevelLoad;

        if (isIdle)
        {
            isIdle = false;
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
    }

    public void Reenable(Vector2 pos, bool addToSpawner = true)
    {
        this.transform.position = pos;
        this.ResetStats();
        this.stateManager.SwitchState(stateManager.chasingState);

        if (addToSpawner) SpawnersManager.Instance.AddZombie();

        if (this.skeletonAnimation != null)
            this.skeletonAnimation.skeleton.SetColor(Color.white);

        this.gameObject.SetActive(true);
        this.Vision.TargetClosestPlayer();

        D_onRespawn?.Invoke();
    }

    public override void Stun(float duration, bool resetAttackTimer = false, bool showStuntext = false)
    {
        if (showStuntext)
            TextPopup.Create("Stun !", this.GetHealthPopupOffset + (Vector2)this.transform.position, GameAssets.StunComponents);
        stateManager.SwitchState(stateManager.stunnedState.SetDuration(duration, resetAttackTimer));
        this.attackTelegraph.CancelTelegraph();
    }

    public override Vector2 Push(Vector2 pusherPosition, float pusherForce, Entity originalPusher)
    {
        if (!canBePushed) return Vector2.zero;

        Vector2 v = base.Push(pusherPosition, pusherForce, originalPusher);

        if (v.magnitude <= Vector2.zero.magnitude) return Vector2.zero;

        stateManager.SwitchState(stateManager.pushedState.SetForce(v, originalPusher));

        return v;
    }

    public override void SetAttackedPlayer(PlayerCharacter target)
    {
        base.SetAttackedPlayer(target);

        Vector2 targetPosition = this.CurrentPlayerTarget == null ? this.storedTargetPosition : this.CurrentPlayerTarget.transform.position;
    }
}

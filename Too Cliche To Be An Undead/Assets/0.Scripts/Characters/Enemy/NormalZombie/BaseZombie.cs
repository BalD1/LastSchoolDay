using Spine.Unity;
using System;
using UnityEngine;

public class BaseZombie : EnemyBase, IDistanceChecker
{
    //[SerializeField] private FSM_NZ_Manager stateManager;
    //public FSM_NZ_Manager StateManager { get => stateManager; }

    //[field: SerializeField] public GameObject spineHolder { get; private set; }
    //[field: SerializeField] public ZombiesAnimationController animationController { get; private set; }
    //[field: SerializeField] public SCRPT_ZombiesAnimData AnimationData { get; private set; }

    //[field: SerializeField] public Transform lookAtObject { get; private set; }

    //[field: SerializeField] public BoxCollider2D attackTrigger;

    //[field: SerializeField] public EnemyVision Vision { get; private set; }
    //[field: SerializeField] public bool isIdle = false;

    //[field: SerializeField] public BoxCollider2D ValidnessCheckerTrigger { get; private set; }

    //public delegate void D_OnAttack();
    //public D_OnAttack D_onAttack;

    //public delegate void D_OnHurt();
    //public D_OnHurt D_onHurt;

    //public delegate void D_OnRespawn();
    //public D_OnRespawn D_onRespawn;

    //public Action<PlayerCharacter> OnStartChasing;
    //public Action OnStoppedChasing;

    //[field: SerializeField] public SCRPT_ZombieAudio AudioData { get; private set; }

    //[field:  SerializeField] public bool allowWander { get; private set; }

    //[SerializeField] private float farTeleportationCooldown = 3;
    //private float lastTeleportationAttemptTime;

    //private int closePlayersCount = 0;

    //[field: SerializeField] public bool tutorialZombie { get; private set; }

    //[SerializeField] private float attack_DURATION = .3f;
    //public float Attack_DURATION { get => attack_DURATION; }

    //[SerializeField] private float push_COOLDOWN = .15f;
    //private float push_TIMER;

    //public bool attackStarted;

    //[SerializeField, ReadOnly] private int maxReceivedStamp;

    //public event Action<FSM_NZ_Manager.E_NZState> OnZombieStateChange;
    //public void CallStateChange(FSM_NZ_Manager.E_NZState newState)
    //    => OnZombieStateChange?.Invoke(newState);

    //public Vector2 AttackDirection { get; set; }

    //[SerializeField] private float targetClosest_COOLDOWN = 3;
    //private float targetClosest_TIMER;

    //public bool isVisible;

    //private static int currentlyChasingZombiesCount = 0;
    //public static int CurrentlyChasingZombiesCOunt { get => currentlyChasingZombiesCount; }

    //public static BaseZombie Create(Vector2 pos, bool seeAtStart, bool _allowScaleOnStamp = true)
    //{
    //    BaseZombie res = Instantiate(GameAssets.Instance.GetRandomZombie, pos, Quaternion.identity).GetComponent<BaseZombie>();
    //    res.isIdle = !seeAtStart;
    //    res.Vision.targetPlayerAtStart = seeAtStart;
    //    if (!seeAtStart) res.ResetTarget();
    //    res.AllowScaleOnStamp = _allowScaleOnStamp;
    //    return res;
    //}

    //protected override void EventsSubscriber()
    //{
    //    base.EventsSubscriber();
    //    GymnasiumCinematicEvents.OnGymnasiumCinematicStarted += ForceKill;
    //}

    //protected override void EventsUnSubscriber()
    //{
    //    base.EventsUnSubscriber();
    //    GymnasiumCinematicEvents.OnGymnasiumCinematicStarted -= ForceKill;
    //}

    //protected override void Start()
    //{
    //    base.Start();
    //    if (tutorialZombie && !DataKeeper.StartInTutorial()) Destroy(this.gameObject);
    //    Pathfinding?.StartUpdatePath();

    //    if (tutorialZombie || isIdle) return;

    //    targetClosest_TIMER = targetClosest_COOLDOWN;

    //    if (SpawnersManager.Instance != null)
    //        OnDeath += SpawnersManager.Instance.RemoveZombie;

    //    OnStartChasing += (PlayerCharacter target) => currentlyChasingZombiesCount++;
    //    OnStoppedChasing += () => currentlyChasingZombiesCount--;
    //}

    //protected override void Update()
    //{
    //    base.Update();

    //    if (isIdle) return;

    //    if (targetClosest_TIMER > 0) targetClosest_TIMER-= Time.deltaTime;
    //    else
    //    {
    //        this.Vision.TargetClosestPlayer();
    //        targetClosest_TIMER = targetClosest_COOLDOWN;
    //    }

    //    if (CurrentPlayerTarget == null)
    //    {
    //        this.Vision.TargetClosestPlayer();
    //        return;
    //    }

    //    if (push_TIMER > 0) push_TIMER -= Time.deltaTime;
    //}

    //public override void SetSpeedOnDistanceFromTarget()
    //{
    //    if (closePlayersCount > 0) return;

    //    float requiredDistance = MinDistanceForNormalSpeed + Camera.main.orthographicSize;
    //    float squaredReqDist = requiredDistance * requiredDistance;
    //    float distanceFromTarget = ((Vector2)this.transform.position - CurrentPositionTarget).sqrMagnitude;
    //    if (distanceFromTarget < squaredReqDist) return;

    //    if (Time.time - lastTeleportationAttemptTime > farTeleportationCooldown.Fluctuate())
    //    {
    //        lastTeleportationAttemptTime = Time.time;
    //        SpawnersManager.Instance.TeleportZombie(this);
    //    }
    //}

    //public void ForceKill()
    //{
    //    EntityHealthSystem.Kill();
    //}

    ////public override void Death(bool forceDeath = false)
    ////{
    ////    base.Death(forceDeath);

    ////    BloodParticles.GetNext(this.PivotOffset.position);

    ////    if (tutorialZombie)
    ////    {
    ////        Destroy(this.gameObject);
    ////        return;
    ////    }
    ////    SpawnersManager.Enqueue(this);

    ////    if (isIdle)
    ////    {
    ////        isIdle = false;
    ////        if (SpawnersManager.Instance != null)
    ////            OnDeath += SpawnersManager.Instance.RemoveZombie;
    ////    }
    ////    this.gameObject.SetActive(false);
    ////}

    //public void Reenable(Vector2 pos, bool addToSpawner = true)
    //{
    //    base.ResetEntity();
    //    this.transform.position = pos;
    //    this.stateManager.CheckStates();
    //    this.stateManager.SwitchState(FSM_NZ_Manager.E_NZState.Chasing);

    //    if (addToSpawner) SpawnersManager.Instance.AddZombie();

    //    if (this.skeletonAnimation != null)
    //    {
    //        this.skeletonAnimation.skeleton.SetColor(Color.white);
    //        this.skeletonAnimation.timeScale = 1;
    //    }

    //    this.gameObject.SetActive(true);
    //    this.Vision.TargetClosestPlayer();

    //    D_onRespawn?.Invoke();
    //}

    //public override void Stun(float duration, bool resetAttackTimer = false, bool showStuntext = false)
    //{
    //    AskForStun(duration, resetAttackTimer, showStuntext);
    //}

    //public override void SetAttackedPlayer(PlayerCharacter target)
    //{
    //    base.SetAttackedPlayer(target);

    //    Vector2 targetPosition = this.CurrentPlayerTarget == null ? this.storedTargetPosition : this.CurrentPlayerTarget.transform.position;
    //}

    //public void OnEnteredFarCheck()
    //{
    //}

    //public void OnExitedFarCheck()
    //{
    //}

    //public void OnEnteredCloseCheck()
    //{
    //    if (isIdle)
    //    {
    //        this.Vision.TargetClosestPlayer();
    //        isIdle = false;
    //    }
    //    closePlayersCount++;
    //}

    //public void OnExitedCloseCheck()
    //{
    //    closePlayersCount--;
    //}
    public void OnEnteredCloseCheck()
    {
        throw new NotImplementedException();
    }

    public void OnEnteredFarCheck()
    {
        throw new NotImplementedException();
    }

    public void OnExitedCloseCheck()
    {
        throw new NotImplementedException();
    }

    public void OnExitedFarCheck()
    {
        throw new NotImplementedException();
    }
}

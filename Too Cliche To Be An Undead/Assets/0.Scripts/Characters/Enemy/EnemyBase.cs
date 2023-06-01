using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BalDUtilities.VectorUtils;

public abstract class EnemyBase : Entity
{
    [Header("Base - Enemy")]

    [Header("Components")]

    [SerializeField] private SCRPT_DropTable dropTable;

    [SerializeField] private SCRPT_EnemyAttack[] attack;
    public SCRPT_EnemyAttack[] AttacksArray { get => attack; }
    public SCRPT_EnemyAttack Attack { get => attack.RandomElement(); }

    [SerializeField] private EnemyPathfinding pathfinding;
    public EnemyPathfinding Pathfinding { get => pathfinding; }

    [field: SerializeField] public Collider2D enemiesBlocker;

    [field: SerializeField] public AttackTelegraph attackTelegraph { get; private set; }

    [SerializeField] private Material attackMaterial;
    public Material AttackMaterial { get => attackMaterial; }

    [SerializeField] private LayerMask wallsMask;

    [SerializeField] private Transform pivotOffset;
    public Transform PivotOffset { get => pivotOffset; }

    [Header("Stats", order = 0)]

    [SerializeField] private float speedMultiplier;
    public float SpeedMultiplier { get => speedMultiplier; }

    [SerializeField] protected float maxForce;
    public float MaxForce { get => maxForce; }
    [field: SerializeField, ReadOnly] public float BaseMaxForce { get; protected set; }

    [SerializeField] protected float movementMass;
    public float MovementMass { get => movementMass; }
    [field: SerializeField, ReadOnly] public float BaseMovementMass { get; protected set; }

    private Vector2 steeredVelocity;

    [SerializeField] private bool allowSlowdown = true;

    public float MaxSpeed { get => this.MaxSpeed_M * this.SpeedMultiplier; }

    [SerializeField] private float randomWanderPositionRadius = 5f;
    public float RandomWanderPositionRadius { get => randomWanderPositionRadius; }

    [SerializeField] private float minWaitBeforeNewWanderPoint = 1f;
    public float MinWaitBeforeNewWanderPoint { get => minWaitBeforeNewWanderPoint; }

    [SerializeField] private float maxWaitBeforeNewWanderPoint = 3f;
    public float MaxWaitBeforeNewWanderPoint { get => maxWaitBeforeNewWanderPoint; }

    [SerializeField] private float distanceBeforeStop = 1f;
    public float DistanceBeforeStop { get => distanceBeforeStop; }

    public float MinDurationBeforeAttack { get => Attack.MinDurationBeforeAttack; }

    public float MaxDurationBeforeAttack { get => Attack.MaxDurationBeforeAttack; }

    [SerializeField] private Vector2 maxScaleOnAttack = new Vector2(1.3f, 1.3f);
    [SerializeField] private LeanTweenType inType = LeanTweenType.easeInSine;
    [SerializeField] private LeanTweenType outType = LeanTweenType.easeOutSine;

    public Vector2 MaxScaleOnAttack { get => maxScaleOnAttack; }
    public LeanTweenType InType { get => inType; }
    public LeanTweenType OutType { get => outType; }

    [Header("Player Related")]


    [SerializeField] protected PlayerCharacter attackedPlayer;
    public PlayerCharacter AttackedPlayer { get => attackedPlayer; }

    [SerializeField] private PlayerCharacter currentPlayerTarget;
    [SerializeField] private Transform currentTransformTarget;
    public PlayerCharacter CurrentPlayerTarget { get => currentPlayerTarget; }
    public Transform CurrentTransformTarget { get => currentPlayerTarget == null ? currentTransformTarget : currentPlayerTarget.transform; }
    public Vector2 CurrentPositionTarget { get => CurrentTransformTarget == null ? storedTargetPosition : CurrentTransformTarget.position; }
    public Vector2 storedTargetPosition;

    public delegate void D_LostPlayer();
    public D_LostPlayer D_lostPlayer;

    public delegate void D_ReceivedStampModifier();
    public D_ReceivedStampModifier D_receivedStampModifier;

    [field: SerializeField] public bool AllowScaleOnStamp { get; protected set; }

    [Header("Misc")]

#if UNITY_EDITOR
    public string currentStateDebug = "N/A";
#endif

    private Vector2 basePosition;
    public Vector2 BasePosition { get => basePosition; }

    private Vector2 desiredVelocity;
    private Vector2 steering;

    protected override void Awake()
    {
        base.Awake();
        basePosition = this.transform.position;

        BaseMaxForce = maxForce;
        BaseMovementMass = movementMass;
    }

    protected override void Start()
    {
        base.Start();

        distanceBeforeStop = distanceBeforeStop.Fluctuate();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void Movements(Vector2 goalPosition, bool slowdownOnApproach = true)
    {
        desiredVelocity = goalPosition * MaxSpeed;

        steering = desiredVelocity - steeredVelocity;
        steering = Vector3.ClampMagnitude(steering, this.MaxForce);
        if (this.MovementMass != 0)
            steering /= this.MovementMass;

        void SteerVelocity(float multiplier = 1)
        {
            steeredVelocity = Vector3.ClampMagnitude(steeredVelocity + steering, MaxSpeed) * multiplier;
        }

        bool isTargetIdle = currentPlayerTarget != null && currentPlayerTarget.Velocity == Vector2.zero;
        if ((slowdownOnApproach && allowSlowdown) && isTargetIdle)
        {
            float distance = Vector2.Distance(this.transform.position, CurrentPositionTarget);

            if (distance < distanceBeforeStop) SteerVelocity(distance / distanceBeforeStop);
            else SteerVelocity();
        }
        else SteerVelocity();

        this.GetRb.velocity = steeredVelocity * Time.fixedDeltaTime;
    }

    public void ChooseRandomPosition()
    {
        Vector2 randDir = (Random.insideUnitCircle * this.transform.position).normalized;
        float randDist = Random.Range(distanceBeforeStop, randomWanderPositionRadius);

        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, randDir, randDist, wallsMask);

        Vector2 point = this.transform.position;

        point = hit.point != Vector2.zero ? hit.point :
                             (Vector2)this.transform.position + randDir * randDist;

        SetTarget(point);
    }

    public void ResetVelocity()
    {
        steeredVelocity = Vector2.zero;
    }

    public virtual void SetAttackedPlayer(PlayerCharacter target)
    {
        if (target == null) return;
        if (target.IsAlive() == false) return;

        if (target.AddAttacker(this))
            attackedPlayer = target;
    }

    public void UnsetAttackedPlayer()
    {
        if (attackedPlayer == null) return;
        attackedPlayer.RemoveAttacker(this);
        attackedPlayer = null;
    }

    public void SetTarget(Vector2 target)
    {
        storedTargetPosition = target;
    }
    public void SetTarget(Transform target)
    {
        currentTransformTarget = target;
    }
    public void SetTarget(PlayerCharacter playerTarget)
    {
        currentPlayerTarget = playerTarget;

        if (playerTarget == null) currentTransformTarget = null;
        else currentTransformTarget = playerTarget.transform;
    }
    public void ResetTarget()
    {
        currentPlayerTarget = null;
        currentTransformTarget = null;
    }

    public override void OnDeath(bool forceDeath = false)
    {
        base.OnDeath(forceDeath);

        if (dropTable != null && !forceDeath)
        {
            dropTable.DropRandom(this.transform.position);
        }

        attackedPlayer?.RemoveAttacker(this);
    }

    public void SetAsZombifiedPlayer(Sprite playerSprite, float playerMaxHP, float playerDamages, float playerSpeed, int playerCrits)
    {
        this.MaxHP_M = playerMaxHP;
        this.OnHeal(playerMaxHP);
        this.MaxDamages_M = playerDamages;
        this.MaxSpeed_M = playerSpeed;
        this.MaxCritChances_M = playerCrits;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
         d_EnteredCollider?.Invoke(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        d_ExitedCollider?.Invoke(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
            d_EnteredTrigger?.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
            d_ExitedTrigger?.Invoke(collision);
    }

    protected override void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!debugMode) return;
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(this.transform.position, distanceBeforeStop);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(this.transform.position, randomWanderPositionRadius);
        Gizmos.color = Color.white;

        //Gizmos.DrawWireSphere()
#endif
    }
}

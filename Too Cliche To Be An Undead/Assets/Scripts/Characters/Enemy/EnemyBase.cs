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

    [field: SerializeField] public BoxCollider2D enemiesBlocker;

    [field: SerializeField] public AttackTelegraph attackTelegraph { get; private set; }

    [SerializeField] private Material attackMaterial;
    public Material AttackMaterial { get => attackMaterial; }

    [SerializeField] private LayerMask wallsMask;

    [SerializeField] private Transform pivotOffset;
    public Transform PivotOffset { get => pivotOffset; }

    [Header("Stats", order = 0)]

    [SerializeField] private float speedMultiplier;
    public float SpeedMultiplier { get => speedMultiplier; }

    public float speedMultiplierWhenOutsideOfCamera = 1;

    [SerializeField] private float maxForce;
    public float MaxForce { get => maxForce; }

    [SerializeField] private float movementMass;
    public float MovementMass { get => movementMass; }

    private Vector2 steeredVelocity;

    [SerializeField] private bool allowSlowdown = true;

    public float MaxSpeed { get => this.maxSpeed_M * this.SpeedMultiplier * speedMultiplierWhenOutsideOfCamera; }

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
    }

    protected override void Start()
    {
        base.Start();
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
        
        float distance = Vector2.Distance(this.transform.position, CurrentPositionTarget);
        if (distance < distanceBeforeStop && slowdownOnApproach && allowSlowdown) 
            steeredVelocity = Vector3.ClampMagnitude(steeredVelocity + steering, MaxSpeed) * (distance / distanceBeforeStop);
        else 
            steeredVelocity = Vector3.ClampMagnitude(steeredVelocity + steering, MaxSpeed);
        
        this.GetRb.velocity =  steeredVelocity * Time.fixedDeltaTime;
        //this.GetRb.velocity = goalPosition * this.GetStats.Speed(this.StatsModifiers) * this.SpeedMultiplier * Time.fixedDeltaTime;
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
        currentTransformTarget = playerTarget.transform;
    }
    public void ResetTarget()
    {
        currentPlayerTarget = null;
        currentTransformTarget = null;
    }

    public override void OnDeath(bool forceDeath = false)
    {
        base.OnDeath(forceDeath);

        if (dropTable != null)
        {
            dropTable.DropRandom(this.transform.position);
        }

        attackedPlayer?.RemoveAttacker(this);
    }

    public void SetAsZombifiedPlayer(Sprite playerSprite, float playerMaxHP, float playerDamages, float playerSpeed, int playerCrits)
    {
        this.sprite.sprite = playerSprite;
        this.sprite.color = Color.green;
        this.maxHP_M = playerMaxHP;
        this.OnHeal(playerMaxHP);
        this.maxDamages_M = playerDamages;
        this.maxSpeed_M = playerSpeed;
        this.maxCritChances_M = playerCrits;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
            d_EnteredCollider?.Invoke(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
            d_EnteredTrigger?.Invoke(collision);
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

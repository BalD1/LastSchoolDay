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

    [SerializeField] private SCRPT_NZ_Attack attack;
    public SCRPT_NZ_Attack Attack { get => attack; }

    [SerializeField] private EnemyPathfinding pathfinding;
    public EnemyPathfinding Pathfinding { get => pathfinding; }

    [SerializeField] private Material attackMaterial;
    public Material AttackMaterial { get => attackMaterial; }

    [Header("Stats", order = 0)]

    [SerializeField] private float speedMultiplier;
    public float SpeedMultiplier { get => speedMultiplier; }

    [SerializeField] private float maxForce;
    public float MaxForce { get => maxForce; }

    [SerializeField] private float movementMass;
    public float MovementMass { get => movementMass; }

    private Vector2 velocity;

    public float MaxSpeed { get => this.GetStats.Speed(this.StatsModifiers) * this.SpeedMultiplier; }

    [SerializeField] private float distanceBeforeStop = 1f;
    public float DistanceBeforeStop { get => distanceBeforeStop; }

    [SerializeField] private float durationBeforeAttack = .3f;
    public float DurationBeforeAttack { get => durationBeforeAttack; }
    

    [Header("Player Related")]

    [SerializeField] private List<PlayerCharacter> detectedPlayers;

    [SerializeField] private PlayerCharacter attackedPlayer;
    public PlayerCharacter AttackedPlayer { get => attackedPlayer; }

    [SerializeField] private PlayerCharacter currentPlayerTarget;
    [SerializeField] private Transform currentTransformTarget;
    public PlayerCharacter CurrentPlayerTarget { get => currentPlayerTarget; }
    public Transform CurrentTransformTarget { get => currentPlayerTarget == null ? currentTransformTarget : currentPlayerTarget.transform; }
    public Vector2 CurrentPositionTarget { get => CurrentTransformTarget == null ? storedTargetPosition : CurrentTransformTarget.position; }
    public List<PlayerCharacter> DetectedPlayers { get => detectedPlayers; }
    public Vector2 storedTargetPosition;

    public delegate void D_DetectedPlayer();
    public D_DetectedPlayer D_detectedPlayer;

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

    public void Movements(Vector2 goalPosition)
    {
        desiredVelocity = goalPosition * MaxSpeed;

        steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, this.MaxForce);
        steering /= this.MovementMass;

        velocity = Vector3.ClampMagnitude(velocity + steering, MaxSpeed);
        this.GetRb.velocity =  velocity * Time.fixedDeltaTime;
        //this.GetRb.velocity = goalPosition * this.GetStats.Speed(this.StatsModifiers) * this.SpeedMultiplier * Time.fixedDeltaTime;

        Debug.DrawRay(transform.position, this.GetRb.velocity.normalized * 2, Color.blue);
        Debug.DrawRay(transform.position * 2, desiredVelocity.normalized * 2, Color.magenta);
    }

    public void ResetVelocity()
    {
        velocity = Vector2.zero;
    }

    public void SetAttackedPlayer(PlayerCharacter target)
    {
        attackedPlayer = target;
        attackedPlayer.AddAttacker(this);
    }

    public void UnsetAtteckedPlayer()
    {
        if (attackedPlayer == null) return;
        attackedPlayer.RemoveAttacker(this);
        attackedPlayer = null;
    }

    public void AddDetectedPlayer(PlayerCharacter newDetection)
    {
        if (detectedPlayers.Count == 0) SetTarget(newDetection);

        detectedPlayers.Add(newDetection);
        D_detectedPlayer?.Invoke();
    }

    public void RemoveDetectedPlayer(PlayerCharacter player)
    {
        detectedPlayers.Remove(player);

        if (detectedPlayers.Count > 0) SetTarget(detectedPlayers.Last());
        else ResetTarget();

        storedTargetPosition = player.transform.position;
        D_lostPlayer?.Invoke();
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
#endif
    }
}

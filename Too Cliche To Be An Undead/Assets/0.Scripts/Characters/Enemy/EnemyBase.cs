using UnityEngine;

public abstract class EnemyBase : Entity
{
    [SerializeField] private SO_DropTable dropTable;

    [field: SerializeField] public Collider2D enemiesBlocker;

    [SerializeField] private float speedMultiplier;
    public float SpeedMultiplier { get => speedMultiplier; }

    [SerializeField] protected float maxForce;
    public float MaxForce { get => maxForce; }
    [field: SerializeField, ReadOnly] public float BaseMaxForce { get; protected set; }

    [SerializeField] protected float movementMass;
    public float MovementMass { get => movementMass; }
    [field: SerializeField, ReadOnly] public float BaseMovementMass { get; protected set; }

    [SerializeField] private float distanceBeforeStop = 1f;
    public float DistanceBeforeStop { get => distanceBeforeStop; }

    [field: SerializeField] public int MinDistanceForNormalSpeed { get; private set; }
    protected float speedMultiplierOnDistance = 1;

    [SerializeField] private Vector2 maxScaleOnAttack = new Vector2(1.3f, 1.3f);
    [SerializeField] private LeanTweenType inType = LeanTweenType.easeInSine;
    [SerializeField] private LeanTweenType outType = LeanTweenType.easeOutSine;

    public Vector2 MaxScaleOnAttack { get => maxScaleOnAttack; }
    public LeanTweenType InType { get => inType; }
    public LeanTweenType OutType { get => outType; }

    protected override void EventsSubscriber()
    {
        base.EventsSubscriber();
        if (HolderTryGetComponent(IComponentHolder.E_Component.HealthSystem, out HealthSystem healthSystem) == IComponentHolder.E_Result.Success)
            healthSystem.OnDeath += OnEnemyDeath;
    }

    protected override void EventsUnSubscriber()
    {
        base.EventsUnSubscriber();
        if (HolderTryGetComponent(IComponentHolder.E_Component.HealthSystem, out HealthSystem healthSystem) == IComponentHolder.E_Result.Success)
            healthSystem.OnDeath -= OnEnemyDeath;
    }

    protected virtual void Start()
    {
        distanceBeforeStop = distanceBeforeStop.Fluctuate();
    }

    private void OnEnemyDeath()
    {
        if (dropTable != null)
            dropTable.DropRandom(this.transform.position);

        if (HolderTryGetComponent(IComponentHolder.E_Component.EnemyAI, out BaseEnemyAI ai) == IComponentHolder.E_Result.Success)
            ai.CurrentTarget.RemoveAttacker(this);
    }

    //public void Movements(Vector2 goalPosition, bool slowdownOnApproach = true)
    //{
    //    if (speedMultiplierOnDistance <= 1)
    //    {
    //        desiredVelocity = goalPosition * MaxSpeed;

    //        steering = desiredVelocity - steeredVelocity;
    //        steering = Vector3.ClampMagnitude(steering, this.MaxForce);
    //        if (this.MovementMass != 0)
    //            steering /= this.MovementMass;

    //        void SteerVelocity(float multiplier = 1)
    //        {
    //            steeredVelocity = Vector3.ClampMagnitude(steeredVelocity + steering, MaxSpeed) * multiplier;
    //        }

    //        bool isTargetIdle = currentPlayerTarget.PlayerMotor != null &&
    //                            currentPlayerTarget != null
    //                            && currentPlayerTarget.PlayerMotor.Velocity == Vector2.zero;
    //        if ((slowdownOnApproach && allowSlowdown) && isTargetIdle)
    //        {
    //            float distance = Vector2.Distance(this.transform.position, CurrentPositionTarget);

    //            if (distance < distanceBeforeStop) SteerVelocity(distance / distanceBeforeStop);
    //            else SteerVelocity();
    //        }
    //        else SteerVelocity();
    //    }
    //    else steeredVelocity = goalPosition * MaxSpeed * speedMultiplierOnDistance;

    //    //this.GetRb.velocity = steeredVelocity * Time.fixedDeltaTime;
    //    this.GetRb.MovePosition(this.GetRb.position + steeredVelocity * Time.fixedDeltaTime);
    //}

    //public override void Death(bool forceDeath = false)
    //{
    //    if (dropTable != null && !forceDeath)
    //    {
    //        dropTable.DropRandom(this.transform.position);
    //    }

    //    this.EnemyDeath(lastDamagesData);

    //    attackedPlayer?.RemoveAttacker(this);
    //}
}

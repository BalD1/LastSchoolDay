using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ObjectMotor : MonoBehaviourEventsHandler
{
    [SerializeField] private GameObject ownerObj;
    protected IComponentHolder owner;

    [SerializeField] private float idleVelocityMargin = .8f;

    protected Rigidbody2D body;
    public Rigidbody2D Body
    {
        get => body;
    } 
        
    [field: SerializeField, ReadOnly] public Vector2 Velocity { get; protected set; }
    [field: SerializeField, ReadOnly] public Vector2 LastDirection { get; protected set; }

    private Vector2 lastVelocity;

    protected bool stayStatic;

    protected override void EventsSubscriber()
    {
        owner.OnComponentModified += OnComponentsModifier;
    }

    protected override void EventsUnSubscriber()
    {
        owner.OnComponentModified -= OnComponentsModifier;
    }

    protected override void Awake()
    {
        owner = ownerObj.GetComponent<IComponentHolder>();
        base.Awake();
        SetComponents();
    }

    private void FixedUpdate()
    {
        lastVelocity = Body.velocity;
    }

    protected virtual void SetComponents()
    {
        owner.HolderTryGetComponent(IComponentHolder.E_Component.RigidBody2D, out body);
    }

    protected virtual void OnComponentsModifier(ComponentChangeEventArgs args)
    {
        if (args.ComponentType == IComponentHolder.E_Component.RigidBody2D)
            body = args.NewComponent as Rigidbody2D;
    }

    public void SetSelfVelocity(Vector2 velocity) => this.Velocity = velocity;
    public void SetBodyVelocity(Vector2 velocity) => Body.velocity = velocity;
    public void SetAllVelocity(Vector2 velocity)
    {
        this.Velocity = velocity;
        Body.velocity = velocity;
    }

    public abstract void MoveByVelocity();
    public abstract void MoveTo(Vector2 goalPosition, bool steerVelocity);

    public void ForceSetLasDirection(Vector2 direction)
        => LastDirection = direction;

    public bool IsIdle()
    {
        if (Velocity == Vector2.zero &&
            IsCloseToZero(Body.velocity, idleVelocityMargin)) return true;
        return false;
    }

    public bool IsCloseToZero(Vector2 vec, float epsilon = 1E-6f)
        => IsCloseToZero(vec.x, epsilon) && IsCloseToZero(vec.y, epsilon);
    public bool IsCloseToZero(float value, float epsilon = 1E-6f)
        => Mathf.Abs(value) < epsilon;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BalDUtilities.VectorUtils;

public class FSM_Entity_Pushed<T> : FSM_Base<T>
{
    protected Entity owner;
    private Entity pusher;
    private Entity originalPusher;
    protected bool baseConditionChecked;
    protected Vector2 force;

    private List<Collider2D> alreadyPushedEntities;

    private int wallLayer;

    private static float wallHitDamagesModifier = .4f;

    private const float forcePushTransmissionPercentage = .9f;

    public override void EnterState(T stateManager)
    {
        wallLayer = LayerMask.NameToLayer("Wall");
        alreadyPushedEntities = new List<Collider2D>();
        owner.GetRb.velocity = Vector2.zero;
        PerformPush(pusher);

        owner.OnEnteredBodyTrigger += TriggerEnter;
        owner.d_EnteredCollider += ColliderEnter;

        owner.D_OnPushed?.Invoke();
    }

    public override void UpdateState(T stateManager)
    {
    }

    public override void FixedUpdateState(T stateManager)
    {
    }

    public override void ExitState(T stateManager)
    {
        owner.OnEnteredBodyTrigger -= TriggerEnter;
        owner.d_EnteredCollider -= ColliderEnter;

        alreadyPushedEntities.Clear();
        baseConditionChecked = false;

        originalPusher = null;
    }

    public override void Conditions(T stateManager)
    {
        if (VectorMaths.Vector2ApproximatlyEquals(owner.GetRb.velocity, Vector2.zero, 0.08f)) baseConditionChecked = true;
    }

    protected virtual void PerformPush(Entity pusher)
    {
        owner.GetRb.AddForce(force, ForceMode2D.Impulse);
    }

    protected virtual void ColliderEnter(Collision2D collision)
    {
        float damages = (owner.LastVelocity.magnitude + owner.GetStats.Weight) * wallHitDamagesModifier;
        damages = Mathf.Round(damages);

        owner.OnTakeDamages(damages, originalPusher);
    }

    protected virtual void TriggerEnter(Collider2D collider)
    {
        // Check if the hit object is an entity
        Entity e = collider.GetComponentInParent<Entity>();
        if (e == null) return;

        // Check if the entity as not already been pushed
        if (alreadyPushedEntities.Contains(collider) || e.Equals(originalPusher)) return;
        alreadyPushedEntities.Add(collider);

        // lessen the PushForce depending on the remaining push time
        owner.GetRb.velocity *= forcePushTransmissionPercentage;
        float appliedForce = owner.GetRb.velocity.magnitude + owner.GetStats.Weight;

        e.Push(owner.transform.position, appliedForce, originalPusher, owner);
    }

    public void SetOwner(Entity _owner) => owner = _owner;
    public FSM_Entity_Pushed<T> SetForce(Vector2 _force, Entity _originalPusher, Entity _pusher)
    {
        force = _force;
        originalPusher = _originalPusher;
        pusher = _pusher;
        return this;
    }

    public override string ToString() => "Pushed";
}

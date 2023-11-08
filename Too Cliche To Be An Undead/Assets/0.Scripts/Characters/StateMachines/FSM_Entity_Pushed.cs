using System.Collections.Generic;
using UnityEngine;
using BalDUtilities.VectorUtils;
using System;

public class FSM_Entity_Pushed<Manager, StateName> : FSM_Base<Manager, StateName>
                                                     where Manager : FSM_ManagerBase
                                                     where StateName : Enum
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

    public override void EnterState(Manager stateManager)
    {
        base.EnterState(stateManager);
        wallLayer = LayerMask.NameToLayer("Wall");
        alreadyPushedEntities = new List<Collider2D>();
        //owner.GetRb.velocity = Vector2.zero;
        PerformPush(pusher);

        //owner.OnPushed?.Invoke();
    }

    public override void UpdateState(Manager stateManager)
    {
    }

    public override void FixedUpdateState(Manager stateManager)
    {
    }

    public override void ExitState(Manager stateManager)
    {
        base.ExitState(stateManager);
        alreadyPushedEntities.Clear();
        baseConditionChecked = false;

        originalPusher = null;
    }

    public override void Conditions(Manager stateManager)
    {
        //if (VectorMaths.Vector2ApproximatlyEquals(owner.GetRb.velocity, Vector2.zero, 0.08f)) baseConditionChecked = true;
    }

    protected override void EventsSubscriber(Manager stateManager)
    {
        //owner.OnEnteredBodyTrigger += TriggerEnter;
        //owner.d_EnteredCollider += ColliderEnter;
    }

    protected override void EventsUnsubscriber(Manager stateManager)
    {
        //owner.OnEnteredBodyTrigger -= TriggerEnter;
        //owner.d_EnteredCollider -= ColliderEnter;
    }

    protected virtual void PerformPush(Entity pusher)
    {
        //owner.GetRb.AddForce(force, ForceMode2D.Impulse);
    }

    protected virtual void ColliderEnter(Collision2D collision)
    {
        //float damages = (owner.LastVelocity.magnitude + owner.GetStats.Weight) * wallHitDamagesModifier;
        //damages = Mathf.Round(damages);

        //owner.InflinctDamages(damages, originalPusher);
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
        //owner.GetRb.velocity *= forcePushTransmissionPercentage;
        //float appliedForce = owner.GetRb.velocity.magnitude + owner.GetStats.Weight;

        //e.AskPush(appliedForce, owner, originalPusher);
    }

    public FSM_Entity_Pushed<Manager, StateName> SetForce(Vector2 _force, Entity _pusher, Entity _originalPusher)
    {
        force = _force;
        pusher = _pusher;
        originalPusher = _originalPusher;
        return this;
    }

    public override void Setup(Manager stateManager)
    {
        throw new System.NotImplementedException();
    }

    public override string ToString() => "Pushed";


}

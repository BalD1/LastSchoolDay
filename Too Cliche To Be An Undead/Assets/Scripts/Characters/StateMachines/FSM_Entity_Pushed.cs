using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Entity_Pushed<T> : FSM_Base<T>
{
    protected Entity owner;
    protected bool baseConditionChecked;
    private Vector2 force;

    private List<Collider2D> alreadyPushedEntities;


    public override void EnterState(T stateManager)
    {
        alreadyPushedEntities = new List<Collider2D>();
        owner.GetRb.velocity = Vector2.zero;
        owner.GetRb.AddForce(force, ForceMode2D.Impulse);
        owner.d_EnteredTrigger += TriggerEnter;

        if (owner as EnemyBase != null)
            (owner as EnemyBase).UnsetAtteckedPlayer();
        
    }

    public override void UpdateState(T stateManager)
    {
    }

    public override void FixedUpdateState(T stateManager)
    {
    }

    public override void ExitState(T stateManager)
    {
        owner.d_EnteredTrigger -= TriggerEnter;
        baseConditionChecked = false;
    }

    public override void Conditions(T stateManager)
    {
        if (owner.GetRb.velocity.Equals(Vector2.zero)) baseConditionChecked = true;
    }

    private void TriggerEnter(Collider2D collider)
    {
        // Check if the hit object is an entity
        Entity e = collider.GetComponentInParent<Entity>();
        if (e == null) return;

        // Check if the entity as not already been pushed
        if (alreadyPushedEntities.Contains(collider)) return;

        alreadyPushedEntities.Add(collider);

        // lessen the PushForce depending on the remaining push time
        float appliedForce = owner.GetRb.velocity.magnitude + owner.GetStats.Weight;

        e.Push(owner.transform.position, appliedForce);
    }

    public void SetOwner(Entity _owner) => owner = _owner;
    public FSM_Entity_Pushed<T> SetForce(Vector2 _force)
    {
        force = _force;
        return this;
    }

    public override string ToString() => "Pushed";
}

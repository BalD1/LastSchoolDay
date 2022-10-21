using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Entity_Pushed<T> : FSM_Base<T>
{
    protected Entity owner;
    protected bool baseConditionChecked;
    private Vector2 force;

    public override void EnterState(T stateManager)
    {
        owner.GetRb.velocity = Vector2.zero;
        owner.GetRb.AddForce(force, ForceMode2D.Impulse);
    }

    public override void UpdateState(T stateManager)
    {
    }

    public override void FixedUpdateState(T stateManager)
    {
    }

    public override void ExitState(T stateManager)
    {
        baseConditionChecked = false;
    }

    public override void Conditions(T stateManager)
    {
        if (owner.GetRb.velocity.Equals(Vector2.zero)) baseConditionChecked = true;
    }

    public void SetOwner(Entity _owner) => owner = _owner;
    public FSM_Entity_Pushed<T> SetForce(Vector2 _force)
    {
        force = _force;
        return this;
    }
}

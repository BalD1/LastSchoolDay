using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Entity_Idle<T> : FSM_Base<T>
{
    protected Entity owner;
    protected bool baseConditionChecked;

    public override void EnterState(T stateManager)
    {
        if (owner.GetRb != null)
            owner.GetRb.velocity = Vector2.zero;

        if (owner.GetAnimator != null)
            owner.SetAnimatorArgs(Entity.ANIMATOR_ARGS_VELOCITY, 0f);
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
        if (owner.GetRb?.velocity != Vector2.zero) baseConditionChecked = true;
    }

    public void SetOwner(Entity _owner) => owner = _owner;

    public override string ToString() => "Idle";
}

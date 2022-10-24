using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSM_Base<T>
{
    public abstract void EnterState(T stateManager);
    public abstract void UpdateState(T stateManager);
    public abstract void FixedUpdateState(T stateManager);
    public abstract void ExitState(T stateManager);
    public abstract void Conditions(T stateManager);

    public abstract override string ToString();
}

using UnityEngine;

public abstract class FSM_ManagerBase : MonoBehaviour
{
    protected virtual void Start() => SetupStates();
    protected abstract void Update();
    protected abstract void FixedUpdate();

    public abstract void SetupStates();
    public abstract override string ToString();
}
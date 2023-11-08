using System;

public interface IState
{
    public abstract void EnterState();
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void ExitState();
    public abstract void Conditions();
}
using System;
using UnityEngine;

public abstract class CinematicAction
{
    public event Action<CinematicAction> OnEnded;
    protected virtual void EndAction(CinematicAction self) => OnEnded?.Invoke(self);
    public abstract void Execute();
}
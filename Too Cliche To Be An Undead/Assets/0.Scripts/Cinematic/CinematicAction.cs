using System;
using UnityEngine;

public abstract class CinematicAction
{
    protected Cinematic owner;

    public event Action<CinematicAction> OnActionEnded;
    protected virtual void ActionEnded(CinematicAction action) => OnActionEnded?.Invoke(action);
    public void SetOwner(Cinematic owner) => this.owner = owner;
    public abstract void Execute();
}
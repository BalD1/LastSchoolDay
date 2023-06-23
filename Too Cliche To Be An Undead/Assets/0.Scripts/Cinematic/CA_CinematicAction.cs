using System;
using UnityEngine;

[System.Serializable]
public class CA_CinematicAction
{
    protected Cinematic owner;

    public event Action<CA_CinematicAction> OnActionEnded;
    protected virtual void ActionEnded(CA_CinematicAction action) => OnActionEnded?.Invoke(action);
    public void SetOwner(Cinematic owner) => this.owner = owner;
    public virtual void Execute() { }
}
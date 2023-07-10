using System;
using UnityEngine;

[System.Serializable]
public class CA_CinematicAction
{
    [SerializeField] protected LeanTweenType leanType = LeanTweenType.linear;
    protected Cinematic owner;

    public event Action<CA_CinematicAction> OnActionEnded;
    protected virtual void ActionEnded(CA_CinematicAction action) => OnActionEnded?.Invoke(action);
    public virtual CA_CinematicAction SetOwner(Cinematic owner)
    {
        this.owner = owner;
        return this;
    }
    public virtual CA_CinematicAction SetLeanType(LeanTweenType newType)
    {
        leanType = newType;
        return this;
    }

    public virtual void Execute() { }
}
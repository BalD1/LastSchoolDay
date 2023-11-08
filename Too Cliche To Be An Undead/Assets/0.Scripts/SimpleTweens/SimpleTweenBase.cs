using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimpleTweenBase<T> : MonoBehaviour where T : Component
{
    [SerializeField] protected T target;

    [SerializeField] protected float tweenDuration;
    [SerializeField] protected LeanTweenType tweenInType;
    [SerializeField] protected LeanTweenType tweenOutType;

    private bool isTweening;

    public event Action OnTweenEnded;

    public virtual bool TryPlay()
    {
        if (target == null)
        {
            this.Log("Tween target was null", CustomLogger.E_LogType.Error);
            return false;
        }
        if (isTweening) return false;
        isTweening = true;

        Play();
        return true;
    }

    protected abstract void Play();

    protected virtual void TweenEnded()
    {
        isTweening = false;
        OnTweenEnded?.Invoke();
    }
}

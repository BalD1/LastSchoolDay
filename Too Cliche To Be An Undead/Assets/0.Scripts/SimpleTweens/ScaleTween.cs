using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTween : SimpleTweenBase<Transform>
{
    [SerializeField] private Vector2 scaleTarget;
    private Vector2 baseScale;

    protected override void Play()
    {
        baseScale = transform.localScale;
        target.LeanScale(scaleTarget, tweenDuration).setEase(tweenInType).setOnComplete(
            () => target.LeanScale(baseScale, tweenDuration).setEase(tweenOutType).setOnComplete(TweenEnded)
            );
    }
}

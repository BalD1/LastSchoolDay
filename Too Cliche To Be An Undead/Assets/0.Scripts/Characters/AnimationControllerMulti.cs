using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AnimationControllerMulti : AnimationControllerBase
{
    [field: SerializeField] public SkeletonAnimationMulti CurrentMultiSkeletonAnimation { get; private set; }
    [field: SerializeField] public SkeletonAnimation CurrentSkeletonAnimation { get; private set; }

    protected override void EventsSubscriber()
    {
        CurrentMultiSkeletonAnimation.SwitchSkeleton += SwitchSkeleton;
    }

    protected override void EventsUnSubscriber()
    {
        CurrentMultiSkeletonAnimation.SwitchSkeleton -= SwitchSkeleton;
    }

    public void SetSkeleton(SkeletonAnimationMulti skeletonAnimation)
        => CurrentMultiSkeletonAnimation = skeletonAnimation;

    protected override void Setup() { }

    public override bool TryFlipSkeleton(bool lookAtRight)
    {
        if (CurrentMultiSkeletonAnimation == null) return false;
        if (!(lookAtRight ^ IsLookingAtRight())) return false;
        FlipSkeleton();
        return true;
    }
    public override void FlipSkeleton()
    {
        Vector2 scale = CurrentMultiSkeletonAnimation.transform.localScale;
        scale.x *= -1;
        CurrentMultiSkeletonAnimation.transform.localScale = scale;
    }

    public override bool IsLookingAtRight()
        => CurrentMultiSkeletonAnimation.transform.localScale.x > 0;

    public override void SetAnimation(string animation, bool loop, float timeScale = 1)
    {
        if (CurrentMultiSkeletonAnimation == null || animation == null) return;

        CurrentMultiSkeletonAnimation.SetAnimation(animation, loop).TimeScale = timeScale;
    }
    public override void SetAnimation(Spine.Animation animation, bool loop, float timeScale = 1)
    {
        if (CurrentMultiSkeletonAnimation == null || animation == null) return;

        CurrentMultiSkeletonAnimation.SetAnimation(animation, loop).TimeScale = timeScale;
    }

    public override void AddAnimation(string animation, bool loop, float timeScale = 1)
    {
        if (CurrentMultiSkeletonAnimation == null || animation == null) return;

        CurrentMultiSkeletonAnimation.SetAnimation(animation, loop).TimeScale = timeScale;
    }
    public override void AddAnimation(Spine.Animation animation, bool loop, float timeScale = 1)
    {
        if (CurrentMultiSkeletonAnimation == null || animation == null) return;

        CurrentMultiSkeletonAnimation.SetAnimation(animation, loop).TimeScale = timeScale;
    }

    public SkeletonAnimation GetSkeletonOf(int idx)
        => CurrentMultiSkeletonAnimation.transform.GetChild(idx + 1).GetComponent<SkeletonAnimation>();

    public override void SetAnimationSequence(S_AnimationSequenceSingle[] sequence, bool loopLast, float timeScale = 1)
    {
        if (CurrentMultiSkeletonAnimation == null) return;

        float delay = 0;

        for (int i = 0; i < sequence.Length; i++)
        {
            if (sequence[i].Animation == null) continue;

            float finalTimeScale = timeScale * sequence[i].TimescaleModifier;

            if (i == 0) CurrentMultiSkeletonAnimation.SetAnimation(sequence[i].Animation, false).TimeScale = finalTimeScale;
            else
            {
                bool loop = loopLast && i == sequence.Length - 1;
                LeanTween.delayedCall(delay, () =>
                    {
                        CurrentMultiSkeletonAnimation.SetAnimation(sequence[i].Animation, loop).TimeScale = finalTimeScale;
                    }
                );
                
            }

            delay = sequence[i].DelayBeforeNext;
        }
    }

    public virtual void SwitchSkeleton()
    {
        this.CurrentSkeletonAnimation = CurrentMultiSkeletonAnimation.CurrentSkeletonAnimation;
    }

    public override void SetSkeletonColor(Color color)
    {
        this.CurrentMultiSkeletonAnimation.CurrentSkeletonAnimation.Skeleton.SetColor(color);
    }

    public override void SetSkeletonAlpha(float alpha)
    {
        Skeleton sk = this.CurrentMultiSkeletonAnimation.CurrentSkeletonAnimation.Skeleton;
        Color currentColor = sk.GetColor();
        currentColor.a = alpha;
        sk.SetColor(currentColor);
    }
}

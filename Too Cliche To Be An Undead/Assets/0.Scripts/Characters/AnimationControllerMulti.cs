using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationControllerMulti : AnimationControllerBase
{
    [field: SerializeField] public SkeletonAnimationMulti CurrentSkeletonAnimation { get; private set; }

    protected override void Setup() { }

    public override bool TryFlipSkeleton(bool lookAtRight)
    {
        if (CurrentSkeletonAnimation == null) return false;
        if (!(lookAtRight ^ IsLookingAtRight())) return false;
        FlipSkeleton();
        return true;
    }
    public override void FlipSkeleton()
    {
        Vector2 scale = CurrentSkeletonAnimation.transform.localScale;
        scale.x *= -1;
        CurrentSkeletonAnimation.transform.localScale = scale;
    }

    public override bool IsLookingAtRight()
        => CurrentSkeletonAnimation.transform.localScale.x > 0;

    public override void SetAnimation(string animation, bool loop, float timeScale = 1)
    {
        if (CurrentSkeletonAnimation == null || animation == null) return;

        CurrentSkeletonAnimation.SetAnimation(animation, loop).TimeScale = timeScale;
    }
    public override void SetAnimation(Spine.Animation animation, bool loop, float timeScale = 1)
    {
        if (CurrentSkeletonAnimation == null || animation == null) return;

        CurrentSkeletonAnimation.SetAnimation(animation, loop).TimeScale = timeScale;
    }

    public override void AddAnimation(string animation, bool loop, float timeScale = 1)
    {
        if (CurrentSkeletonAnimation == null || animation == null) return;

        CurrentSkeletonAnimation.SetAnimation(animation, loop).TimeScale = timeScale;
    }
    public override void AddAnimation(Spine.Animation animation, bool loop, float timeScale = 1)
    {
        if (CurrentSkeletonAnimation == null || animation == null) return;

        CurrentSkeletonAnimation.SetAnimation(animation, loop).TimeScale = timeScale;
    }

    public SkeletonAnimation GetSkeletonOf(int idx)
        => CurrentSkeletonAnimation.transform.GetChild(idx + 1).GetComponent<SkeletonAnimation>();

    protected override void EventsSubscriber()
    {
    }

    protected override void EventsUnSubscriber()
    {
    }

    public override void SetAnimationSequence(S_AnimationSequenceSingle[] sequence, bool loopLast, float timeScale = 1)
    {
        if (CurrentSkeletonAnimation == null) return;

        float delay = 0;

        for (int i = 0; i < sequence.Length; i++)
        {
            if (sequence[i].Animation == null) continue;

            float finalTimeScale = timeScale * sequence[i].TimescaleModifier;

            if (i == 0) CurrentSkeletonAnimation.SetAnimation(sequence[i].Animation, false).TimeScale = finalTimeScale;
            else
            {
                bool loop = loopLast && i == sequence.Length - 1;
                LeanTween.delayedCall(delay, () =>
                    {
                        CurrentSkeletonAnimation.SetAnimation(sequence[i].Animation, loop).TimeScale = finalTimeScale;
                    }
                );
                
            }

            delay = sequence[i].DelayBeforeNext;
        }
    }
}

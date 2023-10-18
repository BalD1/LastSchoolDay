using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerSingle : AnimationControllerBase
{
    [field: SerializeField] public SkeletonAnimation SkeletonAnimation { get; private set; }

    protected override void Setup() { }

    public override bool TryFlipSkeleton(bool lookAtRight)
    {
        if (SkeletonAnimation == null) return false;
        if (!(lookAtRight ^ IsLookingAtRight())) return false;
        FlipSkeleton();
        return true;
    }
    public override void FlipSkeleton()
    {
        Vector2 scale = SkeletonAnimation.transform.localScale;
        scale.x *= -1;
        SkeletonAnimation.transform.localScale = scale;
    }

    public override bool IsLookingAtRight()
        => SkeletonAnimation.transform.localScale.x > 0;

    public override void SetAnimation(string animation, bool loop, float timeScale = 1)
    {
        if (SkeletonAnimation == null || animation == null) return;

        SkeletonAnimation.AnimationState.SetAnimation(0, animation, loop).TimeScale = timeScale;
    }
    public override void SetAnimation(Spine.Animation animation, bool loop, float timeScale = 1)
    {
        if (SkeletonAnimation == null || animation == null) return;

        SkeletonAnimation.AnimationState.SetAnimation(0, animation, loop).TimeScale = timeScale;
    }

    public override void AddAnimation(string animation, bool loop, float timeScale = 1)
    {
        if (SkeletonAnimation == null || animation == null) return;

        SkeletonAnimation.AnimationState.SetAnimation(0, animation, loop).TimeScale = timeScale;
    }
    public override void AddAnimation(Spine.Animation animation, bool loop, float timeScale = 1)
    {
        if (SkeletonAnimation == null || animation == null) return;

        SkeletonAnimation.AnimationState.SetAnimation(0, animation, loop).TimeScale = timeScale;
    }

    public override void SetAnimationSequence(S_AnimationSequenceSingle[] sequence, bool loopLast, float timeScale = 1)
    {
        if (SkeletonAnimation == null) return;
        for (int i = 0; i < sequence.Length; i++)
        {
            if (sequence[i].Animation == null) continue;

            float finalTimeScale = timeScale * sequence[i].TimescaleModifier;

            if (i == 0) SkeletonAnimation.AnimationState.SetAnimation(0, sequence[i].Animation, false).TimeScale = finalTimeScale;
            else
            {
                bool loop = loopLast && i == sequence.Length - 1;
                SkeletonAnimation.AnimationState.AddAnimation(0, sequence[i].Animation, loop, sequence[i].DelayBeforeNext).TimeScale = finalTimeScale;
            }
        }
    }

    protected override void EventsSubscriber()
    {
    }

    protected override void EventsUnSubscriber()
    {
    }
}

using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    [SpineAnimation]
    [SerializeField] private string idleAnimation;

    [SerializeField] [ReadOnly] private string currentState = "N/A";

    private Spine.Skeleton skeleton;

    private void Awake()
    {
        if (skeletonAnimation != null)
            skeleton = skeletonAnimation.Skeleton;
    }

    public void FlipSkeleton(bool lookAtRight)
    {
        if (skeletonAnimation == null) return;

        if (lookAtRight && IsLookingAtRight()) return;
        if (!lookAtRight && !IsLookingAtRight()) return;

        Vector2 scale = skeletonAnimation.gameObject.transform.localScale;
        scale.x *= -1;
        skeletonAnimation.gameObject.transform.localScale = scale;
    }

    private void SetAnimation(string animation, bool loop, float timeScale = 1)
    {
        if (skeletonAnimation == null) return;

        skeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = timeScale;
    }

    public void SetCharacterState(string state)
    {
        currentState = state;

        switch (state)
        {
            case "Idle":
                SetAnimation(idleAnimation, true);
                break;

            default:
                break;
        }
    }

    public bool IsLookingAtRight() => skeletonAnimation.gameObject.transform.localScale.x > 0;
}

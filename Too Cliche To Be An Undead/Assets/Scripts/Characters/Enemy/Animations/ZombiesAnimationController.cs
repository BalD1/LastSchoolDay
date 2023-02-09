using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesAnimationController : MonoBehaviour
{
    [Header("Animations")]

    [SerializeField] private NormalZombie owner;

    [SerializeField] private SkeletonAnimation skeletonAnimation;
    public SkeletonAnimation SkeletonAnimation { get => skeletonAnimation; }

    [SerializeField][ReadOnly] private string currentState = "N/A";

    [field: SerializeField][field: ReadOnly] public bool isValid { get; private set; }

    [Space]
    [Header("Editor")]

    [InspectorButton(nameof(SetAnimationInspector), ButtonWidth = 200)]
    [SerializeField] private bool ForceAnimation;

#if UNITY_EDITOR
    [SpineAnimation]
    [SerializeField] private string editor_AnimationToSet;
    [SerializeField] private bool editor_loopAnimation;
#endif

    private void Awake()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, owner.animationData.WalkAnim, true);
        isValid = true;
    }

    private void Start()
    {
        Setup();
    }

    public void Setup()
    {
        //temp
        if (owner.animationData == null)
        {
            skeletonAnimation.gameObject.SetActive(false);
            isValid = false;
            return;
        }

        //temp
        skeletonAnimation.gameObject.SetActive(true);

        SetAnimation(owner.animationData.WalkAnim, true);

        isValid = true;
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

    public void SetAnimation(string animation, bool loop, float timeScale = 1)
    {
        if (skeletonAnimation == null || animation == null) return;

        skeletonAnimation.AnimationState.SetAnimation(0, animation, loop).TimeScale = timeScale;
    }
    public void SetAnimation(Spine.Animation animation, bool loop, float timeScale = 1)
    {
        if (skeletonAnimation == null || animation == null) return;

        skeletonAnimation.AnimationState.SetAnimation(0, animation, loop).TimeScale = timeScale;
    }

    public void AddAnimation(string animation, bool loop, float timeScale = 1)
    {
        if (skeletonAnimation == null || animation == null) return;

        skeletonAnimation.AnimationState.SetAnimation(0, animation, loop).TimeScale = timeScale;
    }
    public void AddAnimation(Spine.Animation animation, bool loop, float timeScale = 1)
    {
        if (skeletonAnimation == null || animation == null) return;

        skeletonAnimation.AnimationState.SetAnimation(0, animation, loop).TimeScale = timeScale;
    }

    public bool IsLookingAtRight() => skeletonAnimation.gameObject.transform.localScale.x > 0;

    private void SetAnimationInspector()
    {
#if UNITY_EDITOR
        SetAnimation(editor_AnimationToSet, editor_loopAnimation);
#endif
    }
}

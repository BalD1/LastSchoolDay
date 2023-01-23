using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Animations")]

    [SerializeField] private SpriteRenderer placeholderSprite;

    [SerializeField] private SkeletonAnimationMulti skeletonAnimation;
    public SkeletonAnimationMulti SkeletonAnimation { get => skeletonAnimation; }

    [SerializeField] private Transform armsParent;

    [field: SerializeField] public SCRPT_PlayersAnimData animationsData { get; private set; }

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
        if (skeletonAnimation == null || animationsData == null)
        {
            placeholderSprite.enabled = true;
            this.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            isValid = false;
        }
        else
        {
            skeletonAnimation.SetAnimation(animationsData.idleAnim, true);
            isValid = true;
        }
    }

    private void Start()
    {
        foreach (Transform item in skeletonAnimation.transform)
        {
            if (item.GetComponent<SkeletonAnimation>() != null)
                item.gameObject.AddComponent<RendererSorting>();
        }

        skeletonAnimation._switchSkeleton += SwitchSkeleton;
    }

    public void Setup(SCRPT_PlayersAnimData animData)
    {
        //temp
        if (animData == null)
        {
            placeholderSprite.enabled = true;
            skeletonAnimation.gameObject.SetActive(false);
            isValid = false;
            return;
        }

        //temp
        skeletonAnimation.gameObject.SetActive(true);
        placeholderSprite.sprite = null;

        animationsData = animData;
        SetAnimation(animationsData.idleAnim, true);

        isValid = true;
    }

    public void SwitchSkeleton()
    {
        foreach (Transform arm in armsParent)
        {
            arm.GetComponent<CustomBoneFollow>().skeletonRenderer = skeletonAnimation.CurrentSkeletonAnimation;
        }
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

        skeletonAnimation.SetAnimation(animation, loop).TimeScale = timeScale;
    }
    public void SetAnimation(Spine.Animation animation, bool loop, float timeScale = 1)
    {
        if (skeletonAnimation == null || animation == null) return;

        skeletonAnimation.SetAnimation(animation, loop).TimeScale = timeScale;
    }

    public void AddAnimation(string animation, bool loop, float timeScale = 1)
    {
        if (skeletonAnimation == null || animation == null) return;

        skeletonAnimation.SetAnimation(animation, loop).TimeScale = timeScale;
    }
    public void AddAnimation(Spine.Animation animation, bool loop, float timeScale = 1)
    {
        if (skeletonAnimation == null || animation == null) return;

        skeletonAnimation.SetAnimation(animation, loop).TimeScale = timeScale;
    }

    public void SetCharacterState(string state)
    {
        if (!isValid) return;

        currentState = state;

        switch (state)
        {
            case "Idle":
                SetAnimation(animationsData.idleAnim, true);
                break;

            case "Moving":
                SetAnimation(animationsData.walkAnim, true);
                break;

            case "Dashing":
                SetAnimation(animationsData.dashAnim, true);
                break;

            case "Dying":
                SetAnimation(animationsData.deathAnim, true);
                break;

            default:
                break;
        }
    }

    public bool IsLookingAtRight() => skeletonAnimation.gameObject.transform.localScale.x > 0;

    private void SetAnimationInspector()
    {
#if UNITY_EDITOR
        SetAnimation(editor_AnimationToSet, editor_loopAnimation);
#endif
    }
}

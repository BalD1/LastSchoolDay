using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Animations")]

    [SerializeField] private PlayerCharacter owner;

    [SerializeField] private SpriteRenderer placeholderSprite;

    [SerializeField] private SkeletonAnimationMulti skeletonAnimation;
    public SkeletonAnimationMulti SkeletonAnimation { get => skeletonAnimation; }

    [SerializeField] private Transform armsParent;

    [field: SerializeField] public CustomBoneFollow leftArmBone { get; private set; }
    [field: SerializeField] public CustomBoneFollow rightArmBone { get; private set; }

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
            skeletonAnimation.SetAnimation(animationsData.IdleAnim, true);
            isValid = true;
        }
    }

    private void Start()
    {
        foreach (Transform item in skeletonAnimation.transform)
        {
            if (item.GetComponent<SkeletonAnimation>() != null)
            {
                item.gameObject.AddComponent<RendererSorting>();
                item.gameObject.AddComponent<VisibilityWatcher>().Setup(owner);
            }
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
        SetAnimation(animationsData.IdleAnim, true);

        isValid = true;
    }

    public void SwitchSkeleton()
    {
        foreach (Transform arm in armsParent)
        {
            arm.GetComponent<CustomBoneFollow>().skeletonRenderer = skeletonAnimation.CurrentSkeletonAnimation;
        }

        foreach (Transform item in skeletonAnimation.transform)
        {
            VisibilityWatcher visibilityWatcher = item.GetComponent<VisibilityWatcher>();
            if (visibilityWatcher != null) visibilityWatcher.ForceVisibility();
        }

        owner.SkeletonAnimation = skeletonAnimation.CurrentSkeletonAnimation;
    }

    public void FlipSkeleton(bool lookAtRight)
    {
        if (skeletonAnimation == null) return;

        if (lookAtRight && IsLookingAtRight()) return;
        if (!lookAtRight && !IsLookingAtRight()) return;

        Vector2 scale = skeletonAnimation.gameObject.transform.localScale;
        scale.x *= -1;
        skeletonAnimation.gameObject.transform.localScale = scale;

        Vector2 v = leftArmBone.offset;
        v.x *= -1;
        leftArmBone.offset = v;

        v = rightArmBone.offset;
        v.x *= -1;
        rightArmBone.offset = v;
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

    public SkeletonAnimation GetSkeleton(int idx)
    {
        return skeletonAnimation.transform.GetChild(idx + 1).GetComponent<SkeletonAnimation>();
    }

    public void SetCharacterState(string state)
    {
        if (!isValid) return;

        currentState = state;

        switch (state)
        {
            case "Idle":
                SetAnimation(animationsData.IdleAnim, true);
                break;

            case "Moving":
                SetAnimation(animationsData.WalkAnim, true);
                break;

            case "Dashing":
                SetAnimation(animationsData.dashAnim, true);
                break;

            case "Dying":
                SetAnimation(animationsData.DeathAnim, true);
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

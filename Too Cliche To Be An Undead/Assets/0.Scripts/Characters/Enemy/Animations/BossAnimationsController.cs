using Spine;
using Spine.Unity;
using UnityEngine;

public class BossAnimationsController : MonoBehaviourEventsHandler
{
    [Header("Animations")]

    [SerializeField] private BossZombie owner;

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

    protected override void EventsSubscriber()
    {
        owner.OnStateChange += SetAnimationFromState;
    }

    protected override void EventsUnSubscriber()
    {
        owner.OnStateChange -= SetAnimationFromState;
    }

    protected override void Awake()
    {
        base.Awake();
        skeletonAnimation.AnimationState.SetAnimation(0, owner.animationData.WalkAnim, true);
        isValid = true;
    }

    private void Start()
    {
        Setup();
    }

    private void SetAnimationFromState(string stateKey)
    {
        foreach (var item in owner.animationData.StateAnimation)
        {
            if (item.Key.ToString() != stateKey) continue;

            SetAnimation(item.Asset, item.Loop);
            return;
        }
    }

    public void Setup()
    {
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

        skeletonAnimation.AnimationState.AddAnimation(0, animation, loop, .25f).TimeScale = timeScale;
    }
    public void AddAnimation(Spine.Animation animation, bool loop, float timeScale = 1)
    {
        if (skeletonAnimation == null || animation == null) return;

        skeletonAnimation.AnimationState.AddAnimation(0, animation, loop, .25f).TimeScale = timeScale;
    }
    public void AddAnimation(Spine.Animation animation, bool loop, float delay, float timeScale = 1)
    {
        if (skeletonAnimation == null || animation == null) return;

        skeletonAnimation.AnimationState.AddAnimation(0, animation, loop, delay).TimeScale = timeScale;
    }

    public bool IsLookingAtRight() => skeletonAnimation.gameObject.transform.localScale.x > 0;

    private void SetAnimationInspector()
    {
#if UNITY_EDITOR
        SetAnimation(editor_AnimationToSet, editor_loopAnimation);
#endif
    }
}

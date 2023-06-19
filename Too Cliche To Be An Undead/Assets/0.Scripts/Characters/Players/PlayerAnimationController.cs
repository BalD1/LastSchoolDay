using BalDUtilities.MouseUtils;
using Spine.Unity;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviourEventsHandler
{
    [Header("Animations")]

    [SerializeField] private PlayerCharacter owner;

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

    [SerializeField] private SkeletonDataAsset jasonSkeletonDataAsset;
    [SerializeField] private SkeletonRendererCustomMaterials jasonMaterialOverride_PF;

    public SkeletonRendererCustomMaterials JasonMaterialOverride { get; private set; }
 
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
        isValid = true;
    }

    private void Start()
    {
        foreach (Transform item in skeletonAnimation.transform)
        {
            if (item.GetComponent<SkeletonAnimation>() != null)
            {
                item.gameObject.AddComponent<RendererSorting>();
                item.gameObject.AddComponent<VisibilityWatcher>().Setup(owner);

                if (item.GetComponent<SkeletonAnimation>().skeletonDataAsset == jasonSkeletonDataAsset)
                {
                    SkeletonRendererCustomMaterials ovm = jasonMaterialOverride_PF.gameObject.Create<SkeletonRendererCustomMaterials>(item);
                    ovm.skeletonRenderer = item.GetComponent<SkeletonRenderer>();
                    JasonMaterialOverride = ovm;
                    JasonMaterialOverride.gameObject.SetActive(false);
                }
            }
        }

        skeletonAnimation._switchSkeleton += SwitchSkeleton;
    }

    private void SetAnimationFromState(string state)
    {
        foreach (var item in animationsData.StateAnimation)
        {
            if (item.Key.ToString() != state) continue;

            SetAnimation(item.Asset, item.Loop);
            return;
        }
    }

    public void Setup(SCRPT_PlayersAnimData animData)
    {
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

        owner.SkeletonAnimation = skeletonAnimation.CurrentSkeletonAnimation;
    }

    public void FlipSkeletonOnMouseOrGamepad()
    {
        if (owner.PlayerInputsComponent.IsOnKeyboard())
        {
            Vector2 mousePos = MousePosition.GetMouseWorldPosition();
            Vector2 mouseDir = (mousePos - (Vector2)owner.transform.position).normalized;
            FlipSkeleton(mouseDir.x > 0);
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

        Vector3 v = leftArmBone.offset;
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

    public bool IsLookingAtRight() => skeletonAnimation.gameObject.transform.localScale.x > 0;

    private void SetAnimationInspector()
    {
#if UNITY_EDITOR
        SetAnimation(editor_AnimationToSet, editor_loopAnimation);
#endif
    }
}

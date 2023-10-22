using BalDUtilities.MouseUtils;
using Spine.Unity;
using UnityEngine;

public class PlayerAnimationController : AnimationControllerMulti
{
    [Header("Animations")]

    [SerializeField] private PlayerCharacter owner;

    [SerializeField] private Transform armsParent;

    [field: SerializeField] public CustomBoneFollow leftArmBone { get; private set; }
    [field: SerializeField] public CustomBoneFollow rightArmBone { get; private set; }

    [SerializeField][ReadOnly] private string currentState = "N/A";

    [field: SerializeField][field: ReadOnly] public bool isValid { get; private set; }


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

    protected override void Start()
    {
        base.Start();
        foreach (Transform item in CurrentSkeletonAnimation.transform)
        {
            if (item.GetComponent<SkeletonAnimation>() != null)
            {
                item.gameObject.AddComponent<RendererSorting>();

                if (item.GetComponent<SkeletonAnimation>().skeletonDataAsset == jasonSkeletonDataAsset)
                {
                    SkeletonRendererCustomMaterials ovm = jasonMaterialOverride_PF.gameObject.Create<SkeletonRendererCustomMaterials>(item);
                    ovm.skeletonRenderer = item.GetComponent<SkeletonRenderer>();
                    JasonMaterialOverride = ovm;
                    JasonMaterialOverride.gameObject.SetActive(false);
                }
            }
        }

        CurrentSkeletonAnimation._switchSkeleton += SwitchSkeleton;
    }

    private void SetAnimationFromState(FSM_Player_Manager.E_PlayerState newState)
    {
        if (!owner.AnimationsData.StateAnimationData.TryGetValue(newState, out SCRPT_PlayersAnimData.S_StateAnimationData anim))
        {
            this.Log("Could not find " + newState + " in animation data " + owner.AnimationsData.StateAnimationData, LogsManager.E_LogType.Error);
            return;
        }

        SetAnimation(anim.Asset, anim.Loop);
    }
    
    public void SwitchSkeleton()
    {
        foreach (Transform arm in armsParent)
        {
            arm.GetComponent<CustomBoneFollow>().skeletonRenderer = CurrentSkeletonAnimation.CurrentSkeletonAnimation;
        }

        owner.SkeletonAnimation = CurrentSkeletonAnimation.CurrentSkeletonAnimation;
    }

    public override void FlipSkeleton()
    {
        Vector3 v = leftArmBone.offset;
        v.x *= -1;
        leftArmBone.offset = v;

        v = rightArmBone.offset;
        v.x *= -1;
        rightArmBone.offset = v;
    }
}

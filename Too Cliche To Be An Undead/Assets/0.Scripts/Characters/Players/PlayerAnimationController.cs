using BalDUtilities.MouseUtils;
using Spine.Unity;
using UnityEngine;

public class PlayerAnimationController : AnimationControllerMulti
{
    [Header("Animations")]

    private PlayerCharacter ownerCharacter;
    private FSM_PlayerCharacter ownerFSM;
    private WeaponHandler ownerWeaponhandler;
    private AimDirection ownerAim;

    private SO_PlayersAnimData currentAnimData;

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

    }

    private void DelayedEventsSubscriber()
    {
        if (owner.HolderTryGetComponent(IComponentHolder.E_Component.FSM, out ownerFSM) == IComponentHolder.E_Result.Success)
            ownerFSM.OnStateChange += SetAnimationFromState;

        ownerCharacter.OnCharacterSwitch += OnOwnerCharacterSwitch;

        if (ownerWeaponhandler.CurrentWeapon != null)
            OnOwnerNewWeapon(ownerWeaponhandler.CurrentWeapon);
        ownerWeaponhandler.OnCreatedNewWeapon += OnOwnerNewWeapon;
    }

    protected override void EventsUnSubscriber()
    {
        ownerFSM.OnStateChange -= SetAnimationFromState;
        ownerCharacter.OnCharacterSwitch -= OnOwnerCharacterSwitch;
        ownerWeaponhandler.CurrentWeapon.OnStartAttack -= SetDirectionalAnimationFromPlayerAim;
        ownerWeaponhandler.OnCreatedNewWeapon -= OnOwnerNewWeapon;
    }

    protected override void Setup()
    {
        base.Setup();
        ownerCharacter = owner as PlayerCharacter;
        owner.HolderTryGetComponent(IComponentHolder.E_Component.WeaponHandler, out ownerWeaponhandler);
        owner.HolderTryGetComponent(IComponentHolder.E_Component.Aimer, out ownerAim);
        SetupSkeletonsComponents();
        DelayedEventsSubscriber();

        currentAnimData = ownerCharacter.CurrentCharacterComponents.AnimData;
        SetAnimationFromState(ownerFSM.BaseStateKey);
    }

    private void OnOwnerNewWeapon(PlayerWeaponBase newWeapon)
    {
        newWeapon.OnStartAttack += SetDirectionalAnimationFromPlayerAim;
    }

    private void SetupSkeletonsComponents()
    {
        foreach (Transform item in CurrentMultiSkeletonAnimation.transform)
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
    }

    private void SetAnimationFromState(FSM_PlayerCharacter.E_PlayerStates newState)
    {
        if (!currentAnimData.TryGetAnimationData(newState, out var anim)) return;
        SetAnimation(anim.Asset, anim.Loop);
    }

    private void SetDirectionalAnimationFromPlayerAim(S_DirectionalAnimationData animations)
    {
        switch (ownerAim.GetGeneralAimDirectionEnum())
        {
            case GameManager.E_Direction.Left:
                SetAnimation(animations.SideAnimation, animations.Loop);
                this.TryFlipSkeleton(false);
                break;
            case GameManager.E_Direction.Right:
                SetAnimation(animations.SideAnimation, animations.Loop);
                this.TryFlipSkeleton(true);
                break;
            case GameManager.E_Direction.Up:
                SetAnimation(animations.BackAnimation, animations.Loop);
                break;
            case GameManager.E_Direction.Down:
                SetAnimation(animations.FrontAnimation, animations.Loop);
                break;
        }
    }

    public override void SwitchSkeleton()
    {
        base.SwitchSkeleton();
        foreach (Transform arm in armsParent)
        {
            arm.GetComponent<CustomBoneFollow>().skeletonRenderer = CurrentMultiSkeletonAnimation.CurrentSkeletonAnimation;
        }
    }

    public override void FlipSkeleton()
    {
        base.FlipSkeleton();
        Vector3 v = leftArmBone.offset;
        v.x *= -1;
        leftArmBone.offset = v;

        v = rightArmBone.offset;
        v.x *= -1;
        rightArmBone.offset = v;
    }

    private void OnOwnerCharacterSwitch(SO_CharactersComponents ccp)
    {
        this.currentAnimData = ccp.AnimData;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Text;
using Spine.Unity;

public class PlayerCharacter : Entity, IInteractable
{
    public event Action<SO_CharactersComponents> OnCharacterSwitch;

    [field: SerializeField] public int PlayerIndex { get; private set; }

    [field: SerializeField] public FSM_PlayerCharacter StateManager { get; private set; } 

    [SerializeField] private SO_PCCHolder pccHolder;
    [field: SerializeField, ReadOnly] public SO_CharactersComponents CurrentCharacterComponents { get; private set; }
    [SerializeField] private TextMeshPro selfReviveText;
    [SerializeField] private GameObject pivotOffset;

    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Material defaultMaterial;

    [SerializeField] private float dyingState_DURATION = 20f;
    [SerializeField][Range(0, 1)] private float reviveHealPercentage = 0.25f;
    public float ReviveHealthPercentage { get => reviveHealPercentage; }

    public enum E_Devices
    {
        Keyboard,
        Xbox,
        DualShock,
        Switch,
        None,
    }

    public GameObject PivotOffset { get => pivotOffset; }
    public TextMeshPro SelfReviveText { get => selfReviveText; }
    public float DyingState_DURATION { get => dyingState_DURATION; }

    private EntityEvents.OnEntityDamagesData<PlayerCharacter> lastDamagesData;

    public Action OnSwitchCharacter;
    public Action<int> OnIndexChange;

    public event Action<GameObject> OnOtherInteract;
    public Action OnInteractInput;


    #region A/S/U/F

    protected override void Awake()
    {
        base.Awake();
        lastDamagesData = new(this, null, 0);
    }

    protected override void EventsSubscriber()
    {
        base.EventsSubscriber();
        ShopEvents.OnBoughtNewLevel += ReceiveShopUpgrade;
    }

    protected override void EventsUnSubscriber()
    {
        base.EventsUnSubscriber();
        ShopEvents.OnBoughtNewLevel -= ReceiveShopUpgrade;
    }

    public void Setup(PlayerInputHandler inputs)
    {
        Setup(inputs, inputs.InputsID);
    }
    public void Setup(PlayerInputHandler inputs, int index)
    {
        HolderChangeComponent(IComponentHolder.E_Component.PlayerInputsComponent, inputs);
        this.PlayerIndex = index;
        inputs.SetOwner(this);

        this.SwitchCharacter(pccHolder.GetComponents(this.GetCharacterName()));
        this.PlayerSetup();
    }

    #endregion

    #region Skill

    //public void ResetSkillAnimator()
    //{
    //    this.GetSkillHolder.GetAnimator.ResetTrigger("EndSkill");
    //    this.GetSkillHolder.GetAnimator.Play("MainState");
    //}

    //public void OffsetSkillHolder(float offset)
    //{
    //    skillHolder.transform.localPosition = (Vector3)weapon.GetGeneralDirectionOfMouseOrGamepad() * offset;
    //}
    //public void OffsetChild(float offset)
    //{
    //    skillHolder.transform.GetChild(0).localPosition = Vector2.down * offset;
    //}

    //public void SetArmsState(bool active, Vector3 newOffset, int skeletonIdx = 0, string boneToFollow = "")
    //{
    //    if (boneToFollow != "")
    //        animationController.leftArmBone.SetBone(boneToFollow);
    //    animationController.leftArmBone.SkeletonRenderer = animationController.GetSkeletonOf(skeletonIdx);

    //    if (animationController.IsLookingAtRight())
    //    {
    //        Vector3 v = newOffset;
    //        v.x *= -1;
    //        newOffset = v;
    //    }

    //    animationController.leftArmBone.offset = newOffset;

    //    animationController.leftArmBone.followXPosition = newOffset.x != 0;
    //    animationController.leftArmBone.followYPosition = newOffset.y != 0;

    //    if (boneToFollow != "")
    //        animationController.rightArmBone.SetBone(boneToFollow);
    //    animationController.rightArmBone.SkeletonRenderer = animationController.GetSkeletonOf(skeletonIdx);
    //    animationController.rightArmBone.offset = newOffset;

    //    animationController.rightArmBone.followXPosition = newOffset.x != 0;
    //    animationController.rightArmBone.followYPosition = newOffset.y != 0;

    //    leftArm.gameObject.SetActive(active);
    //    rightArm.gameObject.SetActive(active);
    //}

    //public void RotateSkillHolder()
    //{
    //    Vector3 rot = weapon.GetRotationOnMouseOrGamepad().eulerAngles;
    //    rot.z -= 90;
    //    skillHolder.transform.eulerAngles = rot;
    //}
    //public void RotateArms()
    //{
    //    //Vector3 rot = weapon.GetRotationOnMouseOrGamepad().eulerAngles;
    //    Quaternion rot = Quaternion.identity;

    //    if (PlayerInputsComponent.currentDeviceType != PlayerInputsManager.E_Devices.Keyboard)
    //    {
    //        float lookAngle = Mathf.Atan2(aimGoal.y, aimGoal.x) * Mathf.Rad2Deg;
    //        rot = Quaternion.Slerp(weapon.transform.rotation, Quaternion.AngleAxis(lookAngle + 180, Vector3.forward), Time.deltaTime * weapon.SlerpSpeed);

    //        animationController.TryFlipSkeleton((rot.eulerAngles.z < 270) && (rot.eulerAngles.z > 90));
    //    }
    //    else rot = weapon.GetRotationOnMouse();

    //    bool flipArms = (rot.eulerAngles.z < 270) && (rot.eulerAngles.z > 90);

    //    Vector2 armsScale = armsParent.localScale;
    //    armsScale.x = flipArms ? 1 : -1;
    //    armsParent.localScale = armsScale;

    //    if (flipArms)
    //    {
    //        Vector3 euler = rot.eulerAngles;
    //        euler.z += 180;
    //        rot.eulerAngles = euler;
    //    }

    //    leftArm.transform.rotation = rot;
    //    rightArm.transform.rotation = rot;
    //}

    #endregion

    #region Dash / Push

    //public void StartDashTimer() => dash_CD_TIMER = MaxDashCD_M;

    //private float CalculateAllKeys()
    //{
    //    float res = 0;
    //    for (int i = 0; i < playerDash.DashSpeedCurve.length; i++)
    //    {
    //        if (i + 1 < playerDash.DashSpeedCurve.length)
    //            res += CalculateSingleKey(i, i + 1);
    //    }

    //    return res;
    //}
    //private float CalculateSingleKey(int index1, int index2)
    //{
    //    Keyframe startKey = playerDash.DashSpeedCurve[index1];
    //    Keyframe endKey = playerDash.DashSpeedCurve[index2];

    //    float res = ((startKey.value + endKey.value) / 2) * (endKey.time - startKey.time);

    //    return res;
    //}

    #endregion

    public void ForceSetIndex(int idx)
    {
        this.PlayerIndex = idx;
        this.gameObject.name = "Player " + idx;

        OnIndexChange?.Invoke(idx);
    }

    #region Tweening

    //public void ScaleTweenObject(GameObject target)
    //{
    //    if (target == null) return;

    //    LeanTween.cancel(target);

    //    LeanTween.scale(target, iconsMaxScale, maxScaleTime).setEase(inType)
    //    .setOnComplete(() =>
    //    {
    //        LeanTween.scale(target, Vector3.one, maxScaleTime).setEase(outType);
    //    });
    //}
    //public void ScaleTweenObject(GameObject target, LeanTweenType _inType, LeanTweenType _outType)
    //{
    //    if (target == null) return;

    //    LeanTween.cancel(target);

    //    LeanTween.scale(target, iconsMaxScale, maxScaleTime).setEase(_inType)
    //    .setOnComplete(() =>
    //    {
    //        LeanTween.scale(target, Vector3.one, maxScaleTime).setEase(_outType);
    //    });
    //}
    //public void ScaleTweenObject(RectTransform target)
    //{
    //    if (target == null) return;

    //    LeanTween.cancel(target);

    //    LeanTween.scale(target, iconsMaxScale, maxScaleTime).setEase(inType)
    //    .setOnComplete(() =>
    //    {
    //        LeanTween.scale(target, Vector3.one, maxScaleTime).setEase(outType);
    //    });
    //}

    #endregion

    #region Interactions

    public void EnteredInRange(GameObject interactor)
    {
    }

    public void ExitedRange(GameObject interactor)
    {
    }

    public void Interact(GameObject interactor)
    {
        OnOtherInteract?.Invoke(interactor);
    }

    public bool CanBeInteractedWith() => this.StateManager.CurrentStateKey == FSM_PlayerCharacter.E_PlayerStates.Dying;

    public float GetDistanceFrom(Transform target) => Vector2.Distance(this.transform.position, target.position);
 
    #endregion

    private void ReceiveShopUpgrade(SO_ShopUpgrade data)
    {
        foreach (var item in data.Modifiers)
        {
            //StatsHandler.TryAddModifier(item, out var result);
        }
        //this.selfReviveCount += data.RevivesToAdd;
    }

    #region Switch & Set

    public void SwitchCharacter(SO_CharactersComponents characterData)
    {
        this.CurrentCharacterComponents = characterData;
        this.OnCharacterSwitch?.Invoke(characterData);
    }
    public void SwitchCharacter(SCRPT_Dash newDash, SCRPT_Skill newSkill, SCRPT_EntityStats newStats, GameManager.E_CharactersNames character, SO_PlayersAnimData animData, SCRPT_PlayerAudio audioData, bool callDelegate = true)
    {
        //DataKeeper.Instance.playersDataKeep[this.playerIndex].character = character;

        //this.playerDash = newDash;

        //this.skillHolder.ChangeSkill(newSkill);

        //this.audioClips = audioData;

        //this.AnimationsData = animData;

        //this.animationController.CurrentMultiSkeletonAnimation.CurrentSkeletonAnimation.Skeleton.SetColor(Color.white);

        //if (animData != null && animData.arms.Length > 0)
        //    this.leftArm.GetComponent<SpriteRenderer>().sprite = animData.arms[0];
        //if (animData != null && animData.arms.Length > 1)
        //    this.rightArm.GetComponent<SpriteRenderer>().sprite = animData.arms[1];

        //bool foundAnim = AnimationsData.TryGetAnimationData(FSM_Player_Manager.E_PlayerState.Attacking, out SCRPT_PlayersAnimData.S_StateAnimationData data);
        //if (foundAnim)
        //    weapon.attackDuration = data.AnimationDuration;

        //CameraManager.Instance.Markers[this.playerIndex].gameObject.SetActive(false);

        //if (callDelegate) OnSwitchCharacter?.Invoke();
    }

    #endregion

    public override void Stun(float duration, bool resetAttackTimer = false, bool showStuntext = false)
    {
        AskForStun(duration, resetAttackTimer, showStuntext);
    }

    public GameManager.E_CharactersNames GetCharacterName()
    {
        return DataKeeper.Instance.playersDataKeep[this.PlayerIndex].character;
    }

    public GameManager.E_CharactersNames GetCharacterName(int idx)
    {
        if (idx > DataKeeper.Instance.playersDataKeep.Count) return GameManager.E_CharactersNames.Shirley;
        
        return DataKeeper.Instance.playersDataKeep[idx].character;
    }

    public void TryPlayAudioclip(AudioClip clip, float pitchRange = 0)
    {
        //this.source.pitch = UnityEngine.Random.Range(1 - pitchRange, 1 + pitchRange);
        //this.source?.PlayOneShot(clip);
    }
}

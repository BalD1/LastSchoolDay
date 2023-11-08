using Spine;
using Spine.Unity;
using UnityEngine;

public class BossAnimationsController : AnimationControllerSingle
{
    [Header("Animations")]

    [SerializeField] private BossZombie owner;

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
        owner.OnBossStateChange += SetAnimationFromState;
    }

    protected override void EventsUnSubscriber()
    {
        owner.OnBossStateChange -= SetAnimationFromState;
    }

    protected override void Setup()
    {
        base.Setup();
        SetAnimation(owner.AnimationData.WalkAnim, true);
        isValid = true;
    }

    private void SetAnimationFromState(FSM_Boss_Manager.E_BossState newState)
    {
        //if (!owner.AnimationData.StateAnimationData.TryGetValue(newState, out SCRPT_BossAnimData.S_StateAnimationData anim))
        //{
        //    this.Log("Could not find " + newState + " in animation data " + owner.AnimationData.StateAnimationData, CustomLogger.E_LogType.Error);
        //    return;
        //}

        //SetAnimation(anim.Asset, anim.Loop);
    }

    private void SetAnimationInspector()
    {
#if UNITY_EDITOR
        SetAnimation(editor_AnimationToSet, editor_loopAnimation);
#endif
    }
}

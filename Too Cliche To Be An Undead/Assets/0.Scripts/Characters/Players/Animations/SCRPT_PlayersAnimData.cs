using AYellowpaper.SerializedCollections;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayersAnimationData", menuName = "Scriptable/Entity/AnimationData/Player")]

public class SCRPT_PlayersAnimData : SCRPT_AnimationsData<FSM_Player_Manager.E_PlayerState>
{
    [field: Header("Player general Animations")]

    [field: SerializeField] public AnimationReferenceAsset IdleAnim { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset WalkAnim { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset AttackAnim_side {  get; private set; }
    [field: SerializeField] public AnimationReferenceAsset AttackAnim_up {  get; private set; }
    [field: SerializeField] public AnimationReferenceAsset AttackAnim_down {  get; private set; }

    [field: SerializeField] public AnimationReferenceAsset DashAnim { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset SkillTransitionAnim { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset SkillIdleAnimSide { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset SkillWalkAnimSide { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset SkillIdleAnimUp { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset SkillIdleAnimDown { get; private set; }

    [field: SerializeField] public S_StateAnimationData[] StateAnimation { get; private set; }

    [field: SerializeField] public Sprite[] arms { get; private set; }
}
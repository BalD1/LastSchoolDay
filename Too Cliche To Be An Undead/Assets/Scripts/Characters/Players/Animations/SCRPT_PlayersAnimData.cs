using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayersAnimationData", menuName = "Scriptable/Entity/AnimationData/Player")]

public class SCRPT_PlayersAnimData : SCRPT_AnimationsData
{
    [field: Header("Player general Animations")]

    [field: SerializeField] public AnimationReferenceAsset dashAnim { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset skillTransitionAnim { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset skillIdleAnimSide { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset skillWalkAnimSide { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset skillIdleAnimUp { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset skillIdleAnimDown { get; private set; }

    [field: SerializeField] public Sprite[] arms { get; private set; }
}
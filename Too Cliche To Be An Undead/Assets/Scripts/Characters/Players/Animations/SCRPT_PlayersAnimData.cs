using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayersAnimationData", menuName = "Scriptable/Entity/AnimationData/Player")]

public class SCRPT_PlayersAnimData : SCRPT_AnimationsData
{
    [field: Header("Shirley Animations")]

    [field: SerializeField] public AnimationReferenceAsset dashAnim { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset skillTransitionAnim { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset skillIdleAnim { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset skillWalkAnim { get; private set; }
}
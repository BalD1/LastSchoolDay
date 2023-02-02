using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "Scriptable/Entity/AnimationData/Enemies/Base")]
public class SCRPT_ZombiesAnimDatga : SCRPT_AnimationsData
{
    [field: Header("Zombies Animations")]

    [field: SerializeField] public AnimationReferenceAsset attackAnticip_Side { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset attackAnticip_Up { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset attackAnticip_Down { get; private set; }
}
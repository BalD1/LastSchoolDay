using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "Scriptable/Entity/AnimationData/Base")]
public class SCRPT_AnimationsData : ScriptableObject
{
    public enum E_Characters
    {
        Shirley,
        Whitney,
        Jason,
        Nelson,

        Zombie_01,
        Zombie_02,
        Zombie_03,
        Zombie_04,
    }

    [field: SerializeField] public E_Characters character { get; private set; }

    [field: SerializeField] public SkeletonDataAsset skeletonDataAsset { get; private set; }

    [field: Header("General Animations")]

    [field: SerializeField] public AnimationReferenceAsset idleAnim { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset walkAnim { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset attackAnim_side { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset attackAnim_up { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset attackAnim_down { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset deathAnim { get; private set; }
}
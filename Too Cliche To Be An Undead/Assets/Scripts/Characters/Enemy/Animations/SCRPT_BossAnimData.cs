using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SCRPT_AnimationsData;

[CreateAssetMenu(fileName = "BossAnimData", menuName = "Scriptable/Entity/AnimationData/Enemies/Boss")]
public class SCRPT_BossAnimData : ScriptableObject
{
    [field: SerializeField] public SkeletonDataAsset skeletonDataAsset { get; private set; }

    [field: Header("General Animations")]

    [SerializeField] private AnimationReferenceAsset[] idleAnim;
    [SerializeField] private AnimationReferenceAsset[] walkAnim;
    [field: SerializeField] public AnimationReferenceAsset AttackAnticipAnim { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset YellAnim { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset DeathAnim { get; private set; }

    public AnimationReferenceAsset IdleAnim { get => idleAnim.RandomElement(); }
    public AnimationReferenceAsset WalkAnim { get => walkAnim.RandomElement(); }
}
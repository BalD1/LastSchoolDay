using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "BossAnimData", menuName = "Scriptable/Entity/AnimationData/Enemies/Boss")]
public class SCRPT_BossAnimData : SCRPT_AnimationsData<FSM_Boss_Manager.E_BossState>
{
    [field: SerializeField] public SkeletonDataAsset SkeletonDataAsset { get; private set; }

    [field: Header("General Animations")]

    [field: SerializeField] public AnimationReferenceAsset IdleAnim { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset WalkAnim { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset AttackAnticipAnim { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset YellAnim { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset JumpStartAnim { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset JumpEndAnim { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset DeathAnim { get; private set; }
}
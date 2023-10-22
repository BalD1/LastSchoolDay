using AYellowpaper.SerializedCollections;
using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "BossAnimData", menuName = "Scriptable/Entity/AnimationData/Enemies/Boss")]
public class SCRPT_BossAnimData : SCRPT_AnimationsData<FSM_Boss_Manager.E_BossState>
{
    [field: SerializeField] public SerializedDictionary<FSM_Boss_Manager.E_BossState, S_StateAnimationData> StateAnimationData { get; private set;}
    [field: SerializeField] public SkeletonDataAsset SkeletonDataAsset { get; private set; }

    [field: Header("General Animations")]

    [field: SerializeField] public AnimationReferenceAsset IdleAnim { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset WalkAnim { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset AttackAnticipAnim { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset YellAnim { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset JumpStartAnim { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset JumpEndAnim { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset DeathAnim { get; private set; }

    public override bool TryGetAnimationData(FSM_Boss_Manager.E_BossState key, out S_StateAnimationData animationData)
    {
        bool result = Intern_TryGetAnimationData(key, StateAnimationData, out animationData);
        return result;
    }
}
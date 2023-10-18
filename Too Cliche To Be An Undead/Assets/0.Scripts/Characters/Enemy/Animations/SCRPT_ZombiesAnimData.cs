using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "Scriptable/Entity/AnimationData/Enemies/Base")]
public class SCRPT_ZombiesAnimData : SCRPT_AnimationsData<FSM_NZ_Manager.E_NZState>
{
    [field: Header("Zombies Animations")]
    [field: SerializeField] public AnimationReferenceAsset IdleAnim { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset WalkAnim { get; private set; }

    [SerializeField] private AnimationReferenceAsset[] attackAnticip_Side;
    [SerializeField] private AnimationReferenceAsset[] attackAnticip_Up;
    [SerializeField] private AnimationReferenceAsset[] attackAnticip_Down;

    public AnimationReferenceAsset AttackAnticip_Side { get => attackAnticip_Side.RandomElement(); }
    public AnimationReferenceAsset GetAnticipAttackAnimSide(int idx) => attackAnticip_Side[idx];
    public AnimationReferenceAsset AttackAnticip_Up { get => attackAnticip_Up.RandomElement(); }
    public AnimationReferenceAsset GetAnticipAttackAnimUp(int idx) => attackAnticip_Up[idx];
    public AnimationReferenceAsset AttackAnticip_Down { get => attackAnticip_Down.RandomElement(); }
    public AnimationReferenceAsset GetAnticipAttackAnimDown(int idx) => attackAnticip_Down[idx];
}
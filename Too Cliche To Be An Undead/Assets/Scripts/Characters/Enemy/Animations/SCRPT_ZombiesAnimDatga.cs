using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "Scriptable/Entity/AnimationData/Enemies/Base")]
public class SCRPT_ZombiesAnimDatga : SCRPT_AnimationsData
{
    [field: Header("Zombies Animations")]

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
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "Scriptable/Entity/AnimationData/Players/Base")]
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

    [SerializeField] private AnimationReferenceAsset[] idleAnim;
    [SerializeField] private AnimationReferenceAsset[] walkAnim;
    [SerializeField] private AnimationReferenceAsset[] attackAnim_side;
    [SerializeField] private AnimationReferenceAsset[] attackAnim_up;
    [SerializeField] private AnimationReferenceAsset[] attackAnim_down;
    [SerializeField] private AnimationReferenceAsset[] death;

    public AnimationReferenceAsset IdleAnim { get => idleAnim.RandomElement(); }
    public AnimationReferenceAsset WalkAnim { get => walkAnim.RandomElement(); }
    public AnimationReferenceAsset AttackAnim_side { get => attackAnim_side.RandomElement(); }
    public AnimationReferenceAsset GetAttackAnimSide(int idx) => attackAnim_side[idx];
    public AnimationReferenceAsset AttackAnim_up { get => attackAnim_up.RandomElement(); }
    public AnimationReferenceAsset GetAttackAnimUp(int idx) => attackAnim_up[idx];
    public AnimationReferenceAsset AttackAnim_down { get => attackAnim_down.RandomElement(); }
    public AnimationReferenceAsset GetAttackAnimDown(int idx) => attackAnim_down[idx];
    public AnimationReferenceAsset DeathAnim { get => death.RandomElement(); }
}
using Spine.Unity;
using TNRD;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponData", menuName = "Scriptable/Entity/WeaponData")]
public class SO_WeaponData : ScriptableObject
{
    [field: SerializeField] public LayerMask DamageableLayer { get; private set; }
    [field: SerializeField] public INewDamageable.E_DamagesType DamagesType { get; private set; }

    [System.Serializable]
    public struct S_AttackData
    {
        [field: SerializeField] public float DamagesMultiplier { get; private set; }
        [field: SerializeField] public float PushPercentage { get; private set; }
        [field: SerializeField] public float AttackMovementMomentum { get; private set; }
        [field: SerializeField] public float AttackMovementForce { get; private set; }
        [field: SerializeField] public float NextAttackTimer { get; private set; }
        [field: SerializeField] public float BreakComboTimer { get; private set; }
        [field: SerializeField] public float AttackDuration { get; private set; }
        [field: SerializeField] public float RangeModifier { get; private set; }
        [field: SerializeField] public S_DirectionalAnimationData Animations { get; private set; }
        [field: SerializeField] public float KnockbackForce { get; private set; }
        [field: SerializeField] public bool UseKnockbackAsPush { get; private set; }
        [field: SerializeField] public SimpleParticlesPlayer Effect { get; private set; }
    }
    [field: SerializeField] public S_AttackData[] AttackData { get; private set; }
}
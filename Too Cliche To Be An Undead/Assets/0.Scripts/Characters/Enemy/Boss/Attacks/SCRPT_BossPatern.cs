using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossPatern", menuName = "Scriptable/Entity/Enemy/Boss/Patern")]
public class SCRPT_BossPatern : ScriptableObject
{
    [System.Serializable]
    public struct S_AttackAndCooldown
    {
        [field: SerializeField] public SO_EnemyAttack attack { get; private set; }
        [field: SerializeField] public float timeBeforeNext { get; private set; }
    }
    [System.Serializable]
    public struct S_Paterns
    {
        [field: SerializeField] public S_AttackAndCooldown[] attackWithCooldown { get; private set; }
        [field: SerializeField] public float recoverTime { get; private set; }
    }

    [field: SerializeField] public float pitchModifierOnStageChange { get; private set; } 
    [field: SerializeField] public S_Paterns[] firstStagePaterns { get; private set; }
    [field: SerializeField] public S_Paterns[] secondStagePaterns { get; private set; }

    public S_Paterns GetRandomPaternOfStage(int stage)
    {
        return stage == 0 ? firstStagePaterns.RandomElement() : secondStagePaterns.RandomElement();
    }
}
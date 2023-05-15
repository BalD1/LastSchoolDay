using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoisonData", menuName = "Scriptable/Entity/Skills/Smokescreen/Poison")]
public class SCRPT_PoisonData : ScriptableObject
{
    [field: SerializeField] public float OwnerDamagesMultiplier { get; private set; }
    [field: SerializeField] public float PoisonTimeBetweenTicks { get; private set; }
    [field: SerializeField] public float PoisonDuration { get; private set; }
    [field: SerializeField] [field: Range(0,100)] public int PoisonCritChances { get; private set; }

    [field: SerializeField] public string PoisonID { get; private set; }
}
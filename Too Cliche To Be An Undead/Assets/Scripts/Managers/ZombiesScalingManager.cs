using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZombiesScalingManager : MonoBehaviour
{
    private static ZombiesScalingManager instance;
    public static ZombiesScalingManager Instance
    {
        get
        {
            return instance;
        }
    }

    [System.Serializable]
    public struct S_ModifiersByStamp
    {
        [SerializeField] private S_ModifierData[] modifiers;

        public S_ModifierData[] Modifiers { get => modifiers; }
    }
    [SerializeField] private S_ModifiersByStamp[] modifiersByStamp;

    public S_ModifiersByStamp[] ModifiersByStamps { get => modifiersByStamp; }

    [System.Serializable]
    public class S_ModifierData
    {
        [SerializeField] private string modifierID = "SCALE_X_";
        public string ModifierID { get => modifierID; }
        [field: SerializeField] public float Value { get; private set; }
        [field: SerializeField] public StatsModifier.E_StatType StatType { get; private set; }

        public void SetID(string newID)
        {
            modifierID = newID;
        }
    }

    [field: SerializeField, ReadOnly] public float TotalSpeedAddition { get; private set; }

    [field: SerializeField] public float MaxSteeringMaxForce { get; private set; }
    [field: SerializeField] public float MaxSteeringMass { get; private set; }
    [field: SerializeField] public float MaxTargetPositionPredictTime { get; private set; }

    public delegate void D_OnSendModifiers(List<S_ModifierData> modifiers);
    public D_OnSendModifiers D_onSendModifiers;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SpawnersManager.Instance.D_stampChange += OnStampChange;

        foreach (var item in ModifiersByStamps)
        {
            foreach (var modifier in item.Modifiers)
            {
                if (modifier.StatType == StatsModifier.E_StatType.Speed)
                    TotalSpeedAddition += modifier.Value;
            }
        }
    }

    private void OnStampChange(int newStamp)
    {
        if (newStamp >= modifiersByStamp.Length) return;

        List<S_ModifierData> modifList = new List<S_ModifierData>();
        modifList = modifiersByStamp[newStamp].Modifiers.ToList();

        D_onSendModifiers?.Invoke(modifList);
    }


}

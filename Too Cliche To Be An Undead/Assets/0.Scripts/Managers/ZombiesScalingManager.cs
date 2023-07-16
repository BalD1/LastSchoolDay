using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZombiesScalingManager : Singleton<ZombiesScalingManager>
{
    [System.Serializable]
    public struct S_ModifiersByStamp
    {
        [SerializeField] private S_ModifierData[] modifiers;
        public Action<EnemyBase> Actions { get; set; }

        public S_ModifierData[] Modifiers { get => modifiers; }
    }
    [SerializeField] private S_ModifiersByStamp[] modifiersByStamp;

    public S_ModifiersByStamp[] ModifiersByStamps { get => modifiersByStamp; }

    [field: SerializeField, ReadOnly] 
    public List<S_ModifierData> CurrentActiveModifiers { get; private set; }

    public int CurrentStamp
    {
        get
        {
            if (SpawnersManager.Instance == null) return -1;
            return SpawnersManager.Instance.SpawnStamp;
        }
    }

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

    public delegate void D_OnSendModifiers();
    public D_OnSendModifiers D_onSendModifiers;

    protected override void Awake()
    {
        base.Awake();
        modifiersByStamp[4].Actions += (EnemyBase owner) =>
        {
            owner.enemiesBlocker.enabled = false;
        };
    }

    protected override void Start()
    {
        base.Start();
        if (SpawnersManager.Instance == null) return;

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

        CurrentActiveModifiers.AddRange(modifList);

        D_onSendModifiers?.Invoke();
    }


}

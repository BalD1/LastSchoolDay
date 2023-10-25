using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[RequireComponent(typeof(StatsHandler))]
public class HealthSystem : MonoBehaviourEventsHandler, INewDamageable
{
    [field: SerializeField] public StatsHandler Stats { get; private set; }

    [field: SerializeField, ReadOnly] public float CurrentHealth { get; protected set; }
    [field: SerializeField, ReadOnly] public float CurrentMaxHealth { get; protected set; }

    public Dictionary<string, NewTickDamages> UniqueTickDamages { get; private set; } = new Dictionary<string, NewTickDamages>();
    public Dictionary<string, List<NewTickDamages>> StackableTickDamages { get; private set; } = new Dictionary<string, List<NewTickDamages>>();

    public event Action OnTookDamages;
    public event Action OnHealed;
    public event Action OnDeath;

    protected override void EventsSubscriber()
    {
        Stats.OnStatChange += OnStatChange;
    }

    protected override void EventsUnSubscriber()
    {
        Stats.OnStatChange -= OnStatChange;
    }

    protected override void Awake()
    {
        if (Stats == null) Stats = this.GetComponent<StatsHandler>();
        base.Awake();
        Setup();
    }

    private void OnStatChange(StatsHandler.StatChangeEventArgs args)
    {
        if (args.Type == IStatContainer.E_StatType.MaxHP) UpdateMaxHealth(args.FinalValue, true);
    }

    private void Setup()
    {
        float maxHealth = -1;
        if (!Stats.BrutFinalStats.TryGetValue(IStatContainer.E_StatType.MaxHP, out maxHealth))
            this.Log("Could not find max HP in Stats");

        UpdateMaxHealth(maxHealth, false);
        CurrentHealth = maxHealth;
    }

    public void UpdateMaxHealth(float newHealth, bool healDifference)
    {
        float pastHealth = CurrentMaxHealth;

        if (!Stats.TryGetFinalStat(IStatContainer.E_StatType.MaxHP, out float maxAllowedHealth))
            maxAllowedHealth = newHealth;

        CurrentMaxHealth = Mathf.Clamp(CurrentHealth + newHealth, 0, maxAllowedHealth);
        if (healDifference)
        {
            float diffenrece = newHealth - pastHealth;
            if (diffenrece > 0)
                this.Heal(diffenrece, false);
        }
    }

    public void InflictDamages(float amount, bool isCrit)
    {
        if (!IsAlive()) return;

        float finalDamages = amount;
        if (isCrit) finalDamages *= GameManager.CRIT_MULTIPLIER;

        CurrentHealth -= finalDamages;

        this.OnHealed?.Invoke();
    }

    public void Heal(float amount, bool isCrit)
    {
        if (!IsAlive()) return;

        float finalHeal = amount;
        if (isCrit) finalHeal *= GameManager.CRIT_MULTIPLIER;

        CurrentHealth = Mathf.Clamp(CurrentHealth + finalHeal, 0, CurrentMaxHealth);

        this.OnHealed?.Invoke();
    }

    public bool IsAlive()
        => CurrentHealth > 0;

    public void Death()
    {
        this.OnDeath?.Invoke();
    }

    public bool TryAddTickDammages(SO_TickDamagesData data, NewEntity origin)
    {
        if (data.Stackable)
        {
            if (!StackableTickDamages.ContainsKey(data.ID))
                StackableTickDamages.Add(data.ID, new List<NewTickDamages>());

            StackableTickDamages[data.ID].Add(new NewTickDamages(data, this, origin));
            Debug.Log("added " + data.ID);
            return true;
        }

        if (UniqueTickDamages.ContainsKey(data.ID)) return false;
        UniqueTickDamages.Add(data.ID, new NewTickDamages(data, this, origin));
        Debug.Log("added " + data.ID);

        return true;
    }

    public void RemoveTickDamage(NewTickDamages tick)
    {
        if (tick.Data.Stackable)
        {
            Debug.Log("removed " + tick.Data.ID);
            StackableTickDamages[tick.Data.ID].Remove(tick);
            return;
        }

        Debug.Log("removed " + tick.Data.ID);
        UniqueTickDamages.Remove(tick.Data.ID);
    }
}

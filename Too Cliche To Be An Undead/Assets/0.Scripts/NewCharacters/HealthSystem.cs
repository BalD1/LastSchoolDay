using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static INewDamageable;

[RequireComponent(typeof(StatsHandler))]
public class HealthSystem : MonoBehaviourEventsHandler, INewDamageable
{
    [field: SerializeField] public StatsHandler Stats { get; private set; }

    [field: SerializeField, ReadOnly] public float CurrentHealth { get; protected set; }
    [field: SerializeField, ReadOnly] public float CurrentMaxHealth { get; protected set; }

    public Dictionary<string, NewTickDamages> UniqueTickDamages { get; private set; } = new Dictionary<string, NewTickDamages>();
    public Dictionary<string, List<NewTickDamages>> StackableTickDamages { get; private set; } = new Dictionary<string, List<NewTickDamages>>();

    [field: SerializeField] public Vector2 HealthPopupOffset { get; private set; }

    [field: SerializeField, ReadOnly] public float InvincibilityTimer { get; protected set; }

    public event Action<INewDamageable.DamagesData> OnTookDamages;
    public event Action OnHealed;
    public event Action OnDeath;

    private void Reset()
    {
        Stats = this.GetComponent<StatsHandler>();
    }

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

    protected virtual void Update()
    {
        if (InvincibilityTimer > 0) InvincibilityTimer -= Time.deltaTime;
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

    public virtual bool TryInflictDamages(DamagesData damagesData)
    {
        if (!IsAlive()) return false;
        if (InvincibilityTimer > 0) return false;
        if (this.Stats.GetTeam() != SO_BaseStats.E_Team.Neutral && 
            this.Stats.GetTeam() == damagesData.DamagerTeam) return false;

        InflictDamages(damagesData);
        return true;
    }
    public virtual void InflictDamages(DamagesData damagesData)
    {
        float finalDamages = damagesData.Damages;
        if (damagesData.IsCrit) finalDamages *= GameManager.CRIT_MULTIPLIER;

        CurrentHealth -= finalDamages;

        this.OnTookDamages?.Invoke(damagesData);
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

    public void Kill()
    {
        this.OnDeath?.Invoke();
    }

    public bool TryAddTickDammages(SO_TickDamagesData data, Entity origin)
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

    public void SetInvincibilityTimer(float time)
        => InvincibilityTimer = time;

    #region EDITOR

#if UNITY_EDITOR
    [SerializeField] protected bool ED_debugMode;
#endif

    protected virtual void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!ED_debugMode) return;

        Vector2 healthBordersSize = new Vector2(0.75f, 0.5f);
        Gizmos.DrawWireCube((Vector2)this.transform.position + HealthPopupOffset, healthBordersSize);

        Color c = UnityEditor.Handles.color;
        UnityEditor.Handles.color = Color.red;

        Vector2 centeredPosition = (Vector2)this.transform.position + HealthPopupOffset;

        if (UnityEditor.SceneView.currentDrawingSceneView == null) return;

        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(centeredPosition);


        Vector2 textOffset = new Vector2(-36, 7.5f);
        Camera cam = UnityEditor.SceneView.currentDrawingSceneView.camera;
        if (cam)
            centeredPosition = cam.ScreenToWorldPoint((Vector2)cam.WorldToScreenPoint(centeredPosition) + textOffset);


        UnityEditor.Handles.Label(centeredPosition, "Health Popup");

        UnityEditor.Handles.color = c;
#endif
    } 

    #endregion
}

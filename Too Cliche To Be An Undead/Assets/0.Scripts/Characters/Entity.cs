using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static IComponentHolder;

public class Entity : MonoBehaviourEventsHandler, IComponentHolder, ITargetable
{
    [SerializeField] private SerializedDictionary<E_Component, Component> entityComponents;
    public SerializedDictionary<E_Component, Component> EntityComponents
    {
        get => entityComponents;
        protected set => entityComponents = value;
    }

    public event Action<ComponentChangeEventArgs> OnComponentModified;

    public event Action OnFullReset;
    public void ResetEntity()
        => OnFullReset?.Invoke();

    public event Action<float, bool, bool> OnAskForStun;
    protected void AskForStun(float duration, bool resetAttackTimer, bool showStunText)
        => OnAskForStun?.Invoke(duration, resetAttackTimer, showStunText);

    public event Action<Collision2D> OnEnteredCollider;
    public event Action<Collision2D> OnExitedCollider;

    public event Action<Collider2D> OnEnteredTrigger;
    public event Action<Collider2D> OnExitedTrigger;

    [field: SerializeField] public List<Entity> Attackers { get; protected set; }

#if UNITY_EDITOR
    [SerializeField] protected bool debugMode;
#endif

    public E_Result HolderTryGetComponent<ExpectedType>(E_Component component, out ExpectedType resultComponent)
                              where ExpectedType : Component
    {
        resultComponent = null;
        if (!EntityComponents.TryGetValue(component, out Component brutResult))
            return E_Result.ComponentNotFound;

        if (brutResult.GetType() != typeof(ExpectedType))
            return E_Result.TypeUnmatch;

        resultComponent = brutResult as ExpectedType;
        return E_Result.Success;
    }

    public ExpectedType HolderGetComponent<ExpectedType>(E_Component component) where ExpectedType : Component
        => EntityComponents[component] as ExpectedType;

    public void HolderChangeComponent<ExpectedType>(E_Component componentType,  ExpectedType component)
                          where ExpectedType : Component
    {
        if (!EntityComponents.ContainsKey(componentType))
            EntityComponents.Add(componentType, component);
        else
            EntityComponents[componentType] = component;

        OnComponentModified?.Invoke(new ComponentChangeEventArgs(componentType, component));
    }

    protected override void EventsSubscriber()
    {
    }

    protected override void EventsUnSubscriber()
    {
    }

    public virtual void Stun(float duration, bool resetAttackTimer = false, bool showStuntext = false) {  }

    public void StartAttackTimer(float durationModifier = 0, bool addRandom = false)
    {
        //float finalDuration = UnityEngine.Random.Range(MaxAttCD_M, MaxAttCD_M * 2);
        //finalDuration += durationModifier;

        //attack_TIMER = finalDuration;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnEnteredCollider?.Invoke(collision);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        OnExitedCollider?.Invoke(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnEnteredTrigger?.Invoke(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        OnExitedTrigger?.Invoke(collision);
    }

    public void AddAttacker(Entity entity)
        => Attackers.Add(entity);

    public void RemoveAttacker(Entity entity)
        => Attackers.Remove(entity);

    public int GetAttackersCount()
        => Attackers.Count;
    public Vector2 GetPosition()
        => this.transform.position;
}

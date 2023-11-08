using System;
using UnityEngine;

public interface IComponentHolder
{
    public ExpectedType HolderGetComponent<ExpectedType>(E_Component component) 
                                     where ExpectedType : Component;

    public abstract E_Result HolderTryGetComponent<ExpectedType>(E_Component component, out ExpectedType result) 
                                             where ExpectedType : Component;

    public void HolderChangeComponent<ExpectedType>(E_Component componentType, ExpectedType component)
                                where ExpectedType : Component;

    public event Action<ComponentChangeEventArgs> OnComponentModified;

    public enum E_Component
    {
        StatsHandler,
        HealthSystem,
        RigidBody2D,
        Motor,
        AnimationController,
        PlayerInputsComponent,
        AudioPlayer,
        EnemyAI,
        FSM,
        Aimer,
        WeaponHandler,
    }

    public enum E_Result
    {
        Success,
        ComponentNotFound,
        TypeUnmatch
    }
}

public class ComponentChangeEventArgs : EventArgs
{
    public IComponentHolder.E_Component ComponentType { get; private set; }
    public Component NewComponent { get; private set; }

    public ComponentChangeEventArgs(IComponentHolder.E_Component componentType, Component newComponent)
    {
        ComponentType = componentType;
        NewComponent = newComponent;
    }
}
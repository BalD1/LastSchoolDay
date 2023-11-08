using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class NewFSM_Base<StateEnum> : MonoBehaviourEventsHandler
                                        where StateEnum : Enum
{
    [SerializeField] protected GameObject ownerObj;
    public IComponentHolder Owner { get; private set; }

    [field: SerializeField] public IState CurrentState { get; private set; }
    [field: SerializeField, ReadOnly] public StateEnum CurrentStateKey { get; private set; }
    [field: SerializeField] public StateEnum BaseStateKey { get; private set; }
    [field: SerializeField] public Dictionary<StateEnum, IState> States { get; private set; } = new Dictionary<StateEnum, IState>();

    public event Action<StateEnum> OnStateChange;

    protected override void Awake()
    {
        Owner = ownerObj.GetComponent<IComponentHolder>();
        base.Awake();
    }

    protected virtual void Start()
    {
        SetupStates();
        if (!States.TryGetValue(BaseStateKey, out IState baseState) || baseState == null)
        {
            this.Log("Could not find base state " +  BaseStateKey, CustomLogger.E_LogType.Error);
            return;
        }
        SetupComponents();
        CurrentState = baseState;
        CurrentStateKey = BaseStateKey;
        CurrentState.EnterState();
    }

    protected abstract void SetupComponents();
    protected abstract void SetupStates();

    protected virtual void Update()
    {
        CurrentState?.Update();
        CurrentState?.Conditions();
    }

    protected virtual void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }

    private void PerformSwitchState(StateEnum state)
    {
        CurrentState?.ExitState();
        if (!States.TryGetValue(state, out IState newState) || newState == null)
        {
            this.Log("Could not find state " + state, CustomLogger.E_LogType.Error);
            return;
        }
        CurrentState = newState;
        CurrentStateKey = state;
        CurrentState?.EnterState();
        OnStateChange?.Invoke(state);
    }

    public void AskSwitchState(StateEnum state)
    {
        PerformSwitchState(state);
    }

    protected override void OnDestroy()
    {
        CurrentState?.ExitState();
        base.OnDestroy();
    }

}

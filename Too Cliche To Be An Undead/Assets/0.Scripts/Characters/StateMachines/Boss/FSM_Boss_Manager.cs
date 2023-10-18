using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Boss_Manager : FSM_ManagerBase
{
    [SerializeField] private BossZombie owner;
    public BossZombie Owner { get => owner; }

    public FSM_Boss_Attacking AttackingState { get; private set; } = new FSM_Boss_Attacking();
    public FSM_Boss_Chasing ChasingState { get; private set; } = new FSM_Boss_Chasing();
    public FSM_Boss_Stun StunnedState { get; private set; } = new FSM_Boss_Stun();
    public FSM_Boss_Recovering RecoveringState { get; private set; } = new FSM_Boss_Recovering();
    public FSM_Boss_Dead DeadState { get; private set; } = new FSM_Boss_Dead();
    public FSM_Boss_AppearCinematic AppearCinematic { get; private set; } = new FSM_Boss_AppearCinematic();

    private FSM_Base<FSM_Boss_Manager, E_BossState> currentState;
    public FSM_Base<FSM_Boss_Manager, E_BossState> CurrentState { get => currentState; }

    private Dictionary<E_BossState, FSM_Base<FSM_Boss_Manager, E_BossState>> statesWithKey;

    public enum E_BossState
    {
        Attacking,
        Chasing,
        Stunned,
        Recovering,
        Dead,
        Appear,
    }

    protected override void Start()
    {
    }

    protected override void Update()
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;
        currentState.UpdateState(this);
        currentState.Conditions(this);
    }

    protected override void FixedUpdate()
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;

        currentState.FixedUpdateState(this);
    }

    public void SwitchState(FSM_Base<FSM_Boss_Manager, E_BossState> newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
        owner.CallStateChange(newState.GetKey());

#if UNITY_EDITOR
        owner.currentStateDebug = this.ToString();
#endif
    }

    public T SwitchState<T>(E_BossState stateKey) where T : FSM_Base<FSM_Boss_Manager, E_BossState>
    {
        SwitchState(stateKey);
        return currentState as T;
    }
    public void SwitchState(E_BossState stateKey)
    {
        statesWithKey.TryGetValue(stateKey, out FSM_Base<FSM_Boss_Manager, E_BossState> newState);
        SwitchState(newState);
    }

    public void Movements(Vector2 goalPosition, bool slowdownOnApproach = true)
    {
        owner.Movements(goalPosition, slowdownOnApproach);
    }

    public bool AttackConditions()
    {
        float distanceFromTarget = Vector2.Distance(owner.transform.position, owner.CurrentPlayerTarget.transform.position);

        bool isAtRightDistance = (distanceFromTarget <= owner.DistanceBeforeStop || distanceFromTarget <= owner.CurrentAttack.attack.AttackDistance);

        bool targetCanBeAttacked = owner.CurrentPlayerTarget?.Attackers.Count < GameManager.MaxAttackers;

        return (isAtRightDistance && owner.Attack_TIMER <= 0 && targetCanBeAttacked);
    }

    public override void SetupStates()
    {
        statesWithKey = new Dictionary<E_BossState, FSM_Base<FSM_Boss_Manager, E_BossState>>()
        {
            {E_BossState.Attacking, AttackingState },
            {E_BossState.Chasing, ChasingState },
            {E_BossState.Stunned, StunnedState },
            {E_BossState.Recovering, RecoveringState },
            {E_BossState.Dead, DeadState },
            {E_BossState.Appear, AppearCinematic }
        };
        foreach (var item in statesWithKey)
        {
            item.Value.Setup(this);
        }
    }

    public override string ToString()
    {
        if (currentState == null) return "N/A";

        return currentState.ToString();
    }
}

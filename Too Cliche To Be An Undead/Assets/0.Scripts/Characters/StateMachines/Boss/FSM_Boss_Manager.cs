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

    private FSM_Base<FSM_Boss_Manager> currentState;
    public FSM_Base<FSM_Boss_Manager> CurrentState { get => currentState; }

    private Dictionary<E_BossState, FSM_Base<FSM_Boss_Manager>> statesWithKey;

    public enum E_BossState
    {
        Attacking,
        Chasing,
        Stunned,
        Recovering,
        Dead,
    }

    protected override void Start()
    {
        if (owner.isAppeared) OnStart();
    }

    public void OnStart()
    {
        currentState = ChasingState;
        currentState.EnterState(this);

#if UNITY_EDITOR
        owner.currentStateDebug = this.ToString();
#endif
    }

    protected override void Update()
    {
        if (!owner.isAppeared) return;
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;
        if (!owner.IsAlive()) return;
        currentState.UpdateState(this);
        currentState.Conditions(this);
    }

    protected override void FixedUpdate()
    {
        if (!owner.isAppeared) return;
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;
        if (!owner.IsAlive()) return;

        currentState.FixedUpdateState(this);
    }

    public void SwitchState(FSM_Base<FSM_Boss_Manager> newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);

#if UNITY_EDITOR
        owner.currentStateDebug = this.ToString();
#endif
    }

    public T SwitchState<T>(E_BossState stateKey) where T : FSM_Base<FSM_Boss_Manager>
    {
        SwitchState(stateKey);
        return currentState as T;
    }
    public void SwitchState(E_BossState stateKey)
    {
        statesWithKey.TryGetValue(stateKey, out FSM_Base<FSM_Boss_Manager> newState);
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
        statesWithKey = new Dictionary<E_BossState, FSM_Base<FSM_Boss_Manager>>()
        {
            {E_BossState.Attacking, AttackingState },
            {E_BossState.Chasing, ChasingState },
            {E_BossState.Stunned, StunnedState },
            {E_BossState.Recovering, RecoveringState },
            {E_BossState.Dead, DeadState },
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

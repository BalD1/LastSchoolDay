using System.Collections.Generic;
using UnityEngine;

public class FSM_NZ_Manager : FSM_ManagerBase
{
    [SerializeField] private BaseZombie owner;
    public BaseZombie Owner { get => owner; }

    public FSM_NZ_Idle IdleState { get; private set; } = new FSM_NZ_Idle();
    public FSM_NZ_Wandering WanderingState { get; private set; } = new FSM_NZ_Wandering();
    public FSM_NZ_Chasing ChasingState { get; private set; } = new FSM_NZ_Chasing();
    public FSM_NZ_Pushed PushedState { get; private set; } = new FSM_NZ_Pushed();
    public FSM_NZ_Stun StunnedState { get; private set; } = new FSM_NZ_Stun();
    public FSM_NZ_Attacking AttackingState { get; private set; } = new FSM_NZ_Attacking();

    private FSM_Base<FSM_NZ_Manager, E_NZState> currentState;
    public FSM_Base<FSM_NZ_Manager, E_NZState> CurrentState { get => currentState; }

    private Dictionary<E_NZState, FSM_Base<FSM_NZ_Manager, E_NZState>> statesWithKey;

    public enum E_NZState
    {
        Idle,
        Wandering,
        Chasing,
        Pushed,
        Stunned,
        Attacking,
    }

    protected override void Start()
    {
        currentState = WanderingState;
        currentState.EnterState(this);

#if UNITY_EDITOR
        //owner.currentStateDebug = this.ToString();
#endif
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

    public void SwitchState(FSM_Base<FSM_NZ_Manager, E_NZState> newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
        //owner.CallStateChange(newState.GetKey());

#if UNITY_EDITOR
        //owner.currentStateDebug = this.ToString(); 
#endif
    }

    public T SwitchState<T>(E_NZState stateKey) where T : FSM_Base<FSM_NZ_Manager, E_NZState>
    {
        SwitchState(stateKey);
        return currentState as T;
    }
    public void SwitchState(E_NZState stateKey)
    {
        statesWithKey.TryGetValue(stateKey, out FSM_Base<FSM_NZ_Manager, E_NZState> newState);
        SwitchState(newState);
    }

    public void Movements(Vector2 goalPosition, bool slowdownOnApproach = true)
    {
        //owner.Movements(goalPosition, slowdownOnApproach);
    }

    public void CheckStates()
    {
        if (statesWithKey == null) SetupStates();
    }
    public override void SetupStates()
    {
        statesWithKey = new Dictionary<E_NZState, FSM_Base<FSM_NZ_Manager, E_NZState>>()
        {
            { E_NZState.Idle, IdleState },
            { E_NZState.Wandering, WanderingState },
            { E_NZState.Chasing, ChasingState },
            { E_NZState.Pushed, PushedState },
            { E_NZState.Stunned, StunnedState },
            { E_NZState.Attacking, AttackingState },
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

    public bool AttackConditions()
    {
        return false;
        //if (owner.CurrentPlayerTarget.CurrentActiveTimestops > 0) return false;
        //float distanceFromTarget = Vector2.Distance(owner.transform.position, owner.CurrentPlayerTarget.transform.position);

        //bool isAtRightDistance = (distanceFromTarget <= owner.DistanceBeforeStop || distanceFromTarget <= owner.Attack.AttackDistance);

        //bool targetCanBeAttacked = owner.CurrentPlayerTarget?.Attackers.Count < GameManager.MaxAttackers;

        //return (isAtRightDistance && owner.Attack_TIMER <= 0 && targetCanBeAttacked && owner.isVisible);
    }

    public void SwitchToStun(float _duration, bool _resetAttackTimer, bool _showText)
    {
        if (_showText) TextPopup.Create("Stunned", owner.transform);
        SwitchState<FSM_NZ_Stun>(E_NZState.Stunned).SetDuration(_duration, _resetAttackTimer);
    }

    public void SwitchToPushed(float _force, Entity _pusher, Entity _originalPusher)
    {
        Vector2 vectorForce = this.GetPushForce(owner, _force, _pusher, _originalPusher);

        float sqrMagnitude = vectorForce.sqrMagnitude;
        if (_pusher is EnemyBase && sqrMagnitude <= (7 * 7)) return;
        if (sqrMagnitude <= 0) return;
        PushedState.SetForce(vectorForce, _pusher, _pusher);
        SwitchState<FSM_NZ_Pushed>(E_NZState.Pushed);
    }
}

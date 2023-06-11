using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FSM_NZ_Manager : FSM_ManagerBase
{
    [SerializeField] private NormalZombie owner;
    public NormalZombie Owner { get => owner; }

    public FSM_NZ_Idle idleState = new FSM_NZ_Idle();
    public FSM_NZ_Wandering wanderingState = new FSM_NZ_Wandering();
    public FSM_NZ_Chasing chasingState = new FSM_NZ_Chasing();
    public FSM_NZ_Pushed pushedState = new FSM_NZ_Pushed();
    public FSM_NZ_Stun stunnedState = new FSM_NZ_Stun();
    public FSM_NZ_Attacking attackingState = new FSM_NZ_Attacking();

    private FSM_Base<FSM_NZ_Manager> currentState;
    public FSM_Base<FSM_NZ_Manager> CurrentState { get => currentState; }

    private void Awake()
    {
        pushedState.SetOwner(Owner);
    }

    protected override void Start()
    {
        currentState = wanderingState;
        currentState.EnterState(this);

#if UNITY_EDITOR
        owner.currentStateDebug = this.ToString();
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

    public void SwitchState(FSM_Base<FSM_NZ_Manager> newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);

#if UNITY_EDITOR
        owner.currentStateDebug = this.ToString(); 
#endif
    }

    public void Movements(Vector2 goalPosition, bool slowdownOnApproach = true)
    {
        owner.Movements(goalPosition, slowdownOnApproach);
    }

    public override string ToString()
    {
        if (currentState == null) return "N/A";

        return currentState.ToString();
    }

    public bool AttackConditions()
    {
        if (this.currentState.ToString() == "Stunned") return false;
        if (owner.CurrentPlayerTarget.CurrentActiveTimestops > 0) return false;
        float distanceFromTarget = Vector2.Distance(owner.transform.position, owner.CurrentPlayerTarget.transform.position);

        bool isAtRightDistance = (distanceFromTarget <= owner.DistanceBeforeStop || distanceFromTarget <= owner.Attack.AttackDistance);

        bool targetCanBeAttacked = owner.CurrentPlayerTarget?.Attackers.Count < GameManager.MaxAttackers;

        return (isAtRightDistance && owner.Attack_TIMER <= 0 && targetCanBeAttacked && owner.isVisible);
    }
}

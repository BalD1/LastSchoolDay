using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Boss_Manager : FSM_ManagerBase
{
    [SerializeField] private BossZombie owner;
    public BossZombie Owner { get => owner; }

    public FSM_Boss_Attacking attackingState = new FSM_Boss_Attacking();
    public FSM_Boss_Chasing chasingState = new FSM_Boss_Chasing();
    public FSM_Boss_Stun stunnedState = new FSM_Boss_Stun();
    public FSM_Boss_Recovering recoveringState = new FSM_Boss_Recovering();

    private FSM_Base<FSM_Boss_Manager> currentState;
    public FSM_Base<FSM_Boss_Manager> CurrentState { get => currentState; }

    private void Awake()
    {
    }

    protected override void Start()
    {
        currentState = chasingState;
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

    public void SwitchState(FSM_Base<FSM_Boss_Manager> newState)
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

    public bool AttackConditions()
    {
        float distanceFromTarget = Vector2.Distance(owner.transform.position, owner.CurrentPlayerTarget.transform.position);

        bool isAtRightDistance = (distanceFromTarget <= owner.DistanceBeforeStop || distanceFromTarget <= owner.CurrentAttack.attack.AttackDistance);

        bool targetCanBeAttacked = owner.CurrentPlayerTarget?.Attackers.Count < GameManager.MaxAttackers;

        return (isAtRightDistance && owner.Attack_TIMER <= 0 && targetCanBeAttacked);
    }

    public override string ToString()
    {
        if (currentState == null) return "N/A";

        return currentState.ToString();
    }
}

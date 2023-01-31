using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Boss_Manager : FSM_ManagerBase
{
    [SerializeField] private BossZombie owner;
    public BossZombie Owner { get => owner; }

    public FSM_Boss_Attacking attacking = new FSM_Boss_Attacking();

    private FSM_Base<FSM_Boss_Manager> currentState;
    public FSM_Base<FSM_Boss_Manager> CurrentState { get => currentState; }

    private void Awake()
    {
    }

    protected override void Start()
    {
        currentState = attacking;
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

    public override string ToString()
    {
        if (currentState == null) return "N/A";

        return currentState.ToString();
    }
}

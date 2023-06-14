using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Manager : FSM_ManagerBase
{
    [SerializeField] private PlayerCharacter owner;
    public PlayerCharacter Owner { get => owner; }
    [SerializeField] private PlayerWeapon ownerWeapon;
    public PlayerWeapon OwnerWeapon { get => ownerWeapon; set => ownerWeapon = value; }

    public FSM_Player_Idle IdleState { get; private set; } = new FSM_Player_Idle();
    public FSM_Player_Moving MovingState { get; private set; } = new FSM_Player_Moving();
    public FSM_Player_Attacking AttackingState { get; private set; } = new FSM_Player_Attacking();
    public FSM_Player_Dashing DashingState { get; private set; } = new FSM_Player_Dashing();
    public FSM_Player_Pushed PushedState { get; private set; } = new FSM_Player_Pushed();
    public FSM_Player_InSkill InSkillState { get; private set; } = new FSM_Player_InSkill();
    public FSM_Player_Dying DyingState { get; private set; } = new FSM_Player_Dying();
    public FSM_Player_Dead DeadState { get; private set; } = new FSM_Player_Dead();
    public FSM_Player_Stuned StunnedState { get; private set; } = new FSM_Player_Stuned();
    public FSM_Player_Cinematic CinematicState { get; private set; } = new FSM_Player_Cinematic();

    private Dictionary<E_PlayerState, FSM_Base<FSM_Player_Manager>> statesWithKey;

    public enum E_PlayerState
    {
        Idle,
        Moving,
        Attacking,
        Dashing,
        Pushed,
        InSkill,
        Dying,
        Dead,
        Stunned,
        Cinematic
    }

    private FSM_Base<FSM_Player_Manager> currentState;
    public FSM_Base<FSM_Player_Manager> CurrentState { get => currentState; }

    public delegate void D_StateChange(string newState);
    public D_StateChange D_stateChange;

    public bool allowChanges = true;

    protected override void Start()
    {
        base.Start();

        AttackingState.owner = this.owner;
        currentState = IdleState;
        currentState.EnterState(this);
        owner.AnimationController.SetCharacterState(this.ToString());
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

    private void SwitchState(FSM_Base<FSM_Player_Manager> state)
    {
        currentState?.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }

    public void SwitchState(E_PlayerState newStateKey)
    {
        statesWithKey.TryGetValue(newStateKey, out FSM_Base<FSM_Player_Manager> newState);
        SwitchState(newState);
    }

    public T SwitchState<T>(T newState) where T : FSM_Base<FSM_Player_Manager>
    {
        D_stateChange?.Invoke(newState.ToString());

        SwitchState(newState as FSM_Base<FSM_Player_Manager>);

        owner.AnimationController.SetCharacterState(this.ToString());

        return currentState as T;
    }

    public T SwitchState<T>(E_PlayerState newStateKey) where T : FSM_Base<FSM_Player_Manager>
    {
        D_stateChange?.Invoke(newStateKey.ToString());

        SwitchState(newStateKey);

        owner.AnimationController.SetCharacterState(this.ToString());
        return currentState as T;
    }

    public override void SetupStates()
    {
        statesWithKey = new Dictionary<E_PlayerState, FSM_Base<FSM_Player_Manager>>()
        {
            {E_PlayerState.Idle, IdleState },
            {E_PlayerState.Moving, MovingState },
            {E_PlayerState.Attacking, AttackingState },
            {E_PlayerState.Dashing, DashingState },
            {E_PlayerState.Pushed, PushedState },
            {E_PlayerState.InSkill, InSkillState },
            {E_PlayerState.Dying, DyingState },
            {E_PlayerState.Dead, DeadState },
            {E_PlayerState.Stunned, StunnedState },
            {E_PlayerState.Cinematic, CinematicState },
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

    public void ResetAll()
    {

    }
}
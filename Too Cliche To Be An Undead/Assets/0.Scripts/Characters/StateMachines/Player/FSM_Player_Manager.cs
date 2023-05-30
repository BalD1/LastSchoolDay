using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Manager : FSM_ManagerBase
{
    [SerializeField] private PlayerCharacter owner;
    public PlayerCharacter Owner { get => owner; }
    [SerializeField] private PlayerWeapon ownerWeapon;
    public PlayerWeapon OwnerWeapon { get => ownerWeapon; set => ownerWeapon = value; }

    public FSM_Player_Idle idleState = new FSM_Player_Idle();
    public FSM_Player_Moving movingState = new FSM_Player_Moving();
    public FSM_Player_Attacking attackingState = new FSM_Player_Attacking();
    public FSM_Player_Dashing dashingState = new FSM_Player_Dashing();
    public FSM_Player_Pushed pushedState = new FSM_Player_Pushed();
    public FSM_Player_InSkill inSkillState = new FSM_Player_InSkill();
    public FSM_Player_Dying dyingState = new FSM_Player_Dying();
    public FSM_Player_Dead deadState = new FSM_Player_Dead();
    public FSM_Player_Stuned stunnedState = new FSM_Player_Stuned();
    public FSM_Player_Cinematic cinematicState = new FSM_Player_Cinematic();

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

    private void Awake()
    {
        pushedState.SetOwner(Owner);
    }

    protected override void Start()
    {
        attackingState.owner = this.owner;
        owner.Weapon.D_nextAttack += attackingState.NextAttack;
        currentState = idleState;
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

    public T SwitchState<T>(T newState, bool forceSwitch = false) where T : FSM_Base<FSM_Player_Manager>
    {
        if (!allowChanges && !forceSwitch) return default(T);
        if (currentState.ToString() == "Dead" && !forceSwitch) return default(T);

        D_stateChange?.Invoke(newState.ToString());

        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
        owner.AnimationController.SetCharacterState(this.ToString());

        return currentState as T;
    }

    public T SwitchState<T>(E_PlayerState newState, bool forceSwitch = false) where T : FSM_Base<FSM_Player_Manager>
    {
        if (!allowChanges && !forceSwitch) return default(T);
        if (currentState.ToString() == "Dead" && !forceSwitch) return default(T);

        D_stateChange?.Invoke(newState.ToString());

        currentState?.ExitState(this);

        switch (newState)
        {
            case E_PlayerState.Idle:
                currentState = idleState;
                break;

            case E_PlayerState.Moving:
                currentState = movingState;
                break;

            case E_PlayerState.Attacking:
                currentState = attackingState;
                break;

            case E_PlayerState.Dashing:
                currentState = dashingState;
                break;

            case E_PlayerState.Pushed:
                currentState = pushedState;
                break;

            case E_PlayerState.InSkill:
                currentState = inSkillState;
                break;

            case E_PlayerState.Dying:
                currentState = dyingState;
                break;

            case E_PlayerState.Dead:
                currentState = deadState;
                break;

            case E_PlayerState.Stunned:
                currentState = stunnedState;
                break;

            case E_PlayerState.Cinematic:
                currentState = cinematicState;
                break;
        }

        currentState.EnterState(this);
        owner.AnimationController.SetCharacterState(this.ToString());
        return currentState as T;
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

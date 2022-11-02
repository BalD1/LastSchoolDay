using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Manager : FSM_ManagerBase
{
    [SerializeField] private PlayerCharacter owner;
    public PlayerCharacter Owner { get => owner; }
    [SerializeField] private PlayerWeapon ownerWeapon;
    public PlayerWeapon OwnerWeapon { get => ownerWeapon; }

    public FSM_Player_Idle idleState = new FSM_Player_Idle();
    public FSM_Player_Moving movingState = new FSM_Player_Moving();
    public FSM_Player_Attacking attackingState = new FSM_Player_Attacking();
    public FSM_Player_Dashing dashingState = new FSM_Player_Dashing();
    public FSM_Player_Pushed pushedState = new FSM_Player_Pushed();
    public FSM_Player_InSkill inSkillState = new FSM_Player_InSkill();
    public FSM_Player_Dying dyingState = new FSM_Player_Dying();

    private FSM_Base<FSM_Player_Manager> currentState;
    public FSM_Base<FSM_Player_Manager> CurrentState { get => currentState; }

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

    public void SwitchState(FSM_Base<FSM_Player_Manager> newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    public override string ToString()
    {
        if (currentState == null) return "N/A";

        return currentState.ToString();
    }
}

using System.Collections.Generic;
using UnityEngine;
using static FSM_NZ_Manager;

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
    public FSM_Player_Stunned StunnedState { get; private set; } = new FSM_Player_Stunned();
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

    public bool allowChanges = true;

    protected override void Start()
    {
        base.Start();
        SwitchState(E_PlayerState.Idle);
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

    #region Switch
    private void SwitchState(FSM_Base<FSM_Player_Manager> state)
    {
        currentState?.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
        owner.OnStateChange?.Invoke(state.ToString());
    }

    public void SwitchState(E_PlayerState newStateKey)
    {
        statesWithKey.TryGetValue(newStateKey, out FSM_Base<FSM_Player_Manager> newState);
        SwitchState(newState);
    }

    public T SwitchState<T>(T newState) where T : FSM_Base<FSM_Player_Manager>
    {
        SwitchState(newState as FSM_Base<FSM_Player_Manager>);
        return currentState as T;
    }

    public T SwitchState<T>(E_PlayerState newStateKey) where T : FSM_Base<FSM_Player_Manager>
    {
        SwitchState(newStateKey);
        return currentState as T;
    } 
    
    public void ForceSetState(E_PlayerState state)
    {
        statesWithKey.TryGetValue(state, out FSM_Base<FSM_Player_Manager> newState);
        SwitchState(newState);
    }
    public T ForceSetState<T>(E_PlayerState state) where T : FSM_Base<FSM_Player_Manager>
    {
        return SwitchState<T>(state) as T;
    }
    #endregion

    #region Conditions

    public void DashConditions()
    {
        if (owner.DashCooldown > 0) return;
        this.SwitchState(E_PlayerState.Dashing);
    }

    public void PushConditions(float _force, Entity _pusher, Entity _originalPusher)
    {
        Vector2 vectorForce = this.GetPushForce(owner, _force, _pusher, _originalPusher);
        if (vectorForce.magnitude > 0)
            SwitchState<FSM_Player_Pushed>(E_PlayerState.Pushed).SetForce(vectorForce, _originalPusher, _pusher);
    }

    public void SwitchToStun(float duration, bool resetAttackTimer = false, bool showStuntext = false)
    {
        if (showStuntext)
            TextPopup.Create("Stun !", owner.GetHealthPopupOffset + (Vector2)this.transform.position);
        this.SwitchState<FSM_Player_Stunned>(E_PlayerState.Stunned).SetDuration(duration, resetAttackTimer);
    }

    public void SwitchToSkill(float duration, float transition, float offset)
    {
        if (owner.GetSkillHolder == null) return;
        if (owner.GetSkill == null) return;

        if (duration <= 0) return;
        if (owner.GetSkillHolder.SkillCooldown > 0) return;
        if (owner.GetSkill.IsInUse) return;

        SwitchState<FSM_Player_InSkill>(E_PlayerState.InSkill).SetTimers(duration, transition, offset);
    } 

    #endregion

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
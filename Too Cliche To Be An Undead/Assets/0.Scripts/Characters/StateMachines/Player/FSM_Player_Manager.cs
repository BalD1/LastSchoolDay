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
    public FSM_Player_Stunned StunnedState { get; private set; } = new FSM_Player_Stunned();
    public FSM_Player_Cinematic CinematicState { get; private set; } = new FSM_Player_Cinematic();
    public E_PlayerState CurrentStateName { get; private set; } = E_PlayerState.Idle;

    private Dictionary<E_PlayerState, FSM_Base<FSM_Player_Manager, E_PlayerState>> statesWithKey;

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

    private FSM_Base<FSM_Player_Manager, E_PlayerState> currentState;
    public FSM_Base<FSM_Player_Manager, E_PlayerState> CurrentState { get => currentState; }

    public bool allowChanges = true;

    protected override void Start()
    {
        if (currentState == null) SwitchState(E_PlayerState.Idle);
    }

    protected override void Update()
    {
        if (!GameManager.IsInGame) return;
        currentState.UpdateState(this);
        currentState.Conditions(this);
    }

    protected override void FixedUpdate()
    {
        if (!GameManager.IsInGame) return;
        currentState.FixedUpdateState(this);
    }

    #region Switch
    private void SwitchState(FSM_Base<FSM_Player_Manager, E_PlayerState> state)
    {
        currentState?.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
        owner.CallStateChange(state.StateKey);
    }

    public void SwitchState(E_PlayerState newStateKey)
    {
        statesWithKey.TryGetValue(newStateKey, out FSM_Base<FSM_Player_Manager, E_PlayerState> newState);
        SwitchState(newState);
    }

    public T SwitchState<T>(T newState) where T : FSM_Base<FSM_Player_Manager, E_PlayerState>
    {
        SwitchState(newState as FSM_Base<FSM_Player_Manager, E_PlayerState>);
        return currentState as T;
    }

    public T SwitchState<T>(E_PlayerState newStateKey) where T : FSM_Base<FSM_Player_Manager, E_PlayerState>
    {
        SwitchState(newStateKey);
        return currentState as T;
    } 
    
    public void ForceSetState(E_PlayerState state)
    {
        statesWithKey.TryGetValue(state, out FSM_Base<FSM_Player_Manager, E_PlayerState> newState);
        SwitchState(newState);
    }
    public T ForceSetState<T>(E_PlayerState state) where T : FSM_Base<FSM_Player_Manager, E_PlayerState>
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
        if (vectorForce.sqrMagnitude > 0)
        {
            this.PushedState.SetForce(vectorForce, _pusher, _originalPusher);
            SwitchState<FSM_Player_Pushed>(E_PlayerState.Pushed);
        }
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

    public void CinematicStateChange(bool newState)
    {
        SwitchState(newState ? E_PlayerState.Cinematic : E_PlayerState.Idle);
    }
    public void DialogueStart(bool fromCinematic)
    {
        if (fromCinematic) return;
        this.SwitchState(E_PlayerState.Cinematic);
    }
    public void DialogueEnded(bool fromCinematic)
    {
        if (fromCinematic) return;
        this.SwitchState(E_PlayerState.Idle);
    }

    #endregion

    public override void SetupStates()
    {
        statesWithKey = new Dictionary<E_PlayerState, FSM_Base<FSM_Player_Manager, E_PlayerState>>()
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
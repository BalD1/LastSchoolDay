using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Player_Idle : State_Player_Base
{
    public State_Player_Idle(FSM_PlayerCharacter fsm) : base(fsm)
    {
    }

    public override void EventsSubscriber()
    {
        ownerFSM.OwnerInputs.OnMovementsInput += ReadMovementsInput;
        ownerFSM.OwnerInputs.OnAttackInput += OnAttackInput;
    }

    public override void EventsUnSubscriber()
    {
        ownerFSM.OwnerInputs.OnMovementsInput -= ReadMovementsInput;
        ownerFSM.OwnerInputs.OnAttackInput -= OnAttackInput;
    }

    public override void EnterState()
    {
        base.EnterState();
        ownerFSM.OwnerInputs.ForceReadMovements();
    }

    public override void Update()
    {
        if (ownerFSM.OwnerInputs.IsOnKeyboard())
        {
            bool lookAtRight = GameManager.Instance.MouseWorldPos.x > ownerFSM.PlayerAnimationController.transform.position.x;
            ownerFSM.PlayerAnimationController.TryFlipSkeleton(lookAtRight);
        }
    }

    public override void FixedUpdate()
    {
    }

    public override void Conditions()
    {
        if (!ownerFSM.OwnerMotor.IsIdle())
            ownerFSM.AskSwitchState(FSM_PlayerCharacter.E_PlayerStates.Moving);
    }

    private void ReadMovementsInput(Vector2 value)
    {
        ownerFSM.OwnerMotor.ReadMovementsInputs(value);
        ownerFSM.AskSwitchState(FSM_PlayerCharacter.E_PlayerStates.Moving);
    }

    private void OnAttackInput()
    {
        if (ownerFSM.OwnerWeaponHandler.CurrentWeapon.AskAttack())
            ownerFSM.AskSwitchState(FSM_PlayerCharacter.E_PlayerStates.Attacking);
    }
}

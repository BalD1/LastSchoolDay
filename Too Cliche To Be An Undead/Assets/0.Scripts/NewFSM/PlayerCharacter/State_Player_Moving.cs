using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Player_Moving : State_Player_Base
{
    private PlayerInputHandler ownerInputs;
    private PlayerMotor ownerMotor;
    private WeaponHandler ownerWeaponHandler;

    public State_Player_Moving(FSM_PlayerCharacter fsm) : base(fsm)
    {
        ownerFSM.Owner.HolderTryGetComponent(IComponentHolder.E_Component.PlayerInputsComponent, out ownerInputs);
        ownerFSM.Owner.HolderTryGetComponent(IComponentHolder.E_Component.Motor, out ownerMotor);
        ownerFSM.Owner.HolderTryGetComponent(IComponentHolder.E_Component.WeaponHandler, out ownerWeaponHandler);
    }

    public override void EventsSubscriber()
    {
        ownerInputs.OnMovementsInput += ReadMovementsInput;
        ownerInputs.OnAttackInput += OnAttackInput;
    }

    public override void EventsUnSubscriber()
    {
        ownerInputs.OnMovementsInput -= ReadMovementsInput;
        ownerInputs.OnAttackInput -= OnAttackInput;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void Update()
    {
        ownerMotor.ReadMovementsInputsFromComponent();
    }

    public override void FixedUpdate()
    {
        ownerMotor.MoveByVelocity();
    }

    public override void Conditions()
    {
        if (ownerMotor.IsIdle())
            ownerFSM.AskSwitchState(FSM_PlayerCharacter.E_PlayerStates.Idle);
    }

    private void ReadMovementsInput(Vector2 value)
        => ownerMotor.ReadMovementsInputs(value);

    private void OnAttackInput()
    {
        if (ownerWeaponHandler.CurrentWeapon.AskAttack())
            ownerFSM.AskSwitchState(FSM_PlayerCharacter.E_PlayerStates.Attacking);
    }
}

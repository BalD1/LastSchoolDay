using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Player_Attacking : State_Player_Base
{
    private PlayerInputHandler ownerInputs;
    private WeaponHandler ownerWeaponHandler;
    private PlayerMotor ownerMotor;

    public State_Player_Attacking(FSM_PlayerCharacter fsm) : base(fsm)
    {
        ownerFSM.Owner.HolderTryGetComponent(IComponentHolder.E_Component.PlayerInputsComponent, out ownerInputs);
        ownerFSM.Owner.HolderTryGetComponent(IComponentHolder.E_Component.WeaponHandler, out ownerWeaponHandler);
        ownerFSM.Owner.HolderTryGetComponent(IComponentHolder.E_Component.Motor, out ownerMotor);
    }

    public override void EventsSubscriber()
    {
        ownerInputs.OnAttackInput += OnAttackInput;
        ownerWeaponHandler.CurrentWeapon.OnAttackEnded += OnAttackEnded;
    }

    public override void EventsUnSubscriber()
    {
        ownerInputs.OnAttackInput -= OnAttackInput;
        ownerWeaponHandler.CurrentWeapon.OnAttackEnded -= OnAttackEnded;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void Conditions()
    {
    }

    private void OnAttackEnded()
    {
        if (ownerInputs.MovementsInput == Vector2.zero)
            ownerFSM.AskSwitchState(FSM_PlayerCharacter.E_PlayerStates.Idle);
        else
            ownerFSM.AskSwitchState(FSM_PlayerCharacter.E_PlayerStates.Moving);
    }

    private void OnAttackInput()
    {
        ownerWeaponHandler.CurrentWeapon.AskAttack();
    }
}

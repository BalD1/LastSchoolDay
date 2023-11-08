using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_PlayerCharacter : NewFSM_Base<FSM_PlayerCharacter.E_PlayerStates>
{
    public PlayerInputHandler OwnerInputs { get => ownerInputs; }
    public PlayerMotor OwnerMotor { get => ownerMotor; }
    public WeaponHandler OwnerWeaponHandler { get => ownerWeaponHandler; }
    public PlayerAnimationController PlayerAnimationController { get => playerAnimationController; }
    public AimDirection PlayerAim { get => playerAim; }

    private PlayerInputHandler ownerInputs;
    private PlayerMotor ownerMotor;
    private WeaponHandler ownerWeaponHandler;
    private PlayerAnimationController playerAnimationController;
    private AimDirection playerAim;

    protected override void EventsSubscriber()
    {
    }

    protected override void EventsUnSubscriber()
    {
    }

    protected override void SetupComponents()
    {
        Owner.HolderTryGetComponent(IComponentHolder.E_Component.PlayerInputsComponent, out ownerInputs);
        Owner.HolderTryGetComponent(IComponentHolder.E_Component.Motor, out ownerMotor);
        Owner.HolderTryGetComponent(IComponentHolder.E_Component.WeaponHandler, out ownerWeaponHandler);
        Owner.HolderTryGetComponent(IComponentHolder.E_Component.AnimationController, out playerAnimationController);
        Owner.HolderTryGetComponent(IComponentHolder.E_Component.AnimationController, out playerAim);
    }

    protected override void SetupStates()
    {
        foreach (E_PlayerStates stateName in Enum.GetValues(typeof(E_PlayerStates)))
        {
            State_Player_Base state = null;

            switch (stateName)
            {
                case E_PlayerStates.Idle:
                    state = new State_Player_Idle(this);
                    break;

                case E_PlayerStates.Moving:
                    state = new State_Player_Moving(this);
                    break;

                case E_PlayerStates.Attacking:
                    state = new State_Player_Attacking(this);
                    break;

                case E_PlayerStates.Dying:
                    state = new State_Player_Dying(this);
                    break;

                case E_PlayerStates.Dead:
                    state = new State_Player_Dead(this);
                    break;

                default:
                    this.Log("Could not find state " + stateName, CustomLogger.E_LogType.Error);
                    break;
            }

            States.Add(stateName, state);
        }
    }

    public enum E_PlayerStates
    {
        Idle,
        Moving,
        Attacking,
        Dying,
        Dead,
    }
}

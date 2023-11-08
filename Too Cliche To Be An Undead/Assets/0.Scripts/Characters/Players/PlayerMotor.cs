using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotor : EntityMotor<PlayerAnimationController>
{
    private PlayerInputHandler playerInputHandler;

    protected override void SetComponents()
    {
        base.SetComponents();
        if (owner.HolderTryGetComponent(IComponentHolder.E_Component.PlayerInputsComponent, out playerInputHandler) != IComponentHolder.E_Result.Success)
        {
            owner.OnComponentModified += WaitForOwnerInputsSetup;
        }
    }

    private void WaitForOwnerInputsSetup(ComponentChangeEventArgs args)
    {
        if (args.ComponentType == IComponentHolder.E_Component.PlayerInputsComponent)
        {
            playerInputHandler = owner.HolderGetComponent<PlayerInputHandler>(IComponentHolder.E_Component.PlayerInputsComponent);
            owner.OnComponentModified -= WaitForOwnerInputsSetup;
        }
    }

    public void ReadMovementsInputsFromComponent()
    => ReadMovementsInputs(playerInputHandler.MovementsInput);
    public void ReadMovementsInputs(InputAction.CallbackContext context)
    => ReadMovementsInputs(context.ReadValue<Vector2>());
    public void ReadMovementsInputs(Vector2 value)
    {
        Velocity = value;
        if (Velocity != Vector2.zero) LastDirection = Velocity;
    }
}

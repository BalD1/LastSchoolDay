using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField] private PlayerCharacter owner;

    private Rigidbody2D body;
    [field: SerializeField, ReadOnly] public Vector2 Velocity { get; private set; }
    [field: SerializeField, ReadOnly] public Vector2 LastDirection { get; private set; }

    private bool stayStatic;

    private void Awake()
    {
        body = owner.GetRb;
    }

    public void SetSelfVelocity(Vector2 velocity) => this.Velocity = velocity;
    public void SetAllVelocity(Vector2 velocity)
    {
        this.Velocity = velocity;
        body.velocity = velocity;
    }

    public void ReadMovementsInputs(InputAction.CallbackContext context)
    => ReadMovementsInputs(context.ReadValue<Vector2>());
    public void ReadMovementsInputs(Vector2 value)
    {
        Velocity = value;
        if (Velocity != Vector2.zero) LastDirection = Velocity;
    }

    public void Movements()
    {
        if (stayStatic) return;
        Velocity = Vector2.ClampMagnitude(Velocity, owner.MaxSpeed_M);

        if (Velocity.x != 0)
            owner.AnimationController.TryFlipSkeleton(Velocity.x > 0);

        this.body.MovePosition(this.body.position + Velocity * owner.MaxSpeed_M * Time.fixedDeltaTime);
    }

    public void ForceSetLasDirection(Vector2 direction)
        => LastDirection = direction;
}

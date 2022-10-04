using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Entity
{
    private Vector2 velocity;


    protected override void Update()
    {
        base.Update();
        SetMovementsInputs();

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Movements();
    }

    private void SetMovementsInputs()
    {
        this.velocity.x = Input.GetAxis("Horizontal") * GetStats.Speed;
        this.velocity.y = Input.GetAxis("Vertical") * GetStats.Speed;
    }

    private void Movements()
    {
        velocity = Vector2.ClampMagnitude(velocity, GetStats.Speed);
        this.rb.MovePosition(this.rb.position + velocity * Time.fixedDeltaTime);
    }
}

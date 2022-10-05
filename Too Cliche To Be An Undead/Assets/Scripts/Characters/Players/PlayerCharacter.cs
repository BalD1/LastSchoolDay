using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Entity
{
    private Vector2 velocity;
    public Vector2 Velocity { get => velocity; }

    [SerializeField] private FSM_Player_Manager stateManager;
    public FSM_Player_Manager StateManager { get => stateManager; }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        SetMovementsInputs();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void SetMovementsInputs()
    {
        this.velocity.x = Input.GetAxis("Horizontal") * GetStats.Speed;
        this.velocity.y = Input.GetAxis("Vertical") * GetStats.Speed;
    }

    public void Movements()
    {
        velocity = Vector2.ClampMagnitude(velocity, GetStats.Speed);
        this.rb.MovePosition(this.rb.position + velocity * Time.fixedDeltaTime);
    }
}

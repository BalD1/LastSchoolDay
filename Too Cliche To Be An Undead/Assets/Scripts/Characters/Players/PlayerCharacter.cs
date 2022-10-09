using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : Entity
{
    private Vector2 velocity;
    public Vector2 Velocity { get => velocity; }

    [SerializeField] private FSM_Player_Manager stateManager;
    public FSM_Player_Manager StateManager { get => stateManager; }

    [SerializeField] private PlayerInput input;
    private PlayerControls playerControls;

    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();
        playerControls.InGame.Enable();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void SetInGameControlsState(bool state)
    {
        if (state) playerControls.InGame.Enable();
        else playerControls.InGame.Disable();
    }

    public void ReadMovementsInputs()
    {
        this.velocity.x = playerControls.InGame.Movements.ReadValue<Vector2>().x;
        this.velocity.y = playerControls.InGame.Movements.ReadValue<Vector2>().y;
    }

    public void Movements()
    {
        velocity = Vector2.ClampMagnitude(velocity, GetStats.Speed);
        this.rb.MovePosition(this.rb.position + velocity * GetStats.Speed * Time.fixedDeltaTime);
    }
}

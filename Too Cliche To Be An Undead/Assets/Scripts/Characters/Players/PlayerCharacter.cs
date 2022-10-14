using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerCharacter : Entity
{
    public const string ANIMATOR_ARGS_VELOCITY = "Velocity";
    public const string ANIMATOR_ARGS_HORIZONTAL = "Horizontal";
    public const string ANIMATOR_ARGS_VERTICAL = "Vertical";
    public const string ANIMATOR_ARGS_ATTACKING = "Attacking";
    public const string ANIMATOR_ARGS_ATTACKINDEX = "AttackIndex";

    private Vector2 velocity;
    public Vector2 Velocity { get => velocity; }

    [SerializeField] private FSM_Player_Manager stateManager;
    public FSM_Player_Manager StateManager { get => stateManager; }

    [SerializeField] private PlayerWeapon weapon;
    public PlayerWeapon Weapon { get => weapon; }

    [SerializeField] private SkillBase skill;
    public SkillBase Skill { get => skill; }

    [SerializeField] private Image hpBar;
    [SerializeField] private Image skillIcon;

    //private PlayerControls playerControls;

    protected override void Awake()
    {
        base.Awake();

        /*
        playerControls = new PlayerControls();
        playerControls.InGame.Enable();
        */
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(0)) StartAttack();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void StartAttack()
    {
        //if (attack_TIMER > 0) return;

        //attack_TIMER = GetStats.Attack_COOLDOWN;
        //weapon.DamageEnemiesInRange();

        if (!weapon.prepareNextAttack) weapon.prepareNextAttack = true;
        else weapon.inputStored = true;
    }

    public void SetInGameControlsState(bool state)
    {
        /*
        if (state) playerControls.InGame.Enable();
        else playerControls.InGame.Disable();
        */
    }

    public void ReadMovementsInputs()
    {
        //this.velocity.x = playerControls.InGame.Movements.ReadValue<Vector2>().x;
        //this.velocity.y = playerControls.InGame.Movements.ReadValue<Vector2>().y;

        this.velocity.x = Input.GetAxis("Horizontal");
        this.velocity.y = Input.GetAxis("Vertical");
    }

    public void Movements()
    {
        velocity = Vector2.ClampMagnitude(velocity, GetStats.Speed);
        this.rb.MovePosition(this.rb.position + velocity * GetStats.Speed * Time.fixedDeltaTime);
    }

    public override bool OnTakeDamages(float amount, bool isCrit = false)
    {
        bool res;
        res = base.OnTakeDamages(amount, isCrit);

        if (hpBar != null)
            hpBar.fillAmount = (currentHP / GetStats.MaxHP);

        return res;
    }

    public override void OnHeal(float amount, bool isCrit = false, bool canExceedMaxHP = false)
    {
        base.OnHeal(amount, isCrit, canExceedMaxHP);

        if (hpBar != null)
            hpBar.fillAmount = (currentHP / GetStats.MaxHP);
    }

    public void SetAnimatorTrigger(string trigger) => animator.SetTrigger(trigger);
    public void SetAnimatorArgs(string args, int value) => animator.SetInteger(args, value);
    public void SetAnimatorArgs(string args, float value) => animator.SetFloat(args, value);
    public void SetAnimatorArgs(string args, bool value) => animator.SetBool(args, value);
}

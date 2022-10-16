using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using BalDUtilities.MouseUtils;
using Newtonsoft.Json.Serialization;

public class PlayerCharacter : Entity
{
    #region Animator args

    public const string ANIMATOR_ARGS_VELOCITY = "Velocity";
    public const string ANIMATOR_ARGS_HORIZONTAL = "Horizontal";
    public const string ANIMATOR_ARGS_VERTICAL = "Vertical";
    public const string ANIMATOR_ARGS_ATTACKING = "Attacking";
    public const string ANIMATOR_ARGS_ATTACKINDEX = "AttackIndex";

    #endregion

    private Vector2 velocity;
    public Vector2 Velocity { get => velocity; }

    [SerializeField] private FSM_Player_Manager stateManager;
    public FSM_Player_Manager StateManager { get => stateManager; }

    [SerializeField] private PlayerWeapon weapon;
    public PlayerWeapon Weapon { get => weapon; }

    [SerializeField] private SkillHolder skillHolder;
    public SkillHolder GetSkillHolder { get => skillHolder; }

    [SerializeField] private float dash_COOLDOWN;
    public float Dash_COOLDOWN { get => dash_COOLDOWN; }

    [SerializeField] private float dashForce;
    public float DashForce { get => dashForce; }
    
    [SerializeField] private float dash_DURATION;
    public float Dash_DURATION { get => dash_DURATION; }

    [SerializeField] private Image hpBar;
    [SerializeField] private Image skillIcon;

    private float dash_CD_TIMER;
    public bool isDashing;

    [SerializeField] private int money;
    public int Money { get => money; }

    [SerializeField] private int playerIndex;
    public int PlayerIndex { get => playerIndex; }  

    //private PlayerControls playerControls;

    #region A/S/U/F

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

        GameManager.Instance._onSceneReload += OnSceneReload;

        SetKeepedData();
    }

    protected override void Update()
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;
        base.Update();

        if (Input.GetMouseButtonDown(0)) StartAttack();
        if (Input.GetKeyDown(KeyCode.LeftShift) && dash_CD_TIMER <= 0) StartDash();

        if (dash_CD_TIMER > 0) dash_CD_TIMER -= Time.deltaTime;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    #endregion

    #region Controls / Movements

    public void SetVelocity(Vector2 _velocity) => velocity = _velocity;

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

    #endregion

    #region Attack / Damages / Heal

    public void StartAttack()
    {
        //if (attack_TIMER > 0) return;

        //attack_TIMER = GetStats.Attack_COOLDOWN;
        //weapon.DamageEnemiesInRange();

        if (!weapon.prepareNextAttack) weapon.prepareNextAttack = true;
        else weapon.inputStored = true;
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

    public override void OnDeath(bool forceDeath = false)
    {
        base.OnDeath(forceDeath);
        GameManager.Instance.GameState = GameManager.E_GameState.GameOver;
    }

    #endregion

    public void AddMoney(int amount) => money += amount;
    public void RemoveMoney(int amount, bool canGoInDebt)
    {
        if (!canGoInDebt && money < 0) return;

        money -= amount;

        if (!canGoInDebt && money < 0) money = 0;
    }
    public bool HasEnoughMoney(int price) => money > price ? true : false;

    private void StartDash()
    {
        isDashing = true;
        dash_CD_TIMER = dash_COOLDOWN;
    }

    public void SetSkillThumbnail(Sprite image) => skillIcon.sprite = image;
    public void UpdateSkillThumbnailFill(float fill) => skillIcon.fillAmount = fill;

    public void OffsetSkillHolder(float offset)
    {
        skillHolder.transform.localPosition += (Vector3)weapon.GetDirectionOfMouse() * offset;
    }

    #region Set animators

    public void SetAnimatorTrigger(string trigger) => animator.SetTrigger(trigger);
    public void SetAnimatorArgs(string args, int value) => animator.SetInteger(args, value);
    public void SetAnimatorArgs(string args, float value) => animator.SetFloat(args, value);
    public void SetAnimatorArgs(string args, bool value) => animator.SetBool(args, value);

    #endregion

    private void SetKeepedData()
    {
        this.playerIndex = GameManager.Instance.SetPlayerIndex(this);
        this.money = DataKeeper.Instance.playersDataKeep[this.playerIndex].money;
    }

    private void OnSceneReload()
    {
        DataKeeper.Instance.playersDataKeep[this.playerIndex].money = this.money;
    }
}

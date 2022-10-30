using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using BalDUtilities.MouseUtils;
using TMPro;
using Unity.VisualScripting;

public class PlayerCharacter : Entity
{
    #region Animator args

    public const string ANIMATOR_ARGS_ATTACKING = "Attacking";
    public const string ANIMATOR_ARGS_ATTACKINDEX = "AttackIndex";
    public const string ANIMATOR_ARGS_INSKILL = "InSkill";
    public const string ANIMATOR_ARGS_DASHING = "Dashing";

    #endregion

    #region vars

    private Vector2 velocity;
    public Vector2 Velocity { get => velocity; }

    [SerializeField] private FSM_Player_Manager stateManager;
    public FSM_Player_Manager StateManager { get => stateManager; }

    [SerializeField] private PlayerWeapon weapon;
    public PlayerWeapon Weapon { get => weapon; }

    [SerializeField] private SkillHolder skillHolder;
    public SkillHolder GetSkillHolder { get => skillHolder; }

    public SCRPT_Skill GetSkill { get => skillHolder.Skill; }

    [SerializeField] private PlayerInput inputs;
    public PlayerInput Inputs { get => inputs; }

    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image skillIcon;

    private float dash_CD_TIMER;
    public bool isDashing;

    [SerializeField] private int money;
    public int Money { get => money; }

    [SerializeField] private int playerIndex;
    public int PlayerIndex { get => playerIndex; }

    [SerializeField] private int level;
    public int Level { get => level; }

    [SerializeField] private SCRPT_Dash playerDash;
    public SCRPT_Dash PlayerDash { get => playerDash; }

    [SerializeField] private List<EnemyBase> attackers = new List<EnemyBase>();
    public List<EnemyBase> Attackers { get => attackers; }

#if UNITY_EDITOR
    public bool debugPush;
    private Vector2 gizmosMouseDir;
    private Ray gizmosPushRay;
    private float gizmosPushEnd;
    private float gizmosPushDrag;
#endif

    private PlayerControls playerControls;

    public delegate void D_AttackInput();
    public D_AttackInput D_attackInput;

    public delegate void D_SkillInput();
    public D_SkillInput D_skillInput;

    public delegate void D_DashInput();
    public D_DashInput D_dashInput;

    #endregion

    #region A/S/U/F

    protected override void Awake()
    {
        base.Awake();
        
        playerControls = new PlayerControls();
        playerControls.InGame.Enable();
        
    }

    protected override void Start()
    {
        base.Start();

        GameManager.Instance._onSceneReload += OnSceneReload;

        SetKeepedData();

        UIManager.Instance.D_exitPause += SwitchControlMapToInGame;
    }

    protected override void Update()
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;
        base.Update();

        if (dash_CD_TIMER > 0) dash_CD_TIMER -= Time.deltaTime;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    #endregion

    #region Controls / Movements

    public void SetSelfVelocity(Vector2 _velocity) => velocity = _velocity;
    public void SetAllVelocity(Vector2 _velocity)
    {
        this.velocity = _velocity;
        this.rb.velocity = _velocity;
    }

    public void SetInGameControlsState(bool state)
    {
        if (state) playerControls.InGame.Enable();
        else playerControls.InGame.Disable();
    }
    public void SetUIControlsState(bool state)
    {
        if (state) playerControls.UI.Enable();
        else playerControls.UI.Disable();
    }

    public void SwitchControlMap(string map) => inputs.SwitchCurrentActionMap(map);
    public void SwitchControlMapToInGame() => inputs.SwitchCurrentActionMap("InGame");
    public void SwitchControlMapToUI() => inputs.SwitchCurrentActionMap("UI");

    public void ReadMovementsInputs()
    {
        if (inputs.currentActionMap.name.Equals("InGame") == false) return;
        
        this.velocity = Vector2.zero;
        this.velocity.x = playerControls.InGame.Movements.ReadValue<Vector2>().x;
        this.velocity.y = playerControls.InGame.Movements.ReadValue<Vector2>().y;

        /*
        if (Input.GetKey(KeyCode.Z))
            this.velocity.y = 1;
        if (Input.GetKey(KeyCode.S))
            this.velocity.y = -1;

        if (Input.GetKey(KeyCode.D))
            this.velocity.x = 1;
        if (Input.GetKey(KeyCode.Q))
            this.velocity.x = -1;
        */
    }

    public void Movements()
    {
        velocity = Vector2.ClampMagnitude(velocity, GetStats.Speed(StatsModifiers));
        this.rb.MovePosition(this.rb.position + velocity * GetStats.Speed(StatsModifiers) * Time.fixedDeltaTime);
    }

    #endregion

    #region Attack / Damages / Heal

    public void StartAttack()
    {
        if (!weapon.prepareNextAttack) weapon.prepareNextAttack = true;
        else weapon.inputStored = true;
    }

    public override bool OnTakeDamages(float amount, bool isCrit = false)
    {
        bool res;
        res = base.OnTakeDamages(amount, isCrit);

        if (hpBar != null)
            hpBar.fillAmount = (currentHP / GetStats.MaxHP(StatsModifiers));
        if (hpText != null)
            hpText.text = $"{currentHP} / {GetStats.MaxHP(StatsModifiers)}";

        return res;
    }

    public override void OnHeal(float amount, bool isCrit = false, bool canExceedMaxHP = false)
    {
        base.OnHeal(amount, isCrit, canExceedMaxHP);

        if (hpBar != null)
            hpBar.fillAmount = (currentHP / GetStats.MaxHP(StatsModifiers));
        if (hpText != null)
            hpText.text = $"{currentHP} / {GetStats.MaxHP(StatsModifiers)}";
    }

    public override void OnDeath(bool forceDeath = false)
    {
        base.OnDeath(forceDeath);
        GameManager.Instance.GameState = GameManager.E_GameState.GameOver;
    }

    #endregion

    #region Money

    public void AddMoney(int amount) => money += amount;
    public void RemoveMoney(int amount, bool canGoInDebt)
    {
        if (!canGoInDebt && money < 0) return;

        money -= amount;

        if (!canGoInDebt && money < 0) money = 0;
    }
    public bool HasEnoughMoney(int price) => money > price ? true : false;

    #endregion

    #region Inputs
    
    public void AttackInput(InputAction.CallbackContext context)
    {
        if (context.performed) D_attackInput?.Invoke();
    }

    public void SkillInput(InputAction.CallbackContext context)
    {
        if (context.performed) D_skillInput?.Invoke();
    }

    public void DashInput(InputAction.CallbackContext context)
    {
        if (context.performed) D_dashInput?.Invoke();
    } 

    #endregion

    public void CancelAttackAnimation()
    {
        this.animator.Play("AN_Wh_Idle");
        weapon.EffectAnimator.Play("Main State");
        weapon.ResetAttack();
    }

    public void StartDash()
    {
        if (dash_CD_TIMER > 0) return;

        isDashing = true;
    }

    public void SetSkillThumbnail(Sprite image) => skillIcon.sprite = image;
    public void UpdateSkillThumbnailFill(float fill) => skillIcon.fillAmount = fill;

    public void OffsetSkillHolder(float offset)
    {
        skillHolder.transform.localPosition = (Vector3)weapon.GetDirectionOfMouse() * offset;
    }

    public void RotateSkillHolder()
    {
        switch (weapon.GetDirectionOfMouse())
        {
            case Vector2 v when v.Equals(Vector2.left):
                skillHolder.transform.eulerAngles = new Vector3(0, 0, -90);
                break;

            case Vector2 v when v.Equals(Vector2.right):
                skillHolder.transform.eulerAngles = new Vector3(0, 0, 90);
                break;

            case Vector2 v when v.Equals(Vector2.up):
                skillHolder.transform.eulerAngles = new Vector3(0, 0, -180);
                break;

            case Vector2 v when v.Equals(Vector2.down):
                skillHolder.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
        }
    }

    private void SetKeepedData()
    {
        this.playerIndex = GameManager.Instance.SetPlayerIndex(this);
        this.money = DataKeeper.Instance.playersDataKeep[this.playerIndex].money;
    }

    private void OnSceneReload()
    {
        DataKeeper.Instance.playersDataKeep[this.playerIndex].money = this.money;
        DataKeeper.Instance.playersDataKeep[this.playerIndex].maxLevel = this.Level;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
        d_EnteredTrigger?.Invoke(collision);
    }

    private float CalculateAllKeys()
    {
        float res = 0;
        for (int i = 0; i < playerDash.DashSpeedCurve.length; i++)
        {
            if (i + 1 < playerDash.DashSpeedCurve.length)
                res += CalculateSingleKey(i, i + 1);
        }

        return res;
    }
    private float CalculateSingleKey(int index1, int index2)
    {
        Keyframe startKey = playerDash.DashSpeedCurve[index1];
        Keyframe endKey = playerDash.DashSpeedCurve[index2];

        float res = ((startKey.value + endKey.value) / 2) * (endKey.time - startKey.time);

        return res;
    }

    public void LevelUp() => level++;

    public bool AddAttacker(EnemyBase attacker)
    {
        if (attackers.Count >= GameManager.MaxAttackers) return false;
        if (attackers.Contains(attacker)) return false;

        attackers.Add(attacker);
        return true;
    }

    public void RemoveAttacker(EnemyBase attacker)
    {
        attackers.Remove(attacker);
    }

    public void PauseInputRelay(InputAction.CallbackContext context)
    {
        if (context.started) GameManager.Instance.HandlePause();
    }

    #region Gizmos

    protected override void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!debugMode) return;
        base.OnDrawGizmos();
        GizmosDrawDashPush();
#endif
    }

    private void GizmosDrawDashPush()
    {
#if UNITY_EDITOR

        if (stateManager.ToString().Equals("Dashing") == false)
        {
            gizmosMouseDir = Vector2.zero;
            Vector2 mousPos = MousePosition.GetMouseWorldPosition();
            gizmosMouseDir = (mousPos - (Vector2)this.transform.position).normalized;

            gizmosPushEnd = 1;

            gizmosPushEnd = CalculateAllKeys();
            gizmosPushRay = new Ray(this.transform.position,gizmosMouseDir * gizmosPushEnd);
        }

        //if (stateManager.ToString().Equals("Dashing"))
        ////    Gizmos.DrawLine(this.transform.position, mouseDir * pushForce * stateManager.dashingState.GetRemainingTimeByMax());
        //else
        Gizmos.DrawRay(this.transform.position, gizmosMouseDir * gizmosPushEnd);

        RaycastHit2D[] rh = Physics2D.RaycastAll(this.transform.position, gizmosMouseDir * gizmosPushEnd);
        foreach (var item in rh)
        {
            if (item.collider.CompareTag("Enemy") == false) continue;

            Vector2 origin = ((Vector2)item.collider.transform.position - item.point).normalized;

            // we take the time needed to travel to the Point with the Dash Velocity,
            // then multiply it by the max time of the Dash Speed Curve.
            // this roughly simulates how much Dash Time would remain if we actually dashed. (current time / max time)
            float currentDistanceByMax = (gizmosPushEnd - Vector2.Distance(this.transform.position, item.point)) * 2;
            float maxTime = playerDash.DashSpeedCurve[playerDash.DashSpeedCurve.length - 1].time;
            float dashVel = playerDash.DashSpeedCurve.Evaluate(0);

            float remainingPushForce = playerDash.PushForce * (currentDistanceByMax / dashVel * maxTime);

            float finalForce = remainingPushForce - item.collider.GetComponentInParent<Entity>().GetStats.Weight;
            Gizmos.DrawRay(item.point, (origin * finalForce));
        }
#endif
    } 

    #endregion
}

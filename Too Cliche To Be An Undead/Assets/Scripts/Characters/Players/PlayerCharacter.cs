using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using BalDUtilities.MouseUtils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Users;
using System.Linq;

public class PlayerCharacter : Entity, IInteractable
{
    #region Animator args

    public const string ANIMATOR_ARGS_ATTACKING = "Attacking";
    public const string ANIMATOR_ARGS_ATTACKINDEX = "AttackIndex";
    public const string ANIMATOR_ARGS_INSKILL = "InSkill";
    public const string ANIMATOR_ARGS_DASHING = "Dashing";

    #endregion

    #region vars

    [SerializeField] private int playerIndex;

    [SerializeField] private FSM_Player_Manager stateManager;

    [SerializeField] private PlayerWeapon weapon;
    [SerializeField] private SkillHolder skillHolder;
    [SerializeField] private SCRPT_Dash playerDash;

    [SerializeField] private List<EnemyBase> attackers = new List<EnemyBase>();

    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Material defaultMaterial;

    [SerializeField] private Image portrait;
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image skillIcon;
    [SerializeField] private Image dashIcon;

    [SerializeField] private static int money;
    [SerializeField] private static int level;

    [SerializeField] private float dyingState_DURATION = 20f;
    [SerializeField][Range(0, 1)] private float reviveHealPercentage = 0.25f;

    private float dash_CD_TIMER;
    public bool isDashing;

    private bool stayStatic;

    private Vector2 velocity;

    private Vector2 lastDirection;

    [SerializeField] private PlayerInput inputs;

    private PlayerControls playerControls;

    public delegate void D_AttackInput();
    public D_AttackInput D_attackInput;

    public delegate void D_StartHoldAttackInput();
    public D_StartHoldAttackInput D_startHoldAttackInput;

    public delegate void D_EndHoldAttackInput();
    public D_EndHoldAttackInput D_endHoldAttackInput;

    public delegate void D_SkillInput();
    public D_SkillInput D_skillInput;

    public delegate void D_DashInput();
    public D_DashInput D_dashInput;

    public delegate void D_AimInput(Vector2 val);
    public D_AimInput D_aimInput;

    private InputAction movementsAction;


    public FSM_Player_Manager StateManager { get => stateManager; }
    public PlayerWeapon Weapon { get => weapon; }
    public SkillHolder GetSkillHolder { get => skillHolder; }
    public SCRPT_Skill GetSkill { get => skillHolder.Skill; }
    public Vector2 Velocity { get => velocity; }
    public PlayerInput Inputs { get => inputs; }
    public int Money { get => money; }
    public int PlayerIndex { get => playerIndex; }
    public int Level { get => level; }
    public float DyingState_DURATION { get => dyingState_DURATION; }
    public SCRPT_Dash PlayerDash { get => playerDash; }
    public List<EnemyBase> Attackers { get => attackers; }
    public Vector2 LastDirection { get => lastDirection; }

    public const string SCHEME_KEYBOARD = "Keyboard&Mouse";
    public const string SCHEME_GAMEPAD = "Gamepad";

#if UNITY_EDITOR
    public bool debugPush;
    private Vector2 gizmosMouseDir;
    private Ray gizmosPushRay;
    private float gizmosPushEnd;
    private float gizmosPushDrag;
#endif

    #endregion

    #region A/S/U/F

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (DataKeeper.Instance.playersDataKeep.Count <= 0) return;


        stateManager.SwitchState(stateManager.idleState);

        if (scene.name.Equals("MainMenu"))
        {
            if (this.playerIndex > 0)
            {
                DataKeeper.Instance.RemoveData(this.playerIndex);
                Destroy(this.gameObject);
                return;
            }
            SwitchControlMapToUI();
        }
        else
        {
            //PlayersManager.Instance.AddAlivePlayer();
            this.transform.position = GameManager.Instance.SpawnPoints[this.playerIndex].position;
            SwitchControlMapToInGame();

            GameManager.E_CharactersNames character = DataKeeper.Instance.playersDataKeep[this.PlayerIndex].character;

            UIManager.PlayerHUD pHUD = UIManager.Instance.PlayerHUDs[this.playerIndex];

            if (pHUD.container != null)
            {
                pHUD.container.SetActive(true);

                this.portrait = pHUD.portrait;
                this.hpBar = pHUD.hpBar;
                this.hpText = pHUD.hpText;
                this.skillIcon = pHUD.skillThumbnail;
                this.dashIcon = pHUD.dashThumbnail;
            }

            PlayersManager.PlayerCharacterComponents pcc = PlayersManager.Instance.GetCharacterComponents(character);

            SwitchCharacter(pcc.dash, pcc.skill, pcc.stats, pcc.sprite, pcc.character);

            this.currentHP = GetStats.MaxHP(StatsModifiers);
        }

        if (this.playerIndex == 0) GameManager.Instance.SetPlayer1(this);

        this.stateManager.SwitchState(stateManager.idleState);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected override void Awake()
    {
        base.Awake();
        
        playerControls = new PlayerControls();
        playerControls.InGame.Enable();

        if (SceneManager.GetActiveScene().name.Equals("MainMenu"))
            SwitchControlMapToUI();
        else
            SwitchControlMapToInGame();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void Start()
    {
        DontDestroyOnLoad(this);
        base.Start();

        movementsAction = inputs.actions.FindAction("Movements");

        GameManager.Instance._onSceneReload += OnSceneReload;

        SetKeepedData();

        inputs.neverAutoSwitchControlSchemes = this.playerIndex != 0;

        UIManager.Instance.D_exitPause += SwitchControlMapToInGame;

        if (this.hpBar == null)
        {
            UIManager.PlayerHUD pHUD = UIManager.Instance.PlayerHUDs[0];

            if (pHUD.container != null)
            {
                pHUD.container.SetActive(true);
                this.hpBar = pHUD.hpBar;
                this.hpText = pHUD.hpText;
                this.skillIcon = pHUD.skillThumbnail;

                pHUD.portrait.sprite = UIManager.Instance.GetPortrait(GameManager.E_CharactersNames.Whitney);
            }
        }

        SetDashThumbnail(playerDash.Thumbnail);
    }

    protected override void Update()
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;
        base.Update();

        if (dash_CD_TIMER > 0)
        {
            dash_CD_TIMER -= Time.deltaTime;
            if (dashIcon != null)
                dashIcon.fillAmount = -((dash_CD_TIMER / playerDash.Dash_COOLDOWN) - 1);
        }
    }

    private void LateUpdate()
    {
        Vector3 minScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minScreenBounds.x + 1, maxScreenBounds.x - 1), Mathf.Clamp(transform.position.y, minScreenBounds.y + 1, maxScreenBounds.y - 1), transform.position.z);
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
        if (this.rb != null)
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

    public void ReadMovementsInputs(InputAction.CallbackContext context)
    {
        if (inputs.currentActionMap?.name.Equals("InGame") == false) return;
        velocity = context.ReadValue<Vector2>();
        if (velocity != Vector2.zero) lastDirection = velocity;
    }

    public void ForceUpdateMovementsInput()
    {
        movementsAction.Disable();
        movementsAction.Enable();
    }

    public void StartAction(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        if (!context.performed) return;

        InputDevice d = context.control.device;

        Debug.Log(d);
        if (!InputUser.all[0].pairedDevices.Contains(d)) return;

        GameManager.ChangeScene(GameManager.E_ScenesNames.MainScene);
    }

    public void Movements()
    {
        if (stayStatic) return;
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

    public void CancelAttackAnimation()
    {
        this.animator.Play("AN_Wh_Idle");
        weapon.EffectAnimator.Play("Main State");
        weapon.ResetAttack();
    }

    public override bool OnTakeDamages(float amount, bool isCrit = false)
    {
        if (!IsAlive()) return false;

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
        if (this.stateManager.ToString().Equals("Dying")) return;
        
        base.OnDeath(forceDeath);
        this.stateManager.SwitchState(stateManager.dyingState);
    }

    public void Revive()
    {
        this.OnHeal(this.stats.MaxHP(statsModifiers) * reviveHealPercentage);
        stateManager.SwitchState(stateManager.idleState);
    }

    public void DefinitiveDeath()
    {
        stateManager.SwitchState(stateManager.deadState);

        PlayersManager.Instance.DefinitiveDeath(this);
    }

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

    #endregion

    #region Money

    public static void AddMoney(int amount) => money += amount;
    public static void RemoveMoney(int amount, bool canGoInDebt)
    {
        if (!canGoInDebt && money < 0) return;

        money -= amount;

        if (!canGoInDebt && money < 0) money = 0;
    }
    public static void SetMoney(int newMoney) => money = newMoney;
    public static int GetMoney() => money;
    public static bool HasEnoughMoney(int price) => money > price ? true : false;

    #endregion

    #region Inputs
    
    public void AttackInput(InputAction.CallbackContext context)
    {
        if (context.performed) D_attackInput?.Invoke();
    }

    public void MaintainedAttackIpunt(InputAction.CallbackContext context)
    {
        if (context.started) D_startHoldAttackInput?.Invoke();
        else if (context.canceled) D_endHoldAttackInput?.Invoke();
    }

    public void SkillInput(InputAction.CallbackContext context)
    {
        if (context.performed) D_skillInput?.Invoke();
    }

    public void DashInput(InputAction.CallbackContext context)
    {
        if (context.performed) D_dashInput?.Invoke();
    } 

    public void SelectInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //PlayersManager.Instance.JoinAction(context);
        }
    }

    public void LeftArrowInput(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.MainMenu) return;
        if (context.performed) UIManager.Instance.PlayerLeftArrowOnPanel(playerIndex);
    }

    public void RightArrowInput(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.MainMenu) return;
        if (context.performed) UIManager.Instance.PlayerRightArrowOnPanel(playerIndex);
    }

    public void QuitLobby(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.MainMenu) return;
        if (context.performed) GameManager.Instance.QuitLobby(PlayerIndex);
    }

    public void PauseInputRelay(InputAction.CallbackContext context)
    {
        if (context.started) GameManager.Instance.HandlePause();
    }

    public void StayStaticInput(InputAction.CallbackContext context)
    {
        if (context.started) stayStatic = true;
        else if (context.canceled) stayStatic = false;
    }

    public void AimInput(InputAction.CallbackContext context)
    {
        if (context.performed) D_aimInput?.Invoke(context.ReadValue<Vector2>());
    }

    public void ScrollCurrentBarDown(InputAction.CallbackContext context)
    {
        UIManager.Instance.ScrollCurrentBarDown(context);
    }
    public void ScrollCurrentBarUp(InputAction.CallbackContext context)
    {
        UIManager.Instance.ScrollCurrentBarUp(context);
    }

    #endregion

    #region Skill

    public void SetSkillThumbnail(Sprite image)
    {
        if (skillIcon == null) return;

        skillIcon.sprite = image;
    }
    public void UpdateSkillThumbnailFill(float fill) => skillIcon.fillAmount = fill;

    public void OffsetSkillHolder(float offset)
    {
        skillHolder.transform.localPosition = (Vector3)weapon.GetGeneralDirectionOfMouseOrGamepad() * offset;
    }

    public void RotateSkillHolder()
    {
        switch (weapon.GetGeneralDirectionOfMouseOrGamepad())
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

    #endregion


    #region Dash / Push

    public void SetDashThumbnail(Sprite image)
    {
        if (dashIcon == null) return;

        dashIcon.sprite = image;
    }

    public void StartDash()
    {
        if (dash_CD_TIMER > 0) return;

        isDashing = true;
    }

    public void StartDashTimer() => dash_CD_TIMER = playerDash.Dash_COOLDOWN;

    public override Vector2 Push(Vector2 pusherPosition, float pusherForce, Entity originalPusher)
    {
        if (!canBePushed) return Vector2.zero;

        Vector2 v = base.Push(pusherPosition, pusherForce, originalPusher);

        if (v.magnitude <= Vector2.zero.magnitude) return Vector2.zero;

        stateManager.SwitchState(stateManager.pushedState.SetForce(v, originalPusher));

        return v;
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

    #endregion

    #region Keep Data / Scene Reload

    private void SetKeepedData()
    {
        this.playerIndex = DataKeeper.Instance.CreateData(this);
        PlayerCharacter.money = DataKeeper.Instance.money;

        PlayersManager.Instance.SetupPanels(playerIndex);
    }

    public void ForceSetIndex(int idx)
    {
        this.playerIndex = idx;
        this.gameObject.name = "Player " + idx;
    }

    private void OnSceneReload()
    {
        this.StatsModifiers.Clear();
        this.attackers.Clear();

        this.currentHP = stats.MaxHP(StatsModifiers);
        this.hpBar.fillAmount = 1;

        this.stateManager.ResetAll();

        DataKeeper.Instance.money = PlayerCharacter.money;
        DataKeeper.Instance.maxLevel = this.Level;
    }

    #endregion


    public void EnteredInRange(GameObject interactor)
    {
        sprite.material = outlineMaterial;
    }

    public void ExitedRange(GameObject interactor)
    {
        sprite.material = defaultMaterial;
    }

    public void Interact(GameObject interactor)
    {
        Revive();

        sprite.material = GameAssets.Instance.DefaultMaterial;
    }

    public bool CanBeInteractedWith() => this.stateManager.ToString().Equals("Dying"); 


    public void SetAttack(GameObject newWeapon)
    {
        weapon.ResetAttack();
        Destroy(weapon.gameObject);
        GameObject gO = Instantiate(newWeapon, this.transform);
        weapon = gO.GetComponent<PlayerWeapon>();
        stateManager.OwnerWeapon = weapon;
    }

    public static void LevelUp() => level++;
    public static void SetLevel(int newLevel) => level = newLevel;
    public static int GetLevel => PlayerCharacter.level;

    public void SwitchCharacter(SCRPT_Dash newDash, SCRPT_Skill newSkill, SCRPT_EntityStats newStats, Sprite newSprite, GameManager.E_CharactersNames character)
    {
        this.playerDash = newDash;
        if (dashIcon != null)
            this.dashIcon.sprite = newDash.Thumbnail;

        this.skillHolder.ChangeSkill(newSkill);
        this.stats = newStats;
        this.sprite.sprite = newSprite;

        this.portrait.sprite = UIManager.Instance.GetPortrait(character);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
            d_EnteredTrigger?.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
            d_ExitedTrigger?.Invoke(collision);
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

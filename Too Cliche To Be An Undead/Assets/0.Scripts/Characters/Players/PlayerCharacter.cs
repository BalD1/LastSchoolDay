using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Text;
using Spine.Unity;
using UnityEditor.Rendering.LookDev;

public class PlayerCharacter : Entity, IInteractable
{
    #region vars
    
    [SerializeField] public LayerMask feetsBaseLayer;
    [SerializeField] public LayerMask ignoreHoleLayer;

    [SerializeField] private int playerIndex;

    [SerializeField] private FSM_Player_Manager stateManager;

    [SerializeField] private SCRPT_PlayerAudio audioClips;
    public SCRPT_PlayerAudio GetAudioClips { get => audioClips; }

    [SerializeField] private PlayerAudio playerAudio;
    public PlayerAudio GetPlayerAudio { get => playerAudio; }

    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private GameObject minimapMarker;
    public GameObject MinimapMarker { get => minimapMarker;}
    [SerializeField] private PlayerInteractor selfInteractor;
    [SerializeField] private PlayerWeapon weapon;
    [SerializeField] private SkillHolder skillHolder;
    [SerializeField] private Image skillDurationIcon;
    [SerializeField] private SCRPT_Dash playerDash;
    [SerializeField] private TextMeshPro selfReviveText;
    [SerializeField] private GameObject pivotOffset;

    [field: SerializeField] public Collider2D FarChecker { get; private set; }
    [field: SerializeField] public Collider2D CloseChecker { get; private set; }

    [SerializeField] private Collider2D bodyTrigger;
    [SerializeField] private Collider2D hudBoundsTrigger;
    public Collider2D BodyTrigger { get => bodyTrigger; }
    public Collider2D HUDBoundsTrigger { get => hudBoundsTrigger; }

    [SerializeField] private Transform leftArm;
    [SerializeField] private Transform rightArm;
    [SerializeField] private Transform armsParent;
    public Transform ArmsParent { get => armsParent; }
    public Quaternion ArmsRotation { get => leftArm.rotation; }

    [SerializeField] private Animator skillTutorialAnimator;

    [SerializeField] private PlayersManager.GamepadShakeData onTakeDamagesGamepadShake;

    [SerializeField] private List<EnemyBase> attackers = new List<EnemyBase>();

    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Material defaultMaterial;

    [field: SerializeField] public PlayerHUD PlayerHUD { get; private set; }

    [SerializeField] private Vector3 iconsMaxScale = new Vector3(1.3f, 1.3f, 1.3f);
    [SerializeField] private float maxScaleTime = .7f;
    [SerializeField] private LeanTweenType inType = LeanTweenType.easeInSine;
    [SerializeField] private LeanTweenType outType = LeanTweenType.easeOutSine;

    [SerializeField] private static int money;
    [SerializeField] private static int level;

    [SerializeField] private float dyingState_DURATION = 20f;
    [SerializeField][Range(0, 1)] private float reviveHealPercentage = 0.25f;
    public float ReviveHealthPercentage { get => reviveHealPercentage; }

    [field: SerializeField] public int CurrentActiveTimestops { get; private set;} = 0;

    public enum E_Devices
    {
        Keyboard,
        Xbox,
        DualShock,
        Switch,
        None,
    }

    public int selfReviveCount;

    public float MaxSkillCD_M { get; private set; }
    public float MaxDashCD_M { get; private set; }

    private float dash_CD_TIMER;
    public float DashCooldown { get => dash_CD_TIMER; }

    private bool stayStatic;

    private Vector2 velocity;

    private Vector2 lastDirection;

    private Vector2 aimGoal;

    [field: SerializeField] public PlayerInputs PlayerInputsComponent { get; private set; }

    public GameObject PivotOffset { get => pivotOffset; }
    public PlayerAnimationController AnimationController { get => animationController; }
    public FSM_Player_Manager StateManager { get => stateManager; }
    public PlayerInteractor GetInteractor { get => selfInteractor; }
    public PlayerWeapon Weapon { get => weapon; }
    public SkillHolder GetSkillHolder { get => skillHolder; }
    public SCRPT_Skill GetSkill { get => skillHolder.Skill; }
    public Image SkillDurationIcon { get => skillDurationIcon; }
    public Animator SkillTutoAnimator { get => skillTutorialAnimator; }
    public TextMeshPro SelfReviveText { get => selfReviveText; }
    public Vector2 Velocity { get => velocity; }
    public int Money { get => money; }
    public int PlayerIndex { get => playerIndex; }
    public int Level { get => level; }
    public float DyingState_DURATION { get => dyingState_DURATION; }
    public SCRPT_Dash PlayerDash { get => playerDash; }
    public List<EnemyBase> Attackers { get => attackers; }
    public Vector2 LastDirection { get => lastDirection; set => lastDirection = value; }

    [field: SerializeField] public int KillsCount { get; private set; }
    [field: SerializeField] public int DamagesDealt { get; private set; }
    [field: SerializeField] public int DamagesTaken { get; private set; }

    private float gamepadShake_TIMER;

    #endregion

    #region Events

    public Action OnSwitchCharacter;
    public Action<int> OnIndexChange;

    public Action OnAttackInput;
    public Action<bool> OnAttack;
    public Action OnAttackEnded;
    public Action<bool> OnSuccessfulAttack;
    public Action OnSwiff;
    public Action OnStartHoldAttack;
    public Action OnEndHoldAttack;

    public Action OnSkillInput;
    public Action<float, float, float> OnAskForSkill;
    public Action<bool> OnSkillStart;
    public Action<bool> OnSkillEnd;
    public Action OnEarlySkillStart;

    public Action OnDashInput;
    public Action OnDashStarted;
    public Action<Entity> OnDashHit;

    public Action OnFootPrint;
    public Action<Type> OnSteppedIntoTrigger;

    public event Action OnRevive;
    public Action OnSelfReviveInput;
    public event Action<GameObject> OnOtherInteract;
    public Action OnInteractInput;

    public Action<Vector2> OnAimInput;

    public Action OnSecondContextInput;

    public Action<AudioClip> OnOverrideNextVoiceAttackAudio;
    #endregion

    #region A/S/U/F

    private void ResetEndStats()
    {
        KillsCount = 0;
        DamagesDealt = 0;
        DamagesTaken = 0;
    }

    protected override void EventsSubscriber()
    {
        base.EventsSubscriber();
        GameManager.Instance._onSceneReload += OnSceneReload;
    }

    protected override void EventsUnSubscriber()
    {
        base.EventsUnSubscriber();
        GameManager.Instance._onSceneReload -= OnSceneReload;
    }

    protected override void Awake()
    {
        base.Awake();
        return;
        this.MaxSkillCD_M = skillHolder.Skill.Cooldown;
        this.MaxDashCD_M = playerDash.Dash_COOLDOWN;
    }

    public void Setup(PlayerInputs inputs)
    {
        this.PlayerInputsComponent = inputs;
        this.playerIndex = inputs.InputsID;
        inputs.SetOwner(this);

        SO_CharactersComponents pcc = GameAssets.Instance.CharactersComponentsHolder.GetComponents(this.GetCharacterName());
        PlayerHUD = PlayerHUDManager.Instance.CreateNewHUD(this, this.minimapMarker.GetComponent<SpriteRenderer>(), this.GetCharacterName(), pcc.Dash, pcc.Skill);
        PlayerHUD.ForceHPUpdate();
        this.SwitchCharacter(pcc);
        this.minimapMarker.SetActive(true);
    }

    protected override void Update()
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;
        base.Update();

        if (dash_CD_TIMER > 0)
        {
            dash_CD_TIMER -= Time.deltaTime;

            float fillAmount = dash_CD_TIMER / MaxDashCD_M;
            PlayerHUD.UpdateDashThumbnailFill(fillAmount);
        }
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

    public void ReadMovementsInputs(InputAction.CallbackContext context)
        => ReadMovementsInputs(context.ReadValue<Vector2>());
    public void ReadMovementsInputs(Vector2 value)
    {
        velocity = value;
        if (velocity != Vector2.zero) lastDirection = velocity;
    }

    public void Movements()
    {
        if (stayStatic) return;
        velocity = Vector2.ClampMagnitude(velocity, MaxSpeed_M);

        if (velocity.x != 0)
            animationController.FlipSkeleton(velocity.x > 0);

        this.rb.MovePosition(this.rb.position + velocity * MaxSpeed_M * Time.fixedDeltaTime);
    }

    #endregion

    #region Attack / Damages / Heal

    public void CancelAttackAnimation()
    {
        weapon.ResetAttack();
    }

    public override bool OnTakeDamages(float amount, Entity damager, bool isCrit = false, bool fakeDamages = false, bool callDelegate = true, bool tickDamages = false)
    {
        if (!IsAlive()) return false;

        if (GameManager.Instance.IsInTutorial) fakeDamages = true;

        bool res = base.OnTakeDamages(amount, damager, isCrit, fakeDamages, callDelegate);
        if (res == false) return res;

        if (!fakeDamages) DamagesTaken += (int)amount;

        return res;
    }

    public void AddDealtDamages(int amount) => DamagesDealt += amount;

    protected override void ApplyModifier(StatsModifier m)
    {
        switch (m.StatType)
        {
            case StatsModifier.E_StatType.MaxHP:
                MaxHP_M += m.Modifier;
                break;

            case StatsModifier.E_StatType.Damages:
                MaxDamages_M += m.Modifier;
                break;

            case StatsModifier.E_StatType.AttackRange:
                MaxAttRange_M += m.Modifier;
                break;

            case StatsModifier.E_StatType.Attack_CD:
                MaxAttCD_M += m.Modifier;
                break;

            case StatsModifier.E_StatType.Speed:
                MaxSpeed_M += m.Modifier;
                break;

            case StatsModifier.E_StatType.CritChances:
                MaxCritChances_M += (int)m.Modifier;
                break;

            case StatsModifier.E_StatType.DASH_CD:
                MaxDashCD_M += m.Modifier;
                break;

            case StatsModifier.E_StatType.SKILL_CD:
                MaxSkillCD_M += m.Modifier;
                break;
        }
    }

    public override void OnHeal(float amount, bool isCrit = false, bool canExceedMaxHP = false, bool healFromDeath = false)
    {
        base.OnHeal(amount, isCrit, canExceedMaxHP, healFromDeath);

        D_OnHeal?.Invoke();
    }

    public override void Death(bool forceDeath = false)
    {
        if (this.stateManager.ToString().Equals("Dying")) return;
        
        base.Death(forceDeath);
        this.selfInteractor.ResetCollider();
    }

    public void AskRevive()
    {
        OnRevive?.Invoke();
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

    public void ClearAttackers()
    {
        attackers.Clear();
    }

    public void AddKillCount()
    {
        KillsCount++;
    }

    #endregion

    #region Money

    public static void AddMoney(int amount)
    {
        money += amount;
        UIManager.Instance.UpdateMoney();
    }
    public static void RemoveMoney(int amount, bool canGoInDebt)
    {
        if (!canGoInDebt && money < 0) return;

        money -= amount;
        UIManager.Instance.UpdateMoney();

        if (!canGoInDebt && money < 0) money = 0;
    }
    public static void SetMoney(int newMoney)
    {
        money = newMoney;
#if UNITY_EDITOR == false
        if (UIManager.Instance != null);
        UIManager.Instance.UpdateMoney(); 
#endif
    }

    public static int GetMoney() => money;
    public static bool HasEnoughMoney(int price) => money >= price ? true : false;

    #endregion

    #region Inputs

    public void StayStaticInput(InputAction.CallbackContext context)
    {
        if (context.started) stayStatic = true;
        else if (context.canceled) stayStatic = false;
    }

    #endregion

    #region Skill

    public void ResetSkillAnimator()
    {
        this.GetSkillHolder.GetAnimator.ResetTrigger("EndSkill");
        this.GetSkillHolder.GetAnimator.Play("MainState");
    }

    public void OffsetSkillHolder(float offset)
    {
        skillHolder.transform.localPosition = (Vector3)weapon.GetGeneralDirectionOfMouseOrGamepad() * offset;
    }
    public void OffsetChild(float offset)
    {
        skillHolder.transform.GetChild(0).localPosition = Vector2.down * offset;
    }

    public void SetArmsState(bool active, Vector3 newOffset, int skeletonIdx = 0, string boneToFollow = "")
    {
        if (boneToFollow != "")
            animationController.leftArmBone.SetBone(boneToFollow);
        animationController.leftArmBone.SkeletonRenderer = animationController.GetSkeleton(skeletonIdx);

        if (animationController.IsLookingAtRight())
        {
            Vector3 v = newOffset;
            v.x *= -1;
            newOffset = v;
        }

        animationController.leftArmBone.offset = newOffset;

        animationController.leftArmBone.followXPosition = newOffset.x != 0;
        animationController.leftArmBone.followYPosition = newOffset.y != 0;

        if (boneToFollow != "")
            animationController.rightArmBone.SetBone(boneToFollow);
        animationController.rightArmBone.SkeletonRenderer = animationController.GetSkeleton(skeletonIdx);
        animationController.rightArmBone.offset = newOffset;

        animationController.rightArmBone.followXPosition = newOffset.x != 0;
        animationController.rightArmBone.followYPosition = newOffset.y != 0;

        leftArm.gameObject.SetActive(active);
        rightArm.gameObject.SetActive(active);
    }

    public void RotateSkillHolder()
    {
        Vector3 rot = weapon.GetRotationOnMouseOrGamepad().eulerAngles;
        rot.z -= 90;
        skillHolder.transform.eulerAngles = rot;
    }
    public void RotateArms()
    {
        //Vector3 rot = weapon.GetRotationOnMouseOrGamepad().eulerAngles;
        Quaternion rot = Quaternion.identity;

        if (PlayerInputsComponent.currentDeviceType != PlayerInputsManager.E_Devices.Keyboard)
        {
            float lookAngle = Mathf.Atan2(aimGoal.y, aimGoal.x) * Mathf.Rad2Deg;
            rot = Quaternion.Slerp(weapon.transform.rotation, Quaternion.AngleAxis(lookAngle + 180, Vector3.forward), Time.deltaTime * weapon.SlerpSpeed);

            animationController.FlipSkeleton((rot.eulerAngles.z < 270) && (rot.eulerAngles.z > 90));
        }
        else rot = weapon.GetRotationOnMouse();

        bool flipArms = (rot.eulerAngles.z < 270) && (rot.eulerAngles.z > 90);

        Vector2 armsScale = armsParent.localScale;
        armsScale.x = flipArms ? 1 : -1;
        armsParent.localScale = armsScale;

        if (flipArms)
        {
            Vector3 euler = rot.eulerAngles;
            euler.z += 180;
            rot.eulerAngles = euler;
        }

        leftArm.transform.rotation = rot;
        rightArm.transform.rotation = rot;
    }

    #endregion

    #region Dash / Push

    public void StartDashTimer() => dash_CD_TIMER = MaxDashCD_M;

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

    public void ForceSetIndex(int idx)
    {
        this.playerIndex = idx;
        this.gameObject.name = "Player " + idx;

        OnIndexChange?.Invoke(idx);
    }

    private void OnSceneReload()
    {
        this.StatsModifiers.Clear();
        this.attackers.Clear();
        this.skeletonAnimation.timeScale = 1;

        this.currentHP = this.MaxHP_M;
        PlayerHUD.ForceHPUpdate();

        this.stateManager.ResetAll();

        DataKeeper.Instance.money = PlayerCharacter.money;
        DataKeeper.Instance.maxLevel = this.Level;
    }

    #endregion

    #region Tweening

    public void ScaleTweenObject(GameObject target)
    {
        if (target == null) return;

        LeanTween.cancel(target);

        LeanTween.scale(target, iconsMaxScale, maxScaleTime).setEase(inType)
        .setOnComplete(() =>
        {
            LeanTween.scale(target, Vector3.one, maxScaleTime).setEase(outType);
        });
    }
    public void ScaleTweenObject(GameObject target, LeanTweenType _inType, LeanTweenType _outType)
    {
        if (target == null) return;

        LeanTween.cancel(target);

        LeanTween.scale(target, iconsMaxScale, maxScaleTime).setEase(_inType)
        .setOnComplete(() =>
        {
            LeanTween.scale(target, Vector3.one, maxScaleTime).setEase(_outType);
        });
    }
    public void ScaleTweenObject(RectTransform target)
    {
        if (target == null) return;

        LeanTween.cancel(target);

        LeanTween.scale(target, iconsMaxScale, maxScaleTime).setEase(inType)
        .setOnComplete(() =>
        {
            LeanTween.scale(target, Vector3.one, maxScaleTime).setEase(outType);
        });
    }

    #endregion

    #region Interactions

    public void EnteredInRange(GameObject interactor)
    {
    }

    public void ExitedRange(GameObject interactor)
    {
    }

    public void Interact(GameObject interactor)
    {
        OnOtherInteract?.Invoke(interactor);
    }

    public bool CanBeInteractedWith() => this.stateManager.ToString().Equals("Dying");

    public float GetDistanceFrom(Transform target) => Vector2.Distance(this.transform.position, target.position);
 
    #endregion

    #region Level

    public static void LevelUp() => level++;
    public static void SetLevel(int newLevel) => level = newLevel;
    public static int GetLevel => PlayerCharacter.level;

    #endregion

    #region Switch & Set

    public void SwitchCharacter(SO_CharactersComponents pcc, bool callDelegate = true)
        => SwitchCharacter(pcc.Dash, pcc.Skill, pcc.Stats, pcc.Character, pcc.AnimData, pcc.AudioClips, callDelegate);
    public void SwitchCharacter(SCRPT_Dash newDash, SCRPT_Skill newSkill, SCRPT_EntityStats newStats, GameManager.E_CharactersNames character, SCRPT_PlayersAnimData animData, SCRPT_PlayerAudio audioData, bool callDelegate = true)
    {
        DataKeeper.Instance.playersDataKeep[this.playerIndex].character = character;

        this.playerDash = newDash;

        this.skillHolder.ChangeSkill(newSkill);
        this.stats = newStats;

        this.audioClips = audioData;

        animationController.Setup(animData);

        this.animationController.SkeletonAnimation.CurrentSkeletonAnimation.Skeleton.SetColor(Color.white);

        if (animData != null && animData.arms.Length > 0)
            this.leftArm.GetComponent<SpriteRenderer>().sprite = animData.arms[0];
        if (animData != null && animData.arms.Length > 1)
            this.rightArm.GetComponent<SpriteRenderer>().sprite = animData.arms[1];

        ResetStats();

        foreach (var item in statsModifiers)
        {
            this.ApplyModifier(item);
        }

        this.currentHP = this.MaxHP_M;

        Spine.Animation attackAnim = animationController.animationsData.AttackAnim_side?.Animation;
        if (attackAnim != null)
            weapon.attackDuration = attackAnim.Duration;

        CameraManager.Instance.Markers[this.playerIndex].gameObject.SetActive(false);

        if (callDelegate) OnSwitchCharacter?.Invoke();
    }

    public void SetAttack(GameObject newWeapon)
    {
        weapon.ResetAttack();
        Destroy(weapon.gameObject);
        GameObject gO = Instantiate(newWeapon, this.transform);
        weapon = gO.GetComponent<PlayerWeapon>();
        stateManager.OwnerWeapon = weapon;
    }

    #endregion

    public override void Stun(float duration, bool resetAttackTimer = false, bool showStuntext = false)
    {
        AskForStun(duration, resetAttackTimer, showStuntext);
    }

    public void StartTimeStop()
    {
        CurrentActiveTimestops++;
        if (CurrentActiveTimestops > 0) SetInvincibility(true);
    }
    public void StopTimeStop()
    {
        CurrentActiveTimestops--;
        if (CurrentActiveTimestops <= 0) SetInvincibility(false);
    }

    protected override void ResetStats()
    {
        base.ResetStats();

        this.MaxSkillCD_M = skillHolder.Skill.Cooldown;
        this.MaxDashCD_M = playerDash.Dash_COOLDOWN;
    }

    public GameManager.E_CharactersNames GetCharacterName()
    {
        return DataKeeper.Instance.playersDataKeep[this.playerIndex].character;
    }

    public GameManager.E_CharactersNames GetCharacterName(int idx)
    {
        if (idx > DataKeeper.Instance.playersDataKeep.Count) return GameManager.E_CharactersNames.Shirley;
        
        return DataKeeper.Instance.playersDataKeep[idx].character;
    }

    public void TryPlayAudioclip(AudioClip clip, float pitchRange = 0)
    {
        this.source.pitch = UnityEngine.Random.Range(1 - pitchRange, 1 + pitchRange);
        this.source?.PlayOneShot(clip);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        d_EnteredCollider?.Invoke(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        d_ExitedCollider?.Invoke(collision);
    }

    #region Gizmos

    private void OnGUI()
    {
#if UNITY_EDITOR
        if (!debugMode) return;

        StringBuilder sb = new StringBuilder();

        sb.Append("Player ");
        sb.AppendLine(playerIndex.ToString());

        sb.Append("Kills Count : ");
        sb.AppendLine(KillsCount.ToString());

        sb.Append("Damages Dealt : ");
        sb.AppendLine(DamagesDealt.ToString());

        sb.Append("Damages Taken : ");
        sb.AppendLine(DamagesTaken.ToString());

        float height = 100;
        int playersCount = GameManager.Instance.PlayersCount;

        int posY = Screen.height / 2;

        posY += (int)(height * (playerIndex - (playersCount / 2)));

        Rect r = new Rect(10, posY, Screen.width, height);
        GUI.Label(r, sb.ToString());
#endif
    }

    #endregion
}

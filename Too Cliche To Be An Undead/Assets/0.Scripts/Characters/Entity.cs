using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviourEventsHandler, IDamageable
{
    #region Vars

    //*************************************
    //*********** COMPONENTS **************
    //*************************************

    [Header("Base - Entity")]
    [Header("Components")]

    [SerializeField] protected Rigidbody2D rb;
    public Rigidbody2D GetRb { get => rb; }

    [SerializeField] protected SkeletonAnimation skeletonAnimation;
    public SkeletonAnimation SkeletonAnimation { get => skeletonAnimation; set => skeletonAnimation = value; }

    [SerializeField] protected Animator animator;
    public Animator GetAnimator { get => animator; }

    [SerializeField] protected Vector2 healthPopupOffset;
    public Vector2 GetHealthPopupOffset { get => healthPopupOffset; }

    [field: SerializeField] public Collider2D FeetsCollider { get; protected set; }

    //************************************
    //************* AUDIO ****************
    //************************************

    [Header("Audio")]

    [SerializeField] protected AudioSource source;
    public AudioSource GetAudioSource { get => source; }

    //************************************
    //************* STATS ****************
    //************************************

    [Header("Stats")]

    [SerializeField] protected SCRPT_EntityStats stats;
    public SCRPT_EntityStats GetStats { get => stats; }

    private float maxHP_M;
    public float MaxHP_M 
    { 
        get
        {
            if (maxHP_M >= stats.MaxHP_MAX) return stats.MaxHP_MAX;
            return maxHP_M;
        }
        protected set { maxHP_M = value; } 
    }

    private float maxDamages_M;
    public float MaxDamages_M
    {
        get
        {
            if (maxDamages_M >= stats.BaseDamages_MAX) return stats.BaseDamages_MAX;
            return maxDamages_M;
        }
        protected set { maxDamages_M = value;}
    }

    private float maxAttRange_M;
    public float MaxAttRange_M
    {
        get
        {
            if (maxAttRange_M >= stats.AttackRange_MAX) return stats.AttackRange_MAX;
            return maxAttRange_M;
        }
        protected set { maxAttRange_M = value; }
    }

    private float maxAttCD_M;
    public float MaxAttCD_M
    {
        get
        {
            if (maxAttCD_M >= stats.Attack_COOLDOWN_MAX) return stats.Attack_COOLDOWN_MAX;
            return maxAttCD_M;
        }
        protected set { maxAttCD_M = value; }
    }

    private float maxSpeed_M;
    public float MaxSpeed_M
    {
        get
        {
            if (maxSpeed_M >= stats.Speed_MAX) return stats.Speed_MAX;
            return maxSpeed_M;
        }
        protected set { maxSpeed_M = value; }
    }

    private int maxCritChances_M;
    public int MaxCritChances_M
    {
        get
        {
            if (maxCritChances_M >= stats.CritChances_MAX) return stats.CritChances_MAX;
            return maxCritChances_M;
        }
        protected set { maxCritChances_M = value; }
    }

    [SerializeField] protected List<StatsModifier> statsModifiers = new List<StatsModifier>();
    public List<StatsModifier> StatsModifiers { get => statsModifiers; }
    private List<StatsModifier> modifiersToRemove = new List<StatsModifier>();

    [SerializeField][ReadOnly] protected float currentHP;
    public float CurrentHP { get => currentHP; }

    [SerializeField] protected float flashOnHitTime = .1f;

    protected float invincibility_TIMER;
    protected float attack_TIMER;
    public float Attack_TIMER { get => attack_TIMER; }

    private Vector2 lastVelocity;
    public Vector2 LastVelocity { get => lastVelocity; }

    //***********************************
    //************* MISC ****************
    //*********************************** 

    [Header("Misc")]

    [SerializeField] protected bool invincible;

    public delegate void D_enteredTrigger(Collider2D collider);
    public D_enteredTrigger OnEnteredBodyTrigger;

    public delegate void D_exitedTrigger(Collider2D collider);
    public D_exitedTrigger OnExitedBodyTrigger;

    public delegate void D_enteredCollider(Collision2D collision);
    public D_enteredCollider d_EnteredCollider;

    public delegate void D_exitedCollider(Collision2D collision);
    public D_exitedCollider d_ExitedCollider;

    public delegate void D_EntityEnteredCollider(Entity entity);
    public D_EntityEnteredCollider D_entityEnteredCollider;

    public delegate void D_onDeath();
    public D_onDeath d_OnDeath;

    public delegate void D_OnDeathOf(Entity e);
    public D_OnDeathOf D_onDeathOf;

    public delegate void D_OnTakeDamagesFromEntity(bool crit, Entity damager, bool tickDamage = false);
    public D_OnTakeDamagesFromEntity D_onTakeDamagesFromEntity;

    public event Action<bool> OnHealthChange;
    private void HealthChange(bool tookDamages) => OnHealthChange?.Invoke(tookDamages);

    public Action D_OnHeal;

    public event Action<float, Entity, Entity> OnAskForPush;
    public Action OnPushed;

    public event Action<float, bool, bool> OnAskForStun;
    protected void AskForStun(float duration, bool resetAttackTimer, bool showStunText)
        => OnAskForStun?.Invoke(duration, resetAttackTimer, showStunText);

    public Action OnReset;

    public event Action<string> OnStateChange;
    public void CallStateChange(string newState) => OnStateChange?.Invoke(newState);

    [SerializeField] private List<TickDamages> appliedTickDamages = new List<TickDamages>();
    public List<TickDamages> AppliedTickDamages { get => appliedTickDamages; }

    private List<TickDamages> tickDamagesToRemove = new List<TickDamages>();

    public bool canBePushed = true;

    private Coroutine materialFlash;

#if UNITY_EDITOR
    [SerializeField] protected bool debugMode;
#endif

    #endregion

    protected override void EventsSubscriber()
    {
    }

    protected override void EventsUnSubscriber()
    {
    }

    #region Awake / Start / Update

    protected override void Awake()
    {
        base.Awake();
        ResetStats();
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;

        // update every modifiers timers, and remove the outdated ones
        foreach (var item in StatsModifiers)
        {
            if (item.Update(Time.deltaTime)) modifiersToRemove.Add(item);
        }

        foreach (var item in AppliedTickDamages)
        {
            item.Update(Time.deltaTime);
            if (item.IsFinished()) tickDamagesToRemove.Add(item);
        }

        if (invincibility_TIMER > 0) invincibility_TIMER -= Time.deltaTime;
        if (attack_TIMER > 0) attack_TIMER -= Time.deltaTime;
    }

    protected virtual void LateUpdate()
    {
        foreach (var item in modifiersToRemove)
        {
            UnapplyModifier(item);
            StatsModifiers.Remove(item);
        }
        modifiersToRemove.Clear();

        if (tickDamagesToRemove.Count > 0)
        {
            AppliedTickDamages.RemoveAll(x => tickDamagesToRemove.Contains(x));
            tickDamagesToRemove.Clear();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;
        lastVelocity = this.GetRb.velocity;
    }

    #endregion

    #region Status

    public virtual void Stun(float duration, bool resetAttackTimer = false, bool showStuntext = false) {  }

    public void AddTickDamages(TickDamages tick)
    {
        foreach (var item in appliedTickDamages)
        {
            if (item.ID.Equals(tick.ID))
            {
                item.ResetTimer();
                return;
            }
        }

        appliedTickDamages.Add(tick);
    }
    public void AddTickDamages(string _id, float _damages, float _timeBetweenDamages, float _lifetime, Entity origin, bool damageInstantly = false, int critChances = 0)
    {
        TickDamages t = new TickDamages(_id, _damages, _timeBetweenDamages, _lifetime, _owner: this, _origin: origin, damageInstantly, critChances);
        AddTickDamages(t);
    }

    public bool RemoveTickDamages(TickDamages tick, float delay = 0)
    {
        return RemoveTickDamages(tick.ID, delay);
    }
    public bool RemoveTickDamages(string _id, float delay = 0)
    {
        foreach (var item in appliedTickDamages)
        {
            if (item.ID.Equals(_id))
            {
                item.ForceEnd(delay);
                return true;
            }
        }

        return false;
    }

    public void RemoveAllTickDamages()
    {
        foreach (var item in appliedTickDamages)
        {
            item.ForceEnd();
        }
    }

    public bool SearchTickDamages(string _id)
    {
        foreach (var item in appliedTickDamages)
        {
            if (item.ID.Equals(_id)) return true;
        }

        return false;
    }
    public TickDamages GetAppliedTickDamages(string _id)
    {
        foreach (var item in appliedTickDamages)
        {
            if (item.ID.Equals(_id)) return item;
        }

        return null;
    }

    protected virtual void ApplyModifier(StatsModifier m)
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
        }
    }

    protected virtual void UnapplyModifier(StatsModifier m)
    {
        m.InverseModifier();
        ApplyModifier(m);
    }

    public bool CheckIfModifierWillExceed(StatsModifier.E_StatType type, float value, out float exceed)
    {
        exceed = 0;
        switch (type)
        {
            case StatsModifier.E_StatType.MaxHP:
                exceed = (maxHP_M + value) - stats.MaxHP_MAX;
                break;

            case StatsModifier.E_StatType.Damages:
                exceed = (maxDamages_M + value) - stats.BaseDamages_MAX;
                break;

            case StatsModifier.E_StatType.AttackRange:
                exceed = (MaxAttRange_M + value) - stats.AttackRange_MAX;
                break;

            case StatsModifier.E_StatType.Attack_CD:
                exceed = (maxAttCD_M + value) - stats.Attack_COOLDOWN_MAX;
                break;

            case StatsModifier.E_StatType.Speed:
                exceed = (MaxSpeed_M + value) - stats.Speed_MAX;
                break;

            case StatsModifier.E_StatType.CritChances:
                exceed = (MaxCritChances_M + value) - stats.CritChances_MAX;
                break;
        }

        return exceed > 0;
    }
    public void AddModifier(string id, float value, float time, StatsModifier.E_StatType type)
    {
        StatsModifier m = new StatsModifier(id, value, time, type);
        StatsModifiers.Add(m);

        ApplyModifier(m);

        if (type == StatsModifier.E_StatType.MaxHP) this.OnHeal(value);
    }
    public void AddModifier(string id, float value, StatsModifier.E_StatType type)
    {
        AddModifier(id, value, -1, type);
    }

    public void AddModifier(string id, int value, float time, StatsModifier.E_StatType type)
    {
        StatsModifier m = new StatsModifier(id, value, time, type);
        StatsModifiers.Add(m);

        ApplyModifier(m);

        if (type == StatsModifier.E_StatType.MaxHP) this.OnHeal(value);
    }
    public void AddModifier(string id, int value, StatsModifier.E_StatType type)
    {
        AddModifier(id, value, -1, type);
    }

    public void AddModifierUnique(string id, int value, float time, StatsModifier.E_StatType type)
    {
        StatsModifier sM = SearchModifier(id);
        if (sM != null)
        {
            sM.ResetTimer();
            return;
        }

        AddModifier(id, value, time, type);
    }
    public void AddModifierUnique(string id, int value, StatsModifier.E_StatType type)
    {
        AddModifierUnique(id, value, -1, type);
    }
    public void AddModifierUnique(string id, float value, float time, StatsModifier.E_StatType type)
    {
        StatsModifier sM = SearchModifier(id);
        if (sM != null)
        {
            sM.ResetTimer();
            return;
        }

        AddModifier(id, value, type);
    }
    public void AddModifierUnique(string id, float value, StatsModifier.E_StatType type)
    {
        AddModifierUnique(id, value, -1, type);
    }

    public void RemoveModifier(string id)
    {
        StatsModifier m = new StatsModifier("NULL", -1, StatsModifier.E_StatType.MaxHP);

        foreach (var item in StatsModifiers)
        {
            if (item.IDName.Equals(id))
            {
                m = item;
                break;
            }
        }

        if (m.IDName == "NULL") return;

        StatsModifiers.Remove(m);

        UnapplyModifier(m);
    }

    public void RemoveModifier(StatsModifier modifier)
    {
        if (StatsModifiers.Contains(modifier))
        {
            modifiersToRemove.Add(modifier);
            UnapplyModifier(modifier);
        }
    }

    public void RemoveModifiersAll(string id)
    {
        foreach (var item in StatsModifiers)
        {
            if (item.IDName.Equals(id)) modifiersToRemove.Add(item);
        }
        foreach (var item in modifiersToRemove)
        {
            UnapplyModifier(item);
            StatsModifiers.Remove(item);
        }
    }
    public void RemoveModifiersAll()
    {
        foreach (var item in StatsModifiers)
        {
            modifiersToRemove.Add(item);
        }
        foreach (var item in modifiersToRemove)
        {
            UnapplyModifier(item);
            StatsModifiers.Remove(item);
        }
    }

    public StatsModifier SearchModifier(string id)
    {
        foreach (var item in StatsModifiers)
        {
            if (item.IDName.Equals(id)) return item;
        }

        return null;
    }

    #endregion

    #region Damages / Heal

    public virtual bool OnTakeDamages(float amount, Entity damager, bool isCrit = false, bool fakeDamages = false, bool callDelegate = true, bool tickDamages = false)
    {
        if (invincible) return false;
        if (invincibility_TIMER > 0) return false;

        // if the damager is from the same team, return
        if (damager != null)
            if (damager.stats.Team != SCRPT_EntityStats.E_Team.Neutral 
             && damager.stats.Team == this.GetStats.Team) return false;

        if (callDelegate) D_onTakeDamagesFromEntity?.Invoke(isCrit, damager, tickDamages);

        if (isCrit) amount *= 1.5f;

        amount = MathF.Round(amount);

        if (!fakeDamages)
        {
            currentHP -= amount;

            PlayerCharacter player = damager as PlayerCharacter;
            if (player != null)
            {
                player.AddDealtDamages((int)amount);
            }
        }

        if (amount > 0)
        {
            HealthPopup.Create(position: (Vector2)this.transform.position + healthPopupOffset, amount, isHeal: false, isCrit);
            if (materialFlash != null) StopCoroutine(materialFlash);

            if (this.gameObject.activeSelf)
                materialFlash = StartCoroutine(MaterialFlash());
        }

        if (currentHP <= 0)
        {
            currentHP = 0;
            Death();
        }

        HealthChange(true);

        return true;
    }

    public virtual void OnHeal(float amount, bool isCrit = false, bool canExceedMaxHP = false, bool healFromDeath = false)
    {
        if (!IsAlive() && !healFromDeath) return;

        if (isCrit) amount *= 1.5f;

        if (canExceedMaxHP) currentHP += amount;
        else currentHP = Mathf.Clamp(currentHP += amount, 0, MaxHP_M);

        HealthChange(false);

        if (!this.gameObject.activeSelf) return;

        HealthPopup.Create(position: (Vector2)this.transform.position + healthPopupOffset, amount, isHeal: true, isCrit);
    }

    public virtual void Death(bool forceDeath = false)
    {
        if (invincible) return;
        if (!forceDeath && IsAlive()) return;

        d_OnDeath?.Invoke();
        D_onDeathOf?.Invoke(this);

        if (materialFlash != null) StopCoroutine(materialFlash);
        this.skeletonAnimation.Skeleton.SetColor(Color.white);
    }

    public virtual bool IsAlive() => currentHP > 0;

    protected virtual IEnumerator MaterialFlash()
    {
        this.skeletonAnimation.Skeleton.SetColor(Color.red);
        yield return new WaitForSeconds(flashOnHitTime);
        this.skeletonAnimation.Skeleton.SetColor(Color.white);
    }

    #endregion

    public void SetInvincibility(bool _i) => invincible = _i;
    public void SetTimedInvincibility(float time) => invincibility_TIMER = time;

    public bool RollCrit() => UnityEngine.Random.Range(0, 100) <= MaxCritChances_M ? true : false;

    public virtual void AskPush(float pusherForce, Entity pusher, Entity originalPusher)
    {
        OnAskForPush?.Invoke(pusherForce, pusher, originalPusher);
    }

    public void StartAttackTimer(float durationModifier = 0, bool addRandom = false)
    {
        float finalDuration = UnityEngine.Random.Range(MaxAttCD_M, MaxAttCD_M * 2);
        finalDuration += durationModifier;

        attack_TIMER = finalDuration;
    }
    public void ResetAttackTimer() => attack_TIMER = 0;

    protected virtual void ResetStats()
    {
        this.MaxHP_M = GetStats.MaxHP;
        this.MaxDamages_M = GetStats.BaseDamages;
        this.MaxAttRange_M = GetStats.AttackRange;
        this.MaxAttCD_M = GetStats.Attack_COOLDOWN;
        this.MaxSpeed_M = GetStats.Speed;
        this.MaxCritChances_M = GetStats.CritChances;

        this.currentHP = MaxHP_M;
    }

    #region Debug

    public void LogHP()
    {
#if UNITY_EDITOR
        string col = GetStats.GetMarkdownColor();
        Debug.Log("<b><color=" + col + ">" + this.gameObject.name + "</color></b> : " + currentHP + " / " + MaxHP_M + " (" + (currentHP / MaxHP_M * 100) + "% ) ", this.gameObject); ;
#endif
    }

    public void LogEntity()
    {
#if UNITY_EDITOR
        string col = GetStats.GetMarkdownColor();

        Debug.LogFormat("Entity of type <b>[{0}], {1}</b> \n" +
            "MaxHP = {2} \n" +
            "Base Damages = {3}\n" +
            "Attack Range = {4}\n" +
            "Attack CD = {5}\n" +
            "Crit Chances = {6}\n" +
            "Speed = {7}\n" +
            "Team = <color={8}>{9}</color>",
            GetStats.EntityName, this.gameObject.name, maxHP_M, maxDamages_M, maxAttRange_M, maxAttCD_M, maxCritChances_M, maxSpeed_M, col, GetStats.Team, this.gameObject);
#endif
    }

    protected virtual void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!debugMode) return;

        Vector2 healthBordersSize = new Vector2(0.75f, 0.5f);
        Gizmos.DrawWireCube((Vector2)this.transform.position + healthPopupOffset, healthBordersSize);

        Color c = UnityEditor.Handles.color;
        UnityEditor.Handles.color = Color.red;

        Vector2 centeredPosition = (Vector2)this.transform.position + healthPopupOffset;

        if (UnityEditor.SceneView.currentDrawingSceneView == null) return;

        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(centeredPosition);


        Vector2 textOffset = new Vector2(-36, 7.5f);
        Camera cam = UnityEditor.SceneView.currentDrawingSceneView.camera;
        if (cam)
            centeredPosition = cam.ScreenToWorldPoint((Vector2)cam.WorldToScreenPoint(centeredPosition) + textOffset);


        UnityEditor.Handles.Label(centeredPosition, "Health Popup");

        UnityEditor.Handles.color = c;
#endif
    }

    #endregion
}

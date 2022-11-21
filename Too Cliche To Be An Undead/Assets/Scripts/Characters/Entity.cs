using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Playables;

public class Entity : MonoBehaviour, IDamageable
{
    #region Vars

    //*************************************
    //*********** COMPONENTS **************
    //*************************************

    [Header("Base - Entity")]
    [Header("Components")]

    [SerializeField] protected Rigidbody2D rb;
    public Rigidbody2D GetRb { get => rb; }

    [SerializeField] protected SpriteRenderer sprite;
    public SpriteRenderer GetSprite { get => sprite; }

    [SerializeField] protected Animator animator;
    public Animator GetAnimator { get => animator; }

    [SerializeField] protected Vector2 healthPopupOffset;
    public Vector2 GetHealthPopupOffset { get => healthPopupOffset; }

    //************************************
    //************* AUDIO ****************
    //************************************

    [Header("Audio")]

    [SerializeField] protected SCRPT_EntityAudio audioClips;
    public SCRPT_EntityAudio GetAudioClips { get => audioClips; }

    [SerializeField] protected AudioSource source;

    //************************************
    //************* STATS ****************
    //************************************

    [Header("Stats")]

    [SerializeField] protected SCRPT_EntityStats stats;
    public SCRPT_EntityStats GetStats { get => stats; }

    public float maxHP_M { get; protected set; }
    public float maxDamages_M { get; protected set; }
    public float maxAttRange_M { get; protected set; }
    public float maxAttCD_M { get; protected set; }
    public float maxSpeed_M { get; protected set; }
    public int maxCritChances_M { get; protected set; }

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

    public const string ANIMATOR_ARGS_VELOCITY = "Velocity";
    public const string ANIMATOR_ARGS_HORIZONTAL = "Horizontal";
    public const string ANIMATOR_ARGS_VERTICAL = "Vertical";

    [Header("Misc")]

    [SerializeField] protected bool invincible;

    public delegate void D_enteredTrigger(Collider2D collider);
    public D_enteredTrigger d_EnteredTrigger;

    public delegate void D_exitedTrigger(Collider2D collider);
    public D_exitedTrigger d_ExitedTrigger;

    public delegate void D_enteredCollider(Collision2D collision);
    public D_enteredCollider d_EnteredCollider;

    [SerializeField] private List<TickDamages> appliedTickDamages = new List<TickDamages>();
    public List<TickDamages> AppliedTickDamages { get => appliedTickDamages; }

    private List<TickDamages> tickDamagesToRemove = new List<TickDamages>();

    public bool canBePushed = true;

#if UNITY_EDITOR
    [SerializeField] protected bool debugMode;
#endif

    #endregion

    #region Awake / Start / Update

    protected virtual void Awake()
    {
        this.maxHP_M = GetStats.MaxHP;
        this.maxDamages_M = GetStats.BaseDamages;
        this.maxAttRange_M = GetStats.AttackRange;
        this.maxAttCD_M = GetStats.Attack_COOLDOWN;
        this.maxSpeed_M = GetStats.Speed;
        this.maxCritChances_M = GetStats.CritChances;

        this.currentHP = maxHP_M;
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

    public virtual void Stun(float duration, bool resetAttackTimer = false) { throw new System.NotImplementedException(); }

    public void AddTickDamages(TickDamages tick)
    {
        foreach (var item in appliedTickDamages)
        {
            if (item.ID.Equals(tick.ID))
            {
                item.ResetLifetime();
                return;
            }
        }

        appliedTickDamages.Add(tick);
    }
    public void AddTickDamages(string _id, float _damages, float _timeBetweenDamages, float _lifetime)
    {
        TickDamages t = new TickDamages(_id, _damages, _timeBetweenDamages, _lifetime, this);
        AddTickDamages(t);
    }

    protected virtual void ApplyModifier(StatsModifier m)
    {
        switch (m.StatType)
        {
            case StatsModifier.E_StatType.MaxHP:
                maxHP_M += m.Modifier;
                break;

            case StatsModifier.E_StatType.Damages:
                maxDamages_M += m.Modifier;
                break;

            case StatsModifier.E_StatType.AttackRange:
                maxAttRange_M += m.Modifier;
                break;

            case StatsModifier.E_StatType.Attack_CD:
                maxAttCD_M += m.Modifier;
                break;

            case StatsModifier.E_StatType.Speed:
                maxSpeed_M += m.Modifier;
                break;

            case StatsModifier.E_StatType.CritChances:
                maxCritChances_M += (int)m.Modifier;
                break;
        }
    }

    protected virtual void UnapplyModifier(StatsModifier m)
    {
        m.InverseModifier();
        ApplyModifier(m);
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

    public virtual bool OnTakeDamages(float amount, bool isCrit = false)
    {
        if (invincible) return false;
        if (invincibility_TIMER > 0) return false;

        if (isCrit) amount *= 1.5f;

        amount = MathF.Round(amount);

        currentHP -= amount;

        HealthPopup.Create(position: (Vector2)this.transform.position + healthPopupOffset, amount, isHeal: false, isCrit);
        StartCoroutine(MaterialFlash());

        // Si les pv sont <= à 0, on meurt, sinon on joue un son de Hurt

        if (currentHP <= 0)
        {
            currentHP = 0;
            OnDeath();
        }
        else source.PlayOneShot(audioClips.GetRandomHurtClip());

        return true;
    }
    public virtual bool OnTakeDamages(float amount, SCRPT_EntityStats.E_Team damagerTeam, bool isCrit = false)
    {
        if (invincible) return false;
        if (invincibility_TIMER > 0) return false;

        // si la team de l'attaquant est la même que la nôtre, on ne subit pas de dégâts
        if (damagerTeam != SCRPT_EntityStats.E_Team.Neutral && damagerTeam.Equals(this.GetStats.Team)) return false;

        OnTakeDamages(amount, isCrit);

        return true;
    }

    public virtual void OnHeal(float amount, bool isCrit = false, bool canExceedMaxHP = false)
    {
        if (isCrit) amount *= 1.5f;

        if (canExceedMaxHP) currentHP += amount;
        else currentHP = Mathf.Clamp(currentHP += amount, 0, maxHP_M);

        HealthPopup.Create(position: (Vector2)this.transform.position + healthPopupOffset, amount, isHeal: true, isCrit);
    }

    public virtual void OnDeath(bool forceDeath = false)
    {
        if (invincible) return;
        if (!forceDeath && IsAlive()) return;

        source.PlayOneShot(audioClips.GetRandomDeathClip());
    }

    public bool IsAlive() => currentHP > 0;

    public void StartMaterialFlash(string m, float time) => StartCoroutine(MaterialFlash(m, time));

    protected virtual IEnumerator MaterialFlash()
    {
        this.sprite.material.SetInt("_Hit", 1);
        yield return new WaitForSeconds(flashOnHitTime);
        this.sprite.material.SetInt("_Hit", 0);
    }
    protected virtual IEnumerator MaterialFlash(string m, float time)
    {
        this.sprite.material.SetInt(m, 1);
        yield return new WaitForSeconds(time);
        this.sprite.material.SetInt(m, 0);
    }

    #endregion

    #region Set Animator

    public void SetAnimatorTrigger(string trigger) => animator?.SetTrigger(trigger);
    public void SetAnimatorArgs(string args, int value) => animator?.SetInteger(args, value);
    public void SetAnimatorArgs(string args, float value) => animator?.SetFloat(args, value);
    public void SetAnimatorArgs(string args, bool value) => animator?.SetBool(args, value);

    #endregion

    public void SetInvincibility(bool _i) => invincible = _i;

    public bool RollCrit() => UnityEngine.Random.Range(0, 100) <= maxCritChances_M ? true : false;

    public virtual Vector2 Push(Vector2 pusherPosition, float pusherForce, Entity originalPusher)
    {
        if (!canBePushed) return Vector2.zero;

        Vector2 dir = ((Vector2)this.transform.position - pusherPosition).normalized;

        float finalForce = pusherForce - this.GetStats.Weight;
        if (finalForce <= 0) return Vector2.zero;

        Vector2 v = dir * finalForce;
        return v;
    }

    public void StartAttackTimer(float durationModifier = 0) => attack_TIMER = maxAttCD_M + durationModifier;
    public void ResetAttackTimer() => attack_TIMER = 0;

    #region Debug

    public void LogHP()
    {
#if UNITY_EDITOR
        string col = GetStats.GetMarkdownColor();
        Debug.Log("<b><color=" + col + ">" + this.gameObject.name + "</color></b> : " + currentHP + " / " + maxHP_M + " (" + (currentHP / maxHP_M * 100) + "% ) ", this.gameObject); ;
#endif
    }

    public void LogEntity() => GetStats.Log(this.gameObject);

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

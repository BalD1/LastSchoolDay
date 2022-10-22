using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] protected Material hitMaterial;
    public Material GetHitMaterial { get => hitMaterial; }

    [SerializeField] protected Animator animator;
    public Animator GetAnimator { get => animator; }

    [SerializeField] protected Vector2 healthPopupOffset;
    public Vector2 GetHealthPopupOffset { get => healthPopupOffset; }

    protected Material baseMaterial;

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

    [SerializeField] protected List<StatsModifier> statsModifiers = new List<StatsModifier>();
    public List<StatsModifier> StatsModifiers { get => statsModifiers; }
    private List<StatsModifier> modifiersToRemove = new List<StatsModifier>();

    [SerializeField][ReadOnly] protected float currentHP;
    public float CurrentHP { get => currentHP; }

    [SerializeField] protected float flashOnHitTime = .1f;

    protected float invincibility_TIMER;
    protected float attack_TIMER;

    //***********************************
    //************* MISC ****************
    //*********************************** 

    [Header("Misc")]

    [SerializeField] protected bool invincible;

    public delegate void D_enteredTrigger(Collider2D collider);
    public D_enteredTrigger d_EnteredTrigger;

#if UNITY_EDITOR
    [SerializeField] protected bool debugMode;
#endif

    #endregion

    #region Awake / Start / Update

    protected virtual void Awake()
    {
        baseMaterial = this.sprite.material;
    }

    protected virtual void Start()
    {
        currentHP = GetStats.MaxHP(StatsModifiers);
    }

    protected virtual void Update()
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;

        // update every modifiers timers, and remove the outdated ones
        modifiersToRemove = new List<StatsModifier>();
        foreach (var item in StatsModifiers)
        {
            if (item.Update(Time.deltaTime)) modifiersToRemove.Add(item);
        }
        StatsModifiers.RemoveAll(x => modifiersToRemove.Contains(x));

        if (invincibility_TIMER > 0) invincibility_TIMER -= Time.deltaTime;
        if (attack_TIMER > 0) attack_TIMER -= Time.deltaTime;
    }

    protected virtual void FixedUpdate()
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;
    }

    #endregion

    #region Status

    public virtual void Stun(float duration) { }

    public void AddModifier(float time, float value, StatsModifier.E_StatType type) => StatsModifiers.Add(new StatsModifier(time, value, type));
    public void AddModifier(float value, StatsModifier.E_StatType type) => StatsModifiers.Add(new StatsModifier(value, type));

    public void AddModifier(float time, int value, StatsModifier.E_StatType type) => StatsModifiers.Add(new StatsModifier(time, value, type));
    public void AddModifier(int value, StatsModifier.E_StatType type) => StatsModifiers.Add(new StatsModifier(value, type));

    #endregion

    #region Damages / Heal

    public virtual bool OnTakeDamages(float amount, bool isCrit = false)
    {
        if (invincible) return false;
        if (invincibility_TIMER > 0) return false;

        if (isCrit) amount *= 1.5f;

        currentHP -= amount;

        HealthPopup.Create(position: (Vector2)this.transform.position + healthPopupOffset, amount, isHeal: false, isCrit);
        StartCoroutine(FlashOnHit());

        // Si les pv sont <= à 0, on meurt, sinon on joue un son de Hurt

        if (currentHP <= 0) OnDeath();
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
        else currentHP = Mathf.Clamp(currentHP += amount, 0, GetStats.MaxHP(StatsModifiers));

        HealthPopup.Create(position: (Vector2)this.transform.position + healthPopupOffset, amount, isHeal: true, isCrit);
    }

    public virtual void OnDeath(bool forceDeath = false)
    {
        if (invincible) return;
        if (!forceDeath && IsAlive()) return;

        Debug.Log(this.gameObject.name + " iz dead lol x)", this.gameObject);
        source.PlayOneShot(audioClips.GetRandomDeathClip());
    }

    public bool IsAlive() => currentHP > 0;

    protected virtual IEnumerator FlashOnHit()
    {
        this.sprite.material = hitMaterial;
        yield return new WaitForSeconds(flashOnHitTime);
        this.sprite.material = baseMaterial;
    }

    #endregion


    public bool RollCrit() => Random.Range(0, 100) <= GetStats.CritChances(StatsModifiers) ? true : false;

    public virtual void Flip(bool lookAtLeft) => this.sprite.flipX = lookAtLeft;
    public virtual bool IsFacingLeft() => !this.sprite.flipX;

    public virtual Vector2 Push(Vector2 pusherPosition, float pusherForce)
    {
        Vector2 dir = ((Vector2)this.transform.position - pusherPosition).normalized;

        float finalForce = pusherForce - this.GetStats.Weight;
        if (finalForce <= 0) return Vector2.zero;

        Vector2 v = dir * finalForce;
        return v;
    }

    #region Debug

    public void LogHP()
    {
#if UNITY_EDITOR
        string col = GetStats.GetMarkdownColor();
        Debug.Log("<b><color=" + col + ">" + this.gameObject.name + "</color></b> : " + currentHP + " / " + GetStats.MaxHP(StatsModifiers) + " (" + (currentHP / GetStats.MaxHP(StatsModifiers) * 100) + "% ) ", this.gameObject);
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

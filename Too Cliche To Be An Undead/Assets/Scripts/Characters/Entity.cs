using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IDamageable
{
    //*************************************
    //*********** COMPONENTS **************
    //*************************************

    [SerializeField] protected Rigidbody2D rb;
    public Rigidbody2D GetRb { get => rb; }

    [SerializeField] protected SpriteRenderer sprite;
    public SpriteRenderer GetSprite { get => sprite; }

    [SerializeField] protected Material hitMaterial;
    public Material GetHitMaterial { get => hitMaterial; }

    [SerializeField] protected Animator animator;
    public Animator GetAnimator { get => animator; }

    //************************************
    //************* AUDIO ****************
    //************************************

    [SerializeField] protected SCRPT_EntityAudio audioClips;
    public SCRPT_EntityAudio GetAudioClips { get => audioClips; }

    [SerializeField] protected AudioSource source;

    //************************************
    //************* STATS ****************
    //************************************

    [SerializeField] protected SCRPT_EntityStats stats;
    public SCRPT_EntityStats GetStats { get => stats; }

    [SerializeField] [ReadOnly] protected float currentHP;
    public float CurrentHP { get => currentHP; }

    protected float invincibility_TIMER;
    protected float attack_TIMER;

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        currentHP = GetStats.MaxHP;
    }

    protected virtual void Update()
    {
        if (invincibility_TIMER > 0) invincibility_TIMER -= Time.deltaTime;
        if (attack_TIMER > 0) attack_TIMER -= Time.deltaTime;
    }

    protected virtual void FixedUpdate()
    {
    }

    public virtual bool OnTakeDamages(float amount, bool isCrit = false)
    {
        if (invincibility_TIMER > 0) return false;

        if (isCrit) amount *= 1.5f;

        currentHP -= amount;

        return true;
        // Si les pv sont <= à 0, on meurt, sinon on joue un son de Hurt
        //if (currentHP <= 0) OnDeath();
        //else source.PlayOneShot(audioClips.GetRandomHurtClip());
    }
    public virtual bool OnTakeDamages(float amount, SCRPT_EntityStats.E_Team damagerTeam, bool isCrit = false)
    {
        if (invincibility_TIMER > 0) return false;

        // si la team de l'attaquant est la même que la nôtre, on ne subit pas de dégâts
        if (damagerTeam != SCRPT_EntityStats.E_Team.Neutral && damagerTeam.Equals(this.GetStats.Team)) return false;

        OnTakeDamages(amount, isCrit);

        return true;
    }

    public virtual void OnHeal(float amount, bool isCrit = false)
    {
        if (isCrit) amount *= 1.5f;

        currentHP += amount;
        Debug.Log(currentHP);
    }

    public virtual void OnDeath(bool forceDeath = false)
    {
        if (!forceDeath && IsAlive()) return;

        Debug.Log(this.gameObject.name + " iz dead lol x)", this.gameObject);
        source.PlayOneShot(audioClips.GetRandomDeathClip());
    }

    public bool IsAlive() => currentHP > 0;

    public bool RollCrit() => Random.Range(0, 100) >= GetStats.CritChances ? true : false;

    public virtual void Flip(bool lookAtLeft) => this.sprite.flipX = lookAtLeft;
    public virtual bool IsFacingLeft() => !this.sprite.flipX;

    public void LogHP()
    {
#if UNITY_EDITOR
        string col = GetStats.GetMarkdownColor();
        Debug.Log("<b><color=" + col + ">" + this.gameObject.name + "</color></b> : " + currentHP + " / " + GetStats.MaxHP + " (" + (currentHP / GetStats.MaxHP * 100) + "% ) ", this.gameObject);
#endif
    }

    public void LogEntity() => GetStats.Log(this.gameObject);
}

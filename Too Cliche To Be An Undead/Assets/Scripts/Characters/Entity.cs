using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
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

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        currentHP = GetStats.MaxHP;
    }

    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {

    }

    public void OnTakeDamages(float amount, bool isCrit = false)
    {
        if (isCrit) amount *= 1.5f;

        currentHP -= amount;
        Debug.Log(currentHP);

        // Si les pv sont <= à 0, on meurt, sinon on joue un son de Hurt
        if (currentHP <= 0) OnDeath();
        else source.PlayOneShot(audioClips.GetRandomHurtClip());
    }
    public void OnTakeDamages(float amount, SCRPT_EntityStats.E_Team damagerTeam, bool isCrit = false)
    {
        // si la team de l'attaquant est la même que la nôtre, on ne subit pas de dégâts
        if (damagerTeam != SCRPT_EntityStats.E_Team.Neutral && damagerTeam.Equals(this.GetStats.Team)) return;

        OnTakeDamages(amount, isCrit);
    }

    public void OnHeal(float amount, bool isCrit = false)
    {
        if (isCrit) amount *= 1.5f;

        currentHP += amount;
        Debug.Log(currentHP);
    }

    protected void OnDeath()
    {
        Debug.Log(this.gameObject.name + " is dead lol", this.gameObject);
        source.PlayOneShot(audioClips.GetRandomDeathClip());
    }

    public bool RollCrit() => Random.Range(0, 100) >= GetStats.CritChances ? true : false;
}

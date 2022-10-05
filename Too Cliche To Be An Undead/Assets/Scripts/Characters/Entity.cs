using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    public Rigidbody2D GetRb { get => rb; }

    [SerializeField] protected SpriteRenderer sprite;
    public SpriteRenderer GetSprite { get => sprite; }

    [SerializeField] protected Material hitMaterial;
    public Material GetHitMaterial { get => hitMaterial; }

    [SerializeField] protected Animator animator;
    public Animator GetAnimator { get => animator; }

    [SerializeField] protected SCRPT_EntityStats stats;
    public SCRPT_EntityStats GetStats { get => stats; }

    [SerializeField] [ReadOnly] protected float currentHP;
    public float CurrentHP { get => currentHP; }



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

        if (currentHP <= 0) OnDeath();
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
    }

    protected void OnDeath()
    {
        Debug.Log(this.gameObject.name + " is dead lol", this.gameObject);
    }

    public bool RollCrit() => Random.Range(0, 100) >= GetStats.CritChances ? true : false;
}

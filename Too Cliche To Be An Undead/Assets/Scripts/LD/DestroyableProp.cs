using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableProp : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject destroyParticles;

    [SerializeField] private GameObject damagesParticles;

    [SerializeField] private float currentHP = 50;

    private void Start()
    {
        if (damagesParticles == null) damagesParticles = GameAssets.Instance.BasePropDamagesParticlesPF;
        if (destroyParticles == null) destroyParticles = GameAssets.Instance.BaseDestructionParticlesPF;
        if (damagesParticles == null) damagesParticles = GameAssets.Instance.BasePropDamagesParticlesPF;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        if (player == null) return;

        if (player.StateManager.CurrentState.ToString() != "Dashing") return;

        OnTakeDamages(currentHP);
    }

    public void DestroyObject()
    {
    }

    public bool OnTakeDamages(float amount, bool isCrit = false, bool fakeDamages = false, bool callDelegate = true)
    {
        currentHP -= amount;

        if (damagesParticles != null)
            damagesParticles.Create(this.transform.position);

        if (currentHP <= 0) OnDeath();
        return true;
    }

    public bool OnTakeDamages(float amount, SCRPT_EntityStats.E_Team damagerTeam, bool isCrit = false, bool fakeDamages = false)
    {
        return OnTakeDamages(amount, isCrit, fakeDamages);
    }

    public bool OnTakeDamages(float amount, SCRPT_EntityStats.E_Team damagerTeam, Entity damager, bool isCrit = false)
    {
        return OnTakeDamages(amount, isCrit);
    }

    public void OnHeal(float amount, bool isCrit = false, bool canExceedMaxHP = false)
    {
        currentHP += amount;
    }

    public void OnDeath(bool forceDeath = false)
    {
        if (destroyParticles != null)
            destroyParticles.Create(this.transform.position);

        Destroy(this.gameObject);
    }

    public bool IsAlive() => currentHP > 0;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableProp : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject destroyParticles;

    [SerializeField] private GameObject damagesParticles;

    [SerializeField] private AudioSource source;
    [SerializeField] private SCRPT_DestroyablePropsAudio audioData;

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

        OnTakeDamages(currentHP, null);
    }

    public void DestroyObject()
    {
    }

    public bool OnTakeDamages(float amount, Entity damager, bool isCrit = false, bool fakeDamages = false, bool callDelegate = true, bool tickDamages = false)
    {
        currentHP -= amount;

        if (damagesParticles != null)
            damagesParticles.Create(this.transform.position);

        if (currentHP <= 0) OnDeath();
        else
        {
            if (audioData != null)
                if (audioData.HurtClips.Length > 0) source.PlayOneShot(audioData.HurtClips.RandomElement());
        }
        return true;
    }

    public void OnHeal(float amount, bool isCrit = false, bool canExceedMaxHP = false)
    {
        currentHP += amount;
    }

    public void OnDeath(bool forceDeath = false)
    {
        if (destroyParticles != null)
            destroyParticles.Create(this.transform.position);

        if (audioData != null)
            if (audioData.DeathClips.Length > 0) AudioclipPlayer.Create(this.transform.position, audioData.DeathClips.RandomElement());
        Destroy(this.gameObject);
    }

    public bool IsAlive() => currentHP > 0;
}

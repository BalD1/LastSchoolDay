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

    [SerializeField] private float collisionDamagesTimer = .5f;
    [SerializeField] private float collisionDamages = 5;

    [SerializeField] private float audioTimer_DURATION = .25f;
    private float audioTimer;

    private List<NormalZombie> zombiesInCollider = new List<NormalZombie>();
    private List<float> zombiesDamagesTimer = new List<float>();

    private void Start()
    {
        if (damagesParticles == null) damagesParticles = GameAssets.Instance.BasePropDamagesParticlesPF;
        if (destroyParticles == null) destroyParticles = GameAssets.Instance.BaseDestructionParticlesPF;

        audioTimer = audioTimer_DURATION;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        if (player != null)
        {
            OnPlayerEnter(player);
            return;
        }

        NormalZombie nz = collision.gameObject.GetComponent<NormalZombie>();
        if (nz != null)
        {
            OnZombieEnter(nz);
            return;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        NormalZombie nz = collision.gameObject.GetComponent<NormalZombie>();
        if (nz != null)
        {
            OnZombieExit(nz);
            return;
        }
    }

    private void OnPlayerEnter(PlayerCharacter player)
    {
        if (player.StateManager.CurrentState.ToString() != "Dashing") return;
        OnTakeDamages(currentHP, null);
    }

    private void OnZombieEnter(NormalZombie zombie)
    {
        zombiesInCollider.Add(zombie);
        zombiesDamagesTimer.Add(collisionDamagesTimer);
    }

    private void OnZombieExit(NormalZombie zombie)
    {
        int zombieIdx = zombiesInCollider.IndexOf(zombie);
        zombiesInCollider.RemoveAt(zombieIdx);
        zombiesDamagesTimer.RemoveAt(zombieIdx);
    }

    private void Update()
    {
        for (int i = 0; i < zombiesInCollider.Count; i++)
        {
            zombiesDamagesTimer[i] -= Time.deltaTime;
            if (zombiesDamagesTimer[i] <= 0)
            {
                zombiesDamagesTimer[i] = collisionDamagesTimer;
                OnTakeDamages(collisionDamages, zombiesInCollider[i]);
            }
        }

        if (audioTimer > 0) audioTimer -= Time.deltaTime;
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
            if (audioData != null && audioTimer <= 0)
                if (audioData.HurtClips.Length > 0) source.PlayOneShot(audioData.HurtClips.RandomElement());
        }
        return true;
    }

    public void OnHeal(float amount, bool isCrit = false, bool canExceedMaxHP = false, bool healFromDeath = false)
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

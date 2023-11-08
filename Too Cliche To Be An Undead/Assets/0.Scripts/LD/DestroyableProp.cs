using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableProp : MonoBehaviour, IDamageable, IDistanceChecker
{
    [SerializeField] private GameObject destroyParticles;

    [SerializeField] private GameObject damagesParticles;

    [SerializeField] private AudioSource source;
    [SerializeField] private SCRPT_DestroyablePropsAudio audioData;

    [SerializeField] private float currentHP = 50;

    [SerializeField] protected float collisionDamagesTimer = .5f;
    [SerializeField] protected float collisionDamages = 5;

    [SerializeField] private float audioTimer_DURATION = .25f;
    protected float audioTimer;

    [SerializeField] private int maxDamagesAudio = 3;
    private int playingDamagesAudio = 0;

    protected bool isValid = false;

    private void Start()
    {
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isValid) return;
        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        if (player != null)
        {
            OnPlayerEnter(player);
            return;
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (!isValid) return;

    }

    private void OnPlayerEnter(PlayerCharacter player)
    {
        if (player.StateManager.CurrentState.ToString() != "Dashing") return;
        InflictDamages(currentHP, null);
    }

    public void DestroyObject()
    {
    }

    public bool InflictDamages(float amount, Entity damager, bool isCrit = false, bool fakeDamages = false, bool callDelegate = true, bool tickDamages = false)
    {
        currentHP -= amount;

        Vector3 particlesPos = this.transform.position;
        if (damager != null)
        {
            Vector3 effectObjectPos = damager.gameObject.transform.position;
            float dist = Vector2.Distance(effectObjectPos, this.transform.position) / 2;
            Vector2 dir = (this.transform.position - effectObjectPos).normalized;
            particlesPos = effectObjectPos + (Vector3)(dist * dir);
        }

        if (damagesParticles != null)
            damagesParticles.Create(particlesPos);

        if (currentHP <= 0)
        {
            Death();
            return true;
        }
        if (audioData == null) return true;
        if (audioTimer > 0) return true;
        if (audioData.HurtClips.Length == 0) return true;
        if (playingDamagesAudio > maxDamagesAudio) return true;

        playingDamagesAudio++;
        AudioClip clipToPlay = audioData.HurtClips.RandomElement();
        LeanTween.delayedCall(clipToPlay.length, () => playingDamagesAudio--);
        source.PlayOneShot(clipToPlay);
        return true;
    }

    public void Heal(float amount, bool isCrit = false, bool canExceedMaxHP = false, bool healFromDeath = false)
    {
        currentHP += amount;
    }

    public void Death(bool forceDeath = false)
    {
        if (destroyParticles != null)
            destroyParticles.Create(this.transform.position);

        if (audioData != null)
            if (audioData.DeathClips.Length > 0) AudioclipPlayer.Create(this.transform.position, audioData.DeathClips.RandomElement());
        Destroy(this.gameObject);
    }

    public bool IsAlive() => currentHP > 0;

    public void OnEnteredFarCheck() => isValid = true;
    public void OnExitedFarCheck() => isValid = false;

    public void OnEnteredCloseCheck()
    {
    }
    public void OnExitedCloseCheck()
    {
    }
}

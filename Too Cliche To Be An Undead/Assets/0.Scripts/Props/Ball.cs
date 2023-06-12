using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour, IDamageable
{
    [SerializeField] private float speedMultiplier;
    [SerializeField] private Rigidbody2D body;

    [SerializeField] private AudioSource source;
    [SerializeField] private SCRPT_DestroyablePropsAudio audioData;

    [SerializeField] private float velocityMagnitudeThresholdForAudio = .5f;
    [SerializeField] private float velocityMagnitudeThresholdForDamages = .75f;
    [SerializeField] private float damagesMultiplierOnZombies = 2;
    [SerializeField] private float damagesMultiplierOnPlayers = .25f;
    [SerializeField] private float maxDamagesOnPlayer = 15;

    private bool launched = false;

    public bool IsAlive() => true;

    private void Update()
    {
        if (!launched) return;

        if (body.velocity.magnitude < velocityMagnitudeThresholdForDamages) launched = false;
    }

    public void OnDeath(bool forceDeath = false)
    {
    }

    public void OnHeal(float amount, bool isCrit = false, bool canExceedMaxHP = false, bool healFromDeath = false)
    {
    }

    public bool OnTakeDamages(float amount, Entity damager, bool isCrit = false, bool fakeDamages = false, bool callDelegate = true, bool tickDamages = false)
    {
        Vector2 dir = this.transform.position - damager.transform.position;
        body.AddForce(dir * amount * speedMultiplier, ForceMode2D.Impulse);
        PlayAudio();

        launched = true;

        return true;
    }

    private void PlayAudio()
    {
        source.pitch = Random.Range(.9f, 1.1f);
        source.PlayOneShot(audioData.HurtClips.RandomElement());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (body.velocity.magnitude >= velocityMagnitudeThresholdForAudio)
            PlayAudio();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (launched && body.velocity.magnitude >= velocityMagnitudeThresholdForDamages)
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable == null) return;

            float finalDamages = body.velocity.magnitude * (damageable is PlayerCharacter ? damagesMultiplierOnPlayers : damagesMultiplierOnZombies);

            damageable.OnTakeDamages(finalDamages, null);
        }
    }
}

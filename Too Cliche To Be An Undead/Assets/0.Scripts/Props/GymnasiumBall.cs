using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GymnasiumBall : MonoBehaviour, IDamageable
{
    [SerializeField] private Rigidbody2D body;

    [SerializeField] private float speedMultiplier = .25f;

    public delegate void D_OnTakeDamages();
    public D_OnTakeDamages D_onTakeDamages;

    private Vector2 dir;

    public bool IsAlive() => true;

    public void Death(bool forceDeath = false)
    {
    }

    public void Heal(float amount, bool isCrit = false, bool canExceedMaxHP = false, bool healFromDeath = false)
    {
    }

    public bool InflinctDamages(float amount, Entity damager, bool isCrit = false, bool fakeDamages = false, bool callDelegate = true, bool tickDamages = false)
    {
        if (dir == Vector2.zero) return false;

        if (callDelegate) D_onTakeDamages?.Invoke();

        body.AddForce(dir * amount * speedMultiplier, ForceMode2D.Impulse);

        return true;
    }

    private void Reset()
    {
        body = this.GetComponent<Rigidbody2D>();
    }
}

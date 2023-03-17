using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour, IDamageable
{
    [SerializeField] private float speedMultiplier;
    [SerializeField] private Rigidbody2D body;

    public bool IsAlive() => true;

    public void OnDeath(bool forceDeath = false)
    {
    }

    public void OnHeal(float amount, bool isCrit = false, bool canExceedMaxHP = false)
    {
    }

    public bool OnTakeDamages(float amount, bool isCrit = false, bool fakeDamages = false, bool callDelegate = true)
    {
        return false;
    }

    public bool OnTakeDamages(float amount, SCRPT_EntityStats.E_Team damagerTeam, bool isCrit = false, bool fakeDamages = false)
    {
        return false;
    }

    public bool OnTakeDamages(float amount, SCRPT_EntityStats.E_Team damagerTeam, Entity damager, bool isCrit = false)
    {
        Vector2 dir = this.transform.position - damager.transform.position;

        body.AddForce(dir * amount * speedMultiplier, ForceMode2D.Impulse); 

        return true;
    }
}

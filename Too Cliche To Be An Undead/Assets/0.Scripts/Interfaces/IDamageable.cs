using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public abstract bool InflinctDamages(float amount, Entity damager, bool isCrit = false, bool fakeDamages = false, bool callDelegate = true, bool tickDamages = false);

    public abstract void Heal(float amount, bool isCrit = false, bool canExceedMaxHP = false, bool healfromDeath = false);

    public abstract void Death(bool forceDeath = false);

    public bool IsAlive();
}

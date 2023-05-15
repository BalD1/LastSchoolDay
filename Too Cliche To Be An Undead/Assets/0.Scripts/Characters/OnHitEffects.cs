using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitEffects
{
    private Entity owner;

    private string id;

    private float rangeModifier;
    private float damagesModifier;
    private float knockbackModifier;
    private float cameraShakeIntensityModifier;

    private TickDamages tickDamagesToAdd;

    private int leftHitsCount;
    private int maxHitsCount;

    private float lifetime_DURATION;
    private float lifetime_TIMER;

    private bool isBigHit;

    public float RangeModifier { get => rangeModifier; }
    public float DamagesModifier { get => damagesModifier; }
    public float KnockbackModifier { get => knockbackModifier; }
    public float CameraShakeIntensityModifier { get => cameraShakeIntensityModifier; }

    public bool IsBigHit { get => isBigHit; }

    public TickDamages TickDamagesToAdd { get => tickDamagesToAdd; }

    public OnHitEffects(Entity _owner, string _id, float _rangeModifier, float _damagesModifier, float _knockbackModifier, float _cameraShakeIntensityModifier, TickDamages _tickDamagesToAdd, int _leftHitsCount, float _lifetime, bool _isBigHit)
    {
        this.owner = _owner;

        this.id = _id;

        this.rangeModifier = _rangeModifier;
        this.damagesModifier = _damagesModifier;
        this.knockbackModifier = _knockbackModifier;
        this.cameraShakeIntensityModifier = _cameraShakeIntensityModifier;

        this.tickDamagesToAdd = _tickDamagesToAdd;

        this.leftHitsCount = this.maxHitsCount = _leftHitsCount;
        this.lifetime_DURATION = this.lifetime_TIMER = _lifetime;

        this.isBigHit = _isBigHit;
    }

    public void Update(float time)
    {
        lifetime_TIMER -= time;
    }

    public void SuccessfulyApplied() => leftHitsCount--;

    public bool IsFInished()
    {
        if (lifetime_DURATION > 0 && lifetime_TIMER <= 0) return true;
        if (maxHitsCount > 0 && leftHitsCount <= 0) return true;

        return false;
    }
}

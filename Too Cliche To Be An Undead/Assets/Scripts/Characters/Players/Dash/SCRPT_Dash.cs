using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCRPT_Dash : ScriptableObject
{
    [SerializeField] private AnimationCurve dashSpeedCurve;
    public AnimationCurve DashSpeedCurve { get => dashSpeedCurve; }

    [SerializeField] private float dash_COOLDOWN;
    public float Dash_COOLDOWN { get => dash_COOLDOWN; }

    [SerializeField] private float pushForce;
    public float PushForce { get => pushForce; }

    [SerializeField] private float maxScreenShakeIntensity = 1;
    public float MaxScreenShakeIntensity { get => maxScreenShakeIntensity; }

    [SerializeField] private float maxScreenShakeDuration = .1f;
    public float MaxScreenShakeDuration { get => maxScreenShakeDuration; }

    [SerializeField] protected GameObject particlesPF;

    protected ParticleSystem currentParticles;

    public virtual void OnDashStart(PlayerCharacter owner)
    {
        currentParticles = particlesPF?.Create(parent: owner.SkeletonAnimation.transform).GetComponent<ParticleSystem>();

    }
    public virtual void OnDashUpdate(PlayerCharacter owner)
    {

    }
    public virtual void OnDashStop(PlayerCharacter owner)
    {
        currentParticles.Stop();
    }
}

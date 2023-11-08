using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCRPT_Dash : ScriptableObject
{
    [field: SerializeField] public Sprite Thumbnail { get; private set; }

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

    [SerializeField] protected GameObject[] particlesPF;
    [field: SerializeField] public GameObject OnHitParticlesPF;

    protected List<ParticleSystem> currentParticles;

    public virtual void OnDashStart(PlayerCharacter owner, Vector2 direction)
    {
        currentParticles = new List<ParticleSystem>();

        float particlesRotation = 0;

        if (direction.y <= -.65f)
            particlesRotation = -90;
        else if (direction.y >= .65f)
            particlesRotation = 90;

        for (int i = 0; i < particlesPF.Length; i++)
        {
            //currentParticles.Add(particlesPF[i].Create(parent: owner.SkeletonAnimation.transform).GetComponent<ParticleSystem>());
            currentParticles[i].gameObject.transform.Rotate(particlesRotation, 0, 0);
        }
    }
    public virtual void OnDashUpdate(PlayerCharacter owner)
    {

    }
    public virtual void OnDashStop(PlayerCharacter owner)
    {
        foreach (var item in currentParticles)
            item.Stop();
    }
}

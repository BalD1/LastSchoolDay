using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SCRPT_Dash : ScriptableObject
{
    [SerializeField] private AnimationCurve dashSpeedCurve;
    public AnimationCurve DashSpeedCurve { get => dashSpeedCurve; }

    [SerializeField] private Sprite thumbnail;
    public Sprite Thumbnail { get => thumbnail;}

    [SerializeField] private float dash_COOLDOWN;
    public float Dash_COOLDOWN { get => dash_COOLDOWN; }

    [SerializeField] private float pushForce;
    public float PushForce { get => pushForce; }

    [SerializeField] private float maxScreenShakeIntensity = 1;
    public float MaxScreenShakeIntensity { get => maxScreenShakeIntensity; }

    [SerializeField] private float maxScreenShakeDuration = .1f;
    public float MaxScreenShakeDuration { get => maxScreenShakeDuration; }

    public abstract void OnDashStart(PlayerCharacter owner);
    public abstract void OnDashUpdate(PlayerCharacter owner);
    public abstract void OnDashStop(PlayerCharacter owner);
}

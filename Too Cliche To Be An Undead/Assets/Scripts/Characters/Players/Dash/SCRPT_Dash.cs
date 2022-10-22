using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SCRPT_Dash : ScriptableObject
{
    [SerializeField] private AnimationCurve dashSpeedCurve;
    public AnimationCurve DashSpeedCurve { get => dashSpeedCurve; }

    [SerializeField] private float pushForce;
    public float PushForce { get => pushForce; }

    public abstract void OnDashStart(PlayerCharacter owner);
    public abstract void OnDashUpdate(PlayerCharacter owner);
    public abstract void OnDashStop(PlayerCharacter owner);
}

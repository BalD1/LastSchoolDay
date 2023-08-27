using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CA_CinematicCameraShake : CA_CinematicAction
{
    [SerializeField] private float intensity;
    [SerializeField] private float duration;

    public CA_CinematicCameraShake(float intensity, float duration)
    {
        this.intensity = intensity;
        this.duration = duration;
    }

    public override void Execute()
    {
        CameraManager.Instance.ShakeCamera(intensity, duration);
        LeanTween.delayedCall(duration, () => ActionEnded(this)).setIgnoreTimeScale(true);
    }
}

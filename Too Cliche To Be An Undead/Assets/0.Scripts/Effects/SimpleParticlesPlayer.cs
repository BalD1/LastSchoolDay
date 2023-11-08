using System;
using UnityEngine;

public class SimpleParticlesPlayer : MonoBehaviour, IEffectPlayer
{
    [field: SerializeField] public bool DestroyOnEnd {  get; private set; }
    [field: SerializeField] public ParticleSystem Particles { get; private set; }


    public event Action OnParticlesEnded;

    public void OnParticleSystemStopped()
    {
        OnParticlesEnded?.Invoke();
        Destroy(this.gameObject);
    }

    public void AttachTo(Transform parent)
    {
        this.transform.parent = parent;
    }

    public void PlayAt(Vector2 position, Quaternion rotation)
    {
        this.transform.position = position;
        this.transform.rotation = rotation;
        this.gameObject.SetActive(true);
        Particles.Play();
    }
}

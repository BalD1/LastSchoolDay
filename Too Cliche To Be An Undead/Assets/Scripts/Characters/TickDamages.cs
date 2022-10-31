using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickDamages
{
    private Entity owner;

    private string id;

    private float damages;

    private float timeBetweenDamages;
    private float lifetime_DURATION;

    private float damages_TIMER;
    private float lifetime_TIMER;

    public string ID { get => id; }
    public float Damages { get => damages; }
    public float TimeBetweenDamages { get => timeBetweenDamages; }
    public float Lifetime_DURATION { get => lifetime_DURATION; }
    public float Damages_TIMER { get => damages_TIMER; }
    public float Lifetime_TIMER { get => lifetime_TIMER; }

#if UNITY_EDITOR
    public bool showInEditor;
#endif

    public TickDamages(string _id, float _damages, float _timeBetweenDamages, float _lifetime, Entity _owner)
    {
        this.id = _id;

        this.damages = _damages;

        this.timeBetweenDamages = _timeBetweenDamages;
        this.damages_TIMER = _timeBetweenDamages;

        this.lifetime_DURATION = _lifetime;
        this.lifetime_TIMER = _lifetime;

        this.owner = _owner;
    }

    public void Update(float time)
    {
        lifetime_TIMER -= time;

        damages_TIMER -= time;

        if (damages_TIMER < 0) ApplyDamages();
    }

    private void ApplyDamages()
    {
        damages_TIMER = timeBetweenDamages;

        owner.OnTakeDamages(damages);
    }

    public bool IsFinished() => lifetime_TIMER <= 0;

    public void ResetLifetime() => lifetime_TIMER = lifetime_DURATION;
}
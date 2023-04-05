using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickDamages
{
    private Entity owner;

    private Entity origin;

    private string id;

    private int critChances;

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

    public TickDamages(string _id, float _damages, float _timeBetweenDamages, float _lifetime, Entity _owner, Entity _origin, bool damageInstantly = false, int _critChances = 0)
    {
        this.id = _id;

        this.damages = _damages;

        this.timeBetweenDamages = _timeBetweenDamages;
        this.damages_TIMER = damageInstantly ? 0 : _timeBetweenDamages;

        this.lifetime_DURATION = _lifetime;
        this.lifetime_TIMER = _lifetime;

        this.owner = _owner;

        this.origin = _origin;

        critChances = _critChances;
    }

    public void Update(float time)
    {
        lifetime_TIMER -= time;

        damages_TIMER -= time;

        if (damages_TIMER < 0) ApplyDamages();
    }

    public void ModifyDamages(float newDamages) => damages = newDamages;
    public void ResetTimer(float newTimer = -1) => lifetime_TIMER = lifetime_DURATION = newTimer > 0 ? newTimer : lifetime_DURATION;

    private void ApplyDamages()
    {
        damages_TIMER = timeBetweenDamages;

        bool isCrit = Random.Range(0, 100) <= critChances;
        owner.OnTakeDamages(damages, origin, isCrit, false, true, true);
    }

    public void ForceEnd(float delay = 0) => lifetime_TIMER = delay;

    public bool IsFinished() => lifetime_TIMER <= 0;
}

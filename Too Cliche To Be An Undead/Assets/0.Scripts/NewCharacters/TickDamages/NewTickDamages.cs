using System;
using UnityEngine;

[System.Serializable]
public class NewTickDamages : ITickable
{
    [field: SerializeField, ReadOnly] public SO_TickDamagesData Data { get; private set; }
    [SerializeField, ReadOnly] private HealthSystem handler;

    private int currentTicks;

    private NewEntity origin;

    public NewTickDamages(SO_TickDamagesData _data, HealthSystem _handler, NewEntity _origin)
    {
        this.Data = _data;

        this.handler = _handler;
        this.origin = _origin;

        TickManagerEvents.OnTick += OnTick;
    }

    public virtual void OnTick(int tick)
    {
        currentTicks++;
        if (currentTicks % Data.RequiredTicksToTrigger == 0)
        {
            ApplyDamages();
        }

        if (currentTicks >= Data.TicksLifetime) OnEnd();
    }

    protected virtual void ApplyDamages()
    {
        handler.InflictDamages(Data.Damages, RandomExtensions.PercentageChance(Data.CritChances));
    }

    public void KillTick()
        => OnEnd();

    private void OnEnd()
    {
        TickManagerEvents.OnTick -= OnTick;
        handler.RemoveTickDamage(this);
    }

    public float RemainingTimeInSeconds()
        => RemainingTicks() * TickManager.TICK_TIMER_MAX;

    public int RemainingTicks()
       => Data.TicksLifetime - currentTicks;
}
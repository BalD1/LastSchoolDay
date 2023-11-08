using System;
using UnityEngine;

[System.Serializable]
public class NewTickDamages : ITickable
{
    [field: SerializeField, ReadOnly] public SO_TickDamagesData Data { get; private set; }
    [SerializeField, ReadOnly] private HealthSystem handler;

    private INewDamageable.DamagesData damagesData;

    private int currentTicks;

    private Entity origin;

    public NewTickDamages(SO_TickDamagesData _data, HealthSystem _handler, Entity _origin)
    {
        this.Data = _data;

        this.handler = _handler;
        this.origin = _origin;

        TickManagerEvents.OnTick += OnTick;

        origin.HolderTryGetComponent(IComponentHolder.E_Component.StatsHandler, out StatsHandler stats);
        damagesData = new INewDamageable.DamagesData(stats.GetTeam(), Data.DamagesType, Data.Damages, false, origin);
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
        damagesData.SetIsCrit(RandomExtensions.PercentageChance(Data.CritChances));
        handler.InflictDamages(damagesData);
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
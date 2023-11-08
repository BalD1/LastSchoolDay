using UnityEngine;

public class NewStatsModifier : ITickable
{
    [field: SerializeField, ReadOnly] public SO_StatModifierData Data { get; private set; }
    [SerializeField, ReadOnly] private StatsHandler handler;

    private int currentTicks;

    public NewStatsModifier(SO_StatModifierData data, StatsHandler handler)
    {
        this.Data = data;
        this.handler = handler;

        handler.OnAskReset += ForceKill;

        if (Data.Temporary)
            TickManagerEvents.OnTick += OnTick;
    }

    public void Remove()
        => OnEnd();

    public void OnTick(int tick)
    {
        currentTicks++;

        if (currentTicks >= Data.TicksLifetime) OnEnd();
    }

    protected void OnEnd()
    {
        if (Data.Temporary)
            TickManagerEvents.OnTick -= OnTick;
        handler.OnAskReset -= ForceKill;
        handler.RemoveStatModifier(this);
    }

    public void ForceKill()
        => OnEnd();

    public int RemainingTicks()
        => Data.TicksLifetime - currentTicks;

    public float RemainingTimeInSeconds()
        => RemainingTicks() * TickManager.TICK_TIMER_MAX;
}

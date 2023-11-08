using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSpiller : MonoBehaviourEventsHandler
{
    [SerializeField] protected IComponentHolder owner;

    [SerializeField] private int minBloodSpill = 2;
    [SerializeField] private int maxBloodSpill = 4;

    [SerializeField] private float onCritSpillMultiplier = 1.5f;

    [SerializeField] private float spillLifetime = 30;

    [SerializeField] private float onDamageDirectionRandomRange = 0.5f;

    protected override void EventsSubscriber()
    {
        if (owner.HolderTryGetComponent(IComponentHolder.E_Component.HealthSystem, out HealthSystem healthSystem) == IComponentHolder.E_Result.Success)
        {
            healthSystem.OnTookDamages += OnOwnerTookDamages;
            healthSystem.OnDeath += OnOwnerDeath;
        }
    }

    protected override void EventsUnSubscriber()
    {
        if (owner.HolderTryGetComponent(IComponentHolder.E_Component.HealthSystem, out HealthSystem healthSystem) == IComponentHolder.E_Result.Success)
        {
            healthSystem.OnTookDamages -= OnOwnerTookDamages;
            healthSystem.OnDeath -= OnOwnerDeath;
        }
    }

    private void OnOwnerTookDamages(INewDamageable.DamagesData damagesData)
    {
        SpillBloodOnDamages(damagesData.IsCrit, damagesData.Origin);
    }

    private void OnOwnerDeath()
    {

    }

    protected void SpillBloodOnDamages(bool isCrit, Entity damager, bool tickDamages = false)
    {
        if (damager == null) return;
        if (tickDamages) return;

        int amountToSpawn = Random.Range(minBloodSpill, maxBloodSpill);

        if (isCrit) amountToSpawn = Mathf.RoundToInt(amountToSpawn * onCritSpillMultiplier);

        Vector2 dir = (this.transform.position - damager.transform.position).normalized;

        SpillBlood(amountToSpawn, dir);
    }

    protected void SpillBloodOnDeath()
    {
        int amountToSpawn = Random.Range(minBloodSpill, maxBloodSpill);

        amountToSpawn *= 4;

        SpillBlood(amountToSpawn, Random.insideUnitCircle.normalized);
    }

    private void SpillBlood(int amount, Vector2 direction)
    {
        direction.x += Random.Range(-onDamageDirectionRandomRange, onDamageDirectionRandomRange);
        direction.y += Random.Range(-onDamageDirectionRandomRange, onDamageDirectionRandomRange);

        direction.Normalize();

        for (int i = 0; i < amount; i++)
        {
            BloodParticleSystemHandler.Instance.SpawnBlood(this.transform.position, direction, spillLifetime);
        }
    }
}

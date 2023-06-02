using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSpiller : MonoBehaviour
{
    [SerializeField] protected Entity owner;

    [SerializeField] private int minBloodSpill = 2;
    [SerializeField] private int maxBloodSpill = 4;

    [SerializeField] private float onCritSpillMultiplier = 1.5f;

    [SerializeField] private float spillLifetime = 30;

    [SerializeField] private float onDamageDirectionRandomRange = 0.5f;

    protected virtual void Awake()
    {
        owner.D_onTakeDamagesFromEntity += SpillBloodOnDamages;
        owner.d_OnDeath += SpillBloodOnDeath;
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

    private void OnDestroy()
    {
        owner.D_onTakeDamagesFromEntity -= SpillBloodOnDamages;
        owner.d_OnDeath -= SpillBloodOnDeath;
    }

    private void Reset()
    {
        Entity e = this.GetComponent<Entity>();

        if (e == null) e = this.GetComponentInParent<Entity>();

        owner = e;
    }
}

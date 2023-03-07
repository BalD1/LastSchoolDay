using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSpiller : MonoBehaviour
{
    [SerializeField] private Entity owner;

    [SerializeField] private int minBloodSpill;
    [SerializeField] private int maxBloodSpill;

    [SerializeField] private float onCritSpillMultiplier;

    [SerializeField] private float spillLifetime = 30;

    [SerializeField] private float onDamageDirectionRandomRange;

    private void Awake()
    {
        owner.D_onTakeDamagesFromEntity += SpillBloodOnDamages;
        owner.d_OnDeath += SpillBloodOnDeath;
    }

    private void SpillBloodOnDamages(bool isCrit, Entity damager)
    {
        int amountToSpawn = Random.Range(minBloodSpill, maxBloodSpill);

        if (isCrit) amountToSpawn = Mathf.RoundToInt(amountToSpawn * onCritSpillMultiplier);

        Vector2 dir = (this.transform.position - damager.transform.position).normalized;

        SpillBlood(amountToSpawn, dir);
    }

    private void SpillBloodOnDeath()
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

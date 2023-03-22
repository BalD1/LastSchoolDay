using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootprints : MonoBehaviour
{
    [SerializeField] private PlayerCharacter owner;

    [SerializeField] private float allowfootprints_DURATION = 2;
    private float allowfootprints_TIMER;

    [SerializeField] private float footprintsSpawn_COOLDOWN;
    private float footprintsSpawn_TIMER;

    [SerializeField] private float footprints_LIFETIME;

    [SerializeField] private float delayBetweenSteps = .1f;

    [SerializeField] private float yOffset;

    private bool leftPrint;

    private void Awake()
    {
        footprintsSpawn_TIMER = footprintsSpawn_COOLDOWN;

        owner.d_SteppedIntoTrigger += OwnerSteppedInTrigger;

        owner.D_successfulAttack += OwnerAttacked;
    }

    private void OwnerSteppedInTrigger(Type triggerType)
    {
        if (triggerType.Equals(typeof(BloodStamps)))
            allowfootprints_TIMER = allowfootprints_DURATION;
    }

    private void OwnerAttacked(bool isBigHit) => allowfootprints_TIMER = allowfootprints_DURATION;

    private void Update()
    {
        if (allowfootprints_TIMER <= 0) return;

        allowfootprints_TIMER -= Time.deltaTime;

        if (footprintsSpawn_TIMER > 0)
        {
            footprintsSpawn_TIMER -= Time.deltaTime;
            return;
        }

        if (owner.Velocity == Vector2.zero) return;

        SpawnFootprint();
        LeanTween.delayedCall(delayBetweenSteps, SpawnFootprint);

        footprintsSpawn_TIMER = footprintsSpawn_COOLDOWN;
    }

    private void SpawnFootprint()
    {
        Vector2 pos = this.transform.position;

        pos.y += leftPrint ? yOffset : -yOffset;
        leftPrint = !leftPrint;

        FootprintParticleSystemHandler.Instance.SpawnFootprint(pos, owner.LastDirection, footprints_LIFETIME);
    }
}

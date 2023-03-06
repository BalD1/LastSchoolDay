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

    private MeshParticleSystem meshParticleSystem;


    private void Awake()
    {
        footprintsSpawn_TIMER = footprintsSpawn_COOLDOWN;
        meshParticleSystem = this.GetComponent<MeshParticleSystem>();

        owner.d_SteppedIntoTrigger += OwnerSteppedInTrigger;
    }

    private void OwnerSteppedInTrigger(Type triggerType)
    {
        if (triggerType.Equals(typeof(BloodStamps)))
            allowfootprints_TIMER = allowfootprints_DURATION;
    }

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

        FootprintParticleSystemHandler.Instance.SpawnFootprint(this.transform.position, owner.LastDirection, footprints_LIFETIME);

        footprintsSpawn_TIMER = footprintsSpawn_COOLDOWN;
    }


}

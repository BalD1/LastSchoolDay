using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootprints : MonoBehaviour
{
    [SerializeField] private PlayerCharacter owner;

    [SerializeField] private float footprintsSpawn_COOLDOWN;
    private float footprintsSpawn_TIMER;

    [SerializeField] private float footprints_LIFETIME;

    private MeshParticleSystem meshParticleSystem;


    private void Awake()
    {
        footprintsSpawn_TIMER = footprintsSpawn_COOLDOWN;
        meshParticleSystem = this.GetComponent<MeshParticleSystem>();

        if (owner != null) return;

        owner = this.GetComponentInParent<PlayerCharacter>();

        if (owner == null) Destroy(this);
    }

    private void Update()
    {
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

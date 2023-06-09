using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YardZombiesSpawn : MonoBehaviour
{
    [SerializeField] private YardDoor[] yardDoors;
    [SerializeField] private ElementSpawner[] zombiesSpawners;

    private bool spawnFlag = false;

    private void Awake()
    {
        foreach (var door in yardDoors)
        {
            door.OnPlayerEnteredCollider += OnPlayerEnteredCollider;
        }
    }

    private void OnPlayerEnteredCollider()
    {
        if (spawnFlag) return;
        spawnFlag = true;

        foreach (var item in zombiesSpawners)
        {
            item.SpawnElement();
        }
    }
}

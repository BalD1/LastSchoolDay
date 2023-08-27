using System.Collections.Generic;
using UnityEngine;

public class RoomSpawners : MonoBehaviour
{
    [SerializeField] private ElementSpawner[] roomSpawners;

    [SerializeField] private AnimationCurve maxAllowedSpawnsPerStamp;

    [SerializeField] private BoxCollider2D trigger;

    private bool spawnFlag;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (SpawnersManager.Instance == null) return;
        if (spawnFlag) return;
        if (collision.GetComponent<PlayerCharacter>() == null) return;

        spawnFlag = true;

        List<ElementSpawner> spawners = new List<ElementSpawner>();
        foreach (var item in roomSpawners)
        {
            if (item.ElementToSpawn == ElementSpawner.E_ElementToSpawn.IdleZombie)
                spawners.Add(item);
        }

        float maxAllowedZombies = maxAllowedSpawnsPerStamp.Evaluate(SpawnersManager.Instance.SpawnStamp);
        while (spawners.Count > maxAllowedZombies) 
            spawners.RemoveAt(spawners.RandomIndex());

        foreach (var item in spawners) item.SpawnElement();

        this.EnteredRoomSpawner();

        Destroy(this);
    }
}

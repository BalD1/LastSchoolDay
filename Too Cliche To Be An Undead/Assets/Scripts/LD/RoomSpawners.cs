using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawners : MonoBehaviour
{
    [SerializeField] private ElementSpawner[] roomSpawners;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        if (collision.GetComponent<PlayerCharacter>() == null) return;

        foreach (var item in roomSpawners) item.SpawnElement();
    }
}

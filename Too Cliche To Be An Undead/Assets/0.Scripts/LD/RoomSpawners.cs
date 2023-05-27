using UnityEngine;

public class RoomSpawners : MonoBehaviour
{
    [SerializeField] private ElementSpawner[] roomSpawners;

    private bool spawnFlag;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (spawnFlag) return;
        if (collision.GetComponent<PlayerCharacter>() == null) return;

        spawnFlag = true;

        foreach (var item in roomSpawners) item.SpawnElement();

        SpawnersManager.Instance.ForceBreakup();

        Destroy(this);
    }
}

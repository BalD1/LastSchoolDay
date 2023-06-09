using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayersInRoom : MonoBehaviour
{
    private int playersInRoom;
    private int maxPlayers;

    private void Start()
    {
        maxPlayers = GameManager.Instance.playersByName.Count;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerCharacter>() == null) return;
        playersInRoom++;

        if (SpawnersManager.Instance == null) return;
        if (playersInRoom >= maxPlayers)
            SpawnersManager.Instance.OnAllPlayersInClassroom();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerCharacter>() == null) return;

        if (SpawnersManager.Instance == null) return;
        if (playersInRoom >= maxPlayers)
            SpawnersManager.Instance.PlayersExitedClassroom();

        playersInRoom--;
    }
}

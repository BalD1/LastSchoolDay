using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerCharacter>() == null) return;

        AreaTransitorManager.Instance.PlayerEnteredCorridor();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerCharacter>() == null) return;

        AreaTransitorManager.Instance.PlayerExitedCorridor();
    }
}

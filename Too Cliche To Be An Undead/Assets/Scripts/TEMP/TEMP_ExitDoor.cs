using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP_ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance.hasKey)
            {
                GameManager.Instance.GameState = GameManager.E_GameState.Win;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycard : MonoBehaviour
{
    private bool isPicked = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPicked) return;

        if (collision.CompareTag("Player"))
        {
            isPicked = true;
            GameManager.AcquiredCards += 1;
            UIManager.Instance.UpdateKeycardsCounter();
            Destroy(this.gameObject);
        }
    }
}

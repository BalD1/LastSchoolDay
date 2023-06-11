using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyTrigger : MonoBehaviour
{
    [SerializeField] private PlayerCharacter owner;

    private void Reset()
    {
        owner = this.GetComponentInParent<PlayerCharacter>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
        {
            owner.OnEnteredBodyTrigger?.Invoke(collision);
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
        {
            owner.OnExitedBodyTrigger?.Invoke(collision);
            return;
        }
    }
}

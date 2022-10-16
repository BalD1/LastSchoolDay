using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP_HubDoor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Vector2 playerTPPos;
    [SerializeField] private BoxCollider2D boxCollider2D;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            boxCollider2D.isTrigger = false;
            spriteRenderer.color = Color.red;
            collision.gameObject.transform.parent.position = playerTPPos;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(playerTPPos, 0.5f);
    }
}

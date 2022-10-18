using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] private EnemyBase owner;

    [SerializeField] private float detectionRange;

    [SerializeField] private CircleCollider2D detectionTrigger;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCharacter player = collision.GetComponentInParent<PlayerCharacter>();
            if (owner.DetectedPlayers.Contains(player) == false)
            {
                RaycastHit2D hit = Physics2D.Raycast(owner.transform.position, (owner.transform.forward - collision.transform.position).normalized, detectionRange);
                if (hit.collider.CompareTag("Player")) owner.DetectedPlayers.Add(player);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCharacter player = collision.GetComponentInParent<PlayerCharacter>();
            if (owner.DetectedPlayers.Contains(player) == false)
            {
                owner.lastSeenPosition = player.transform.position;
            }
        }
    }

    private void OnValidate()
    {
        if (detectionTrigger != null)
            detectionTrigger.radius = detectionRange;
    }

#if UNITY_EDITOR
    private bool debugMode;
#endif

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!debugMode) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(this.transform.position, detectionRange);
#endif
    }
}

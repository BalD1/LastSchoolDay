using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetsTrigger : MonoBehaviour
{
    [SerializeField] private BossZombie owner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCharacter player = collision.GetComponent<PlayerCharacter>();
        if (player == null) return;

        owner.D_entityEnteredCollider?.Invoke(player);
    }

    private void Reset()
    {
        owner = this.GetComponentInParent<BossZombie>();
    }
}

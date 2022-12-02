using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieLocker : MonoBehaviour
{
    [SerializeField] private int damagesAmount = 10;
    [SerializeField] private float stunDuration = 1.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCharacter player = collision.GetComponent<PlayerCharacter>();

        if (player == null) return;

        player.Stun(stunDuration);
        player.OnTakeDamages(damagesAmount);
    }
}

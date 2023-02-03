using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieLocker : MonoBehaviour
{
    [SerializeField] private int damagesAmount = 10;
    [SerializeField] private float stunDuration = 1.0f;
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerCharacter player = collision.gameObject.GetComponentInParent<PlayerCharacter>();

        if (player == null) return;
        if (player.StateManager.ToString() == "Pushed") return;

        player.Stun(stunDuration, false, true);
        player.OnTakeDamages(damagesAmount);
    }
}

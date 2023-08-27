using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YardDoorZombiesDetection : MonoBehaviour
{
    [SerializeField] private YardDoor owner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        NormalZombie nz = collision.GetComponentInParent<NormalZombie>();
        if (nz == null) return;

        nz.OnDeath += owner.OnZombieExitedDetecter;
        owner.OnZombieEnteredDetecter();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        NormalZombie nz = collision.GetComponentInParent<NormalZombie>();
        if (nz == null) return;

        nz.OnDeath -= owner.OnZombieExitedDetecter;
        owner.OnZombieExitedDetecter();
    }
}

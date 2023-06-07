using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DestroyableByZombieProps : DestroyableProp
{
    private List<NormalZombie> zombiesInCollider = new List<NormalZombie>();
    private List<float> zombiesDamagesTimer = new List<float>();

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        NormalZombie nz = collision.gameObject.GetComponent<NormalZombie>();
        if (nz != null)
        {
            OnZombieEnter(nz);
            return;
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        base.OnCollisionExit2D(collision);

        NormalZombie nz = collision.gameObject.GetComponent<NormalZombie>();
        if (nz != null)
        {
            OnZombieExit(nz);
            return;
        }
    }

    private void OnZombieEnter(NormalZombie zombie)
    {
        zombiesInCollider.Add(zombie);
        zombiesDamagesTimer.Add(0);
    }

    private void OnZombieExit(NormalZombie zombie)
    {
        int zombieIdx = zombiesInCollider.IndexOf(zombie);
        zombiesInCollider.RemoveAt(zombieIdx);
        zombiesDamagesTimer.RemoveAt(zombieIdx);
    }

    private void Update()
    {
        if (!isValid) return;
        if (zombiesInCollider.Count == 0) return;

        for (int i = 0; i < zombiesInCollider.Count; i++)
        {
            zombiesDamagesTimer[i] -= Time.deltaTime;
            if (zombiesDamagesTimer[i] <= 0)
            {
                zombiesDamagesTimer[i] = collisionDamagesTimer;
                OnTakeDamages(collisionDamages, zombiesInCollider[i]);
            }
        }

        if (audioTimer > 0) audioTimer -= Time.deltaTime;
    }
}

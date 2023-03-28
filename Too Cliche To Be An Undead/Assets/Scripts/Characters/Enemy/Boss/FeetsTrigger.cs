using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetsTrigger : MonoBehaviour
{
    [SerializeField] private BossZombie owner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        if (collision.GetComponent<PlayerCharacter>() == null) return;

        owner.d_EnteredCollider?.Invoke(null);
    }

    private void Reset()
    {
        owner = this.GetComponentInParent<BossZombie>();
    }
}

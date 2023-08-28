using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsTestLily : MonoBehaviour
{
    private Collider2D hitbox;

    private void Awake()
    {
        hitbox = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(this.gameObject);
    }
}
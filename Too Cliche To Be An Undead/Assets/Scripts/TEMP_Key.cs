using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP_Key : Collectable
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.hasKey = true;
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodStamps : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCharacter p = collision.GetComponent<PlayerCharacter>();
        if (p == null) return;

        p.d_SteppedIntoTrigger?.Invoke(typeof(BloodStamps));
    }
}

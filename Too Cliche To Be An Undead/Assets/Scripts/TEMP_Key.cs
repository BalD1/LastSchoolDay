using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP_Key : Collectable
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        GameManager.Instance.GameState = GameManager.E_GameState.Win;
    }
}

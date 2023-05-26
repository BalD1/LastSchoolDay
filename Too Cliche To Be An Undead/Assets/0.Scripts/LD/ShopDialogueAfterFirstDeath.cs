using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopDialogueAfterFirstDeath : DialogueTrigger
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (DataKeeper.Instance.runsCount != 2) return;
        base.OnTriggerEnter2D(collision);
    }
}

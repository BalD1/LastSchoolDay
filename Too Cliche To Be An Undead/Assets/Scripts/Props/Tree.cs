using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] private SpineColorModifier spineColor;

    private int playersBehindCount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.GetComponent<PlayerCharacter>() == null) return;

        playersBehindCount++;

        if (playersBehindCount <= 1)
            spineColor.SwitchToModifiedColor(.5f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.parent.GetComponent<PlayerCharacter>() == null) return;

        playersBehindCount--;

        if (playersBehindCount <= 0)
            spineColor.SwitchToBaseColor(.5f);
    }
}

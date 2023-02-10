using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomArea : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRoomHidder;

    [SerializeField] [ReadOnly] private int playersInRoomCount;

    public void AskForHidderAlphaChange(float newAlpha)
    {
        if (playersInRoomCount > 0) newAlpha = 0;

        Color targetRoomColor = targetRoomHidder.color;
        targetRoomColor.a = newAlpha;
        targetRoomHidder.color = targetRoomColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PlayerCharacter>() == null) return;

        playersInRoomCount++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PlayerCharacter>() == null) return;

        playersInRoomCount--;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransitor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRoomHidder;

    [SerializeField] private bool isVertical;

    private int roomTweenID;

    private int playersInRoom;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PlayerCharacter>() == null) return;

        AreaTransitorManager.PlayersInCorridorCount--;

        if (playersInRoom <= 0) SetRoomHiddenState(false);

        playersInRoom++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PlayerCharacter>() == null) return;

        AreaTransitorManager.PlayersInCorridorCount++;

        playersInRoom--;

        if (playersInRoom <= 0) SetRoomHiddenState(true);
    }

    public void SetRoomHiddenState(bool hidden)
    {
        LeanTween.cancel(roomTweenID);

        roomTweenID = LeanTween.value(targetRoomHidder.gameObject, targetRoomHidder.color, hidden ? AreaTransitorManager.Instance.hiddenColor : AreaTransitorManager.Instance.transparentColor, AreaTransitorManager.fadeTime).setOnUpdate(
        (Color val) =>
        {
            targetRoomHidder.color = val;
        }).id;
    }
}

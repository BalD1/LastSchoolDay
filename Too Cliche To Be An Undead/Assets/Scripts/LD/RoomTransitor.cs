using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class RoomTransitor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRoomHidder;

    [SerializeField] private RoomArea targetRoomArea;

    [SerializeField] private bool isVertical;
    [SerializeField] private bool reverseEntry;

    [SerializeField] private BoxCollider2D trigger;

    private List<Transform> playersInTrigger = new List<Transform>();

    private float closestInUpper;
    private float closestInLower;

    private float triggerSize;
    private float triggerHalfSize;

    private float minPosBetweenPlayers = float.MaxValue;
    private float maxPosBetweenPlayers = -1;

    private float selfPos;

    private int roomTweenID;

    private void Awake()
    {
        triggerSize = isVertical ? trigger.size.y : trigger.size.x;
        triggerHalfSize = triggerSize * .5f;

        selfPos = isVertical ? this.transform.position.y :
                             this.transform.position.x;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PlayerCharacter>() == null) return;

        playersInTrigger.Add(collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PlayerCharacter>() == null) return;

        playersInTrigger.Remove(collision.transform);
    }

    private void Update()
    {
        if (playersInTrigger.Count <= 0) return;

        float playerPos;
        float res = -1;

        minPosBetweenPlayers = float.MaxValue;
        maxPosBetweenPlayers = -1;

        bool isPlayerPosGreater = false;

        foreach (var item in playersInTrigger)
        {
            playerPos = isVertical ? item.transform.position.y :
                                     item.transform.position.x;

            isPlayerPosGreater = playerPos > selfPos;

            res = isPlayerPosGreater ? (playerPos - selfPos + triggerHalfSize) :
                                       (selfPos - playerPos + triggerHalfSize);

            res /= triggerSize;

            res = Mathf.Clamp01(res);

            if (res >= .9f) res = 1;
            if (reverseEntry) res = 1 - res;

            if (res < minPosBetweenPlayers) minPosBetweenPlayers = res;
            if (res > maxPosBetweenPlayers) maxPosBetweenPlayers = res;
        }

        if (isVertical) CheckVerticalDistance(isPlayerPosGreater, res);
        else CheckHorizontalDistance(isPlayerPosGreater, res);
    }

    private void CheckVerticalDistance(bool _isPlayerPosGreater, float _res)
    {
        targetRoomArea.AskForHidderAlphaChange(_isPlayerPosGreater ? _res : 1 - _res);

        AreaTransitorManager.Instance.AskForCorridorAlphaChange(_isPlayerPosGreater ? 1 - _res : _res);
    }

    private void CheckHorizontalDistance(bool _isPlayerPosGreater, float _res)
    {
        targetRoomArea.AskForHidderAlphaChange(_isPlayerPosGreater ? 1 - _res : _res);

        AreaTransitorManager.Instance.AskForCorridorAlphaChange(_isPlayerPosGreater ? _res : 1 - _res);
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

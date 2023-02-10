using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEditor.Progress;

public class RoomTransitor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRoomHidder;

    [SerializeField] private RoomArea targetRoomArea;

    [SerializeField] private bool isVertical;
    [SerializeField] private bool reverseEntry;

    [SerializeField] private BoxCollider2D trigger;

    [SerializeField] private List<Transform> playersInTrigger = new List<Transform>();

    private float triggerSize;
    private float triggerHalfSize;

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

        if (playersInTrigger.Count == 1) ProcessSoloPlayerInTrigger();
        else ProcessMultiplePlayersInTrigger();
    }

    private void ProcessSoloPlayerInTrigger()
    {
        float playerPos;
        float res = -1;

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
        }

        if (isVertical)
        {
            targetRoomArea.AskForHidderAlphaChange(isPlayerPosGreater ? res : 1 - res);

            AreaTransitorManager.Instance.AskForCorridorAlphaChange(isPlayerPosGreater ? 1 - res : res);
        }
        else
        {
            targetRoomArea.AskForHidderAlphaChange(isPlayerPosGreater ? 1 - res : res);

            AreaTransitorManager.Instance.AskForCorridorAlphaChange(isPlayerPosGreater ? res : 1 - res);
        }
    }

    private void ProcessMultiplePlayersInTrigger()
    {
        float playerPos = -1;
        float res = -1;
        bool isPlayerPosGreater = false;

        float highestDistanceInUpper = -1;
        float lowestDistanceInUpper = float.MaxValue;

        float highestDistanceInLower = -1;
        float lowestDistanceInLower = float.MaxValue;

        bool anyPlayerInUpper = false;
        bool anyPlayerInLower = false;

        foreach (var item in playersInTrigger)
        {
            playerPos = isVertical ? item.transform.position.y :
                                     item.transform.position.x;
            isPlayerPosGreater = playerPos > selfPos;

            res = isPlayerPosGreater ? (playerPos - selfPos + triggerHalfSize) :
                                       (selfPos - playerPos + triggerHalfSize);

            if (isPlayerPosGreater) anyPlayerInUpper = true;
            else anyPlayerInLower = true;
            res /= triggerSize;
            res = Mathf.Clamp01(res);

            if (res >= .9f) res = 1;

            if (isPlayerPosGreater)
            {
                if (res > highestDistanceInUpper) highestDistanceInUpper = res;
                if (res < lowestDistanceInUpper) lowestDistanceInUpper = res;
            }
            else
            {
                if (res > highestDistanceInLower) highestDistanceInLower = res;
                if (res < lowestDistanceInLower) lowestDistanceInLower = res;
            }
        }

        float lowerRes = anyPlayerInLower ? highestDistanceInLower : 1 - lowestDistanceInUpper;
        float higherRes = anyPlayerInUpper ? highestDistanceInUpper : 1 - lowestDistanceInLower;

        if (reverseEntry == false)
            targetRoomArea.AskForHidderAlphaChange(isVertical ? higherRes : 1 - higherRes);
        else 
            targetRoomArea.AskForHidderAlphaChange(isVertical ? lowerRes : 1 - lowerRes);


        if (reverseEntry == false)
            AreaTransitorManager.Instance.AskForCorridorAlphaChange(isVertical ? lowerRes : 1 - lowerRes);
        else
            AreaTransitorManager.Instance.AskForCorridorAlphaChange(isVertical ? higherRes : 1 - higherRes);
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

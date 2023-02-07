using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransitor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRoomHidder;

    [SerializeField] private bool isVertical;

    [SerializeField] private BoxCollider2D trigger;

    private List<Transform> playersInTrigger = new List<Transform>();

    private float closestInUpper;
    private float closestInLower;

    private Vector2 triggerSize;

    private int roomTweenID;

    private void Awake()
    {
        triggerSize = trigger.size * .5f;
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
        if (isVertical) CheckVerticalDistance();
        else CheckHorizontalDistance();
    }

    private void CheckVerticalDistance()
    {
        float playerPosY;
        float selfPosY;
        foreach (var item in playersInTrigger)
        {
            playerPosY = item.position.y;
            selfPosY = this.transform.position.y;

            if (playerPosY > selfPosY)
            {
                Debug.Log("n : " + (playerPosY - selfPosY) / triggerSize.y * 100);
                Debug.Log("-1 : " + (1-(playerPosY - selfPosY) / triggerSize.y * 100));
                Color c = targetRoomHidder.color;
                c.a = (playerPosY - selfPosY) / triggerSize.y;
                targetRoomHidder.color = c;

                Color d = AreaTransitorManager.Instance.c.color;
                d.a = 1-(playerPosY - selfPosY) / triggerSize.y;
                AreaTransitorManager.Instance.c.color = d;
            }
            else
            {
                Debug.Log(1-(selfPosY - playerPosY) / triggerSize.y * 100);
                Color c = targetRoomHidder.color;
                c.a = 1 - (selfPosY - playerPosY) / triggerSize.y;
                targetRoomHidder.color = c;

                Color d = AreaTransitorManager.Instance.c.color;
                d.a = 1-(selfPosY - playerPosY) / triggerSize.y;
                AreaTransitorManager.Instance.c.color = d;
            }
        }
    }

    private void CheckHorizontalDistance()
    {

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

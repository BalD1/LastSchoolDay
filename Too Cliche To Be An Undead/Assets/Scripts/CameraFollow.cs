using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private PlayerCharacter playerToFollow;

    void Start()
    {
        playerToFollow = GameManager.Player1Ref;
        this.transform.parent = playerToFollow.transform;
    }
}

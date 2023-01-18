using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YardDoor : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    [SerializeField] [SpineAnimation] private string openAnimation;

    [SerializeField] private BoxCollider2D blockCollision;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerCharacter>() == null) return;

        skeletonAnimation.AnimationState.SetAnimation(0, openAnimation, false);
        blockCollision.enabled = false;
        Destroy(this);
    }
}

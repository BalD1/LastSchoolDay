using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenableDoor : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeleton;

    [SerializeField][SpineAnimation] private string openAnimation;
    [SerializeField][SpineAnimation] private string closeAnimation;

    [SerializeField] private BoxCollider2D blocker;

    [SerializeField] private bool isClosed = false;

    private void Start()
    {
        if (isClosed) Close(true);
        else Open(true);
    }

    public void Close(bool force = false)
    {
        if (isClosed && !force) return;

        skeleton.AnimationState.SetAnimation(0, closeAnimation, false);
        blocker.gameObject.SetActive(true);

        isClosed = true;
    }

    public void Open(bool force = false)
    {
        if (!isClosed && !force) return;

        skeleton.AnimationState.SetAnimation(0, openAnimation, false);
        blocker.gameObject.SetActive(false);

        isClosed = false;
    }
}

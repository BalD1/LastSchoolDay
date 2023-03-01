using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField][SpineAnimation] private string anticip;
    [SerializeField][SpineAnimation] private string attack;
    [SerializeField][SpineAnimation] private string dash_side;
    [SerializeField][SpineAnimation] private string dash_up;
    [SerializeField][SpineAnimation] private string dash_down;
    [SerializeField][SpineAnimation] private string idle;
    [SerializeField][SpineAnimation] private string walk;
    [SerializeField][SpineAnimation] private string jumpstart;
    [SerializeField][SpineAnimation] private string jumpend;

    [SerializeField] private SkeletonAnimation skeleton;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) skeleton.AnimationState.SetAnimation(0, idle, true);
        if (Input.GetKeyDown(KeyCode.F2)) skeleton.AnimationState.SetAnimation(0, walk, true);
        if (Input.GetKeyDown(KeyCode.F3)) skeleton.AnimationState.SetAnimation(0, jumpstart, false);
        if (Input.GetKeyDown(KeyCode.F4)) skeleton.AnimationState.SetAnimation(0, jumpend, false);
    }
}

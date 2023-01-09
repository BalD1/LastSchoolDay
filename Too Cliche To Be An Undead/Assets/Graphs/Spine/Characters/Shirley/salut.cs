using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class salut : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    [SerializeField][SpineAnimation] private string idle, walk, dash, death, skill_trans, skill_idle, skill_walk;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5)) skeletonAnimation.AnimationState.SetAnimation(0, idle, true);
        if (Input.GetKeyDown(KeyCode.F6)) skeletonAnimation.AnimationState.SetAnimation(0, walk, true);
        if (Input.GetKeyDown(KeyCode.F7)) skeletonAnimation.AnimationState.SetAnimation(0, dash, true);
        if (Input.GetKeyDown(KeyCode.F8)) skeletonAnimation.AnimationState.SetAnimation(0, death, false);
        if (Input.GetKeyDown(KeyCode.F9)) skeletonAnimation.AnimationState.SetAnimation(0, skill_trans, false);
        if (Input.GetKeyDown(KeyCode.F10)) skeletonAnimation.AnimationState.SetAnimation(0, skill_idle, true);
        if (Input.GetKeyDown(KeyCode.F11)) skeletonAnimation.AnimationState.SetAnimation(0, skill_walk, true);

        if (Input.GetKeyDown(KeyCode.F12))
        {
            skeletonAnimation.AnimationState.SetAnimation(0, skill_trans, false);
            skeletonAnimation.AnimationState.AddAnimation(0, skill_idle, true, 0);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            skeletonAnimation.AnimationState.SetAnimation(0, skill_trans, false);
            skeletonAnimation.AnimationState.AddAnimation(0, skill_walk, true, 0);
        }
    }
}

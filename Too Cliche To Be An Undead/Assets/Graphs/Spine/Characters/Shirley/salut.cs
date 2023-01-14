using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class salut : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    [SerializeField][SpineAnimation] private string idle, walk, dash, death, skill_trans, skill_idle, skill_walk, attaque_cote, attaque_face, attaque_dos;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {
            skeletonAnimation.skeleton.SetToSetupPose();
            Vector2 scale = skeletonAnimation.transform.localScale;
            scale.x = 1;
            skeletonAnimation.transform.localScale = scale;
            skeletonAnimation.AnimationState.SetAnimation(0, idle, true);
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            skeletonAnimation.skeleton.SetToSetupPose();
            Vector2 scale = skeletonAnimation.transform.localScale;
            scale.x = 1;
            skeletonAnimation.transform.localScale = scale;
            skeletonAnimation.AnimationState.SetAnimation(0, attaque_cote, false);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            skeletonAnimation.skeleton.SetToSetupPose();
            Vector2 scale = skeletonAnimation.transform.localScale;
            scale.x = -1;
            skeletonAnimation.transform.localScale = scale;
            skeletonAnimation.AnimationState.SetAnimation(0, attaque_cote, false);
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            skeletonAnimation.skeleton.SetToSetupPose();
            skeletonAnimation.AnimationState.SetAnimation(0, attaque_face, false);
        }
        if (Input.GetKeyDown(KeyCode.F10)) skeletonAnimation.AnimationState.SetAnimation(0, attaque_dos, false);
    }
}

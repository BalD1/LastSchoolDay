using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class salut : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;

    [SpineAnimation] public string marche;
    [SpineAnimation] public string anti_cot;
    [SpineAnimation] public string att_cot;
    [SpineAnimation] public string anti_dos;
    [SpineAnimation] public string att_dos;
    [SpineAnimation] public string anti_face;
    [SpineAnimation] public string att_face;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5)) skeletonAnimation.AnimationState.SetAnimation(0, marche, true);
        if (Input.GetKeyDown(KeyCode.F6))
        {
            skeletonAnimation.AnimationState.SetAnimation(0, anti_cot, false);
            skeletonAnimation.AnimationState.AddAnimation(0, att_cot, false, .0f);
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            skeletonAnimation.AnimationState.SetAnimation(0, anti_dos, false);
            skeletonAnimation.AnimationState.AddAnimation(0, att_dos, false, .0f);
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            skeletonAnimation.AnimationState.SetAnimation(0, anti_face, false);
            skeletonAnimation.AnimationState.AddAnimation(0, att_face, false, .0f);
        }
    }
}

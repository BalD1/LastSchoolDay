using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineSkeletonPlayer : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    [SerializeField] private string animationToPlay;
    [InspectorButton(nameof(PlayAnimation), ButtonWidth = 200)]
    [SerializeField] private bool playAnimation;

    [Space]
    [SerializeField, SpineAnimation] private string animationToPlay2;
    [InspectorButton(nameof(PlayAnimation2), ButtonWidth = 200)]
    [SerializeField] private bool playAnimation2;

    [Space]
    [SerializeField] private AnimationReferenceAsset animationToPlay3;
    [InspectorButton(nameof(PlayAnimation3), ButtonWidth = 200)]
    [SerializeField] private bool playAnimation3;


    private void PlayAnimation()
    {
        if (skeletonAnimation == null) Debug.Log("Pas de squelette OMG");

        skeletonAnimation.AnimationState.SetAnimation(0, animationToPlay, true);
    }

    private void PlayAnimation2()
    {
        if (skeletonAnimation == null) Debug.Log("Pas de squelette OMG");
        skeletonAnimation.AnimationState.SetAnimation(0, animationToPlay2, true);
    }

    private void PlayAnimation3()
    {
        if (skeletonAnimation == null) Debug.Log("Pas de squelette OMG");
        skeletonAnimation.AnimationState.SetAnimation(0, animationToPlay3, true);
    }
}

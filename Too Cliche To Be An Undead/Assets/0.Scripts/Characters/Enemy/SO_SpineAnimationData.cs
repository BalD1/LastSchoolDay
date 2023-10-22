using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SpineAnimation Data", menuName = "Scriptable/Entity/Animations/AnimationData")]
public class SO_SpineAnimationData : ScriptableObject
{
    [SerializeField] private bool isDirectional;

    [SerializeField] private AnimationReferenceAsset sideAnim;

    [SerializeField] private AnimationReferenceAsset upAnim;
    [SerializeField] private AnimationReferenceAsset downAnim;

    [SerializeField] private float animDuration = -1;

    public AnimationReferenceAsset GetAnimation()
        => sideAnim;

    public AnimationReferenceAsset GetSideAnimation()
        => sideAnim;

    public AnimationReferenceAsset GetUpAnimation()
        => isDirectional ? upAnim : sideAnim;

    public AnimationReferenceAsset GetDownAnimation()
        => isDirectional ? downAnim : sideAnim;

    public float Duration()
    {
        if (animDuration < 0)
            animDuration = sideAnim.Animation.Duration;

        return animDuration;
    }
}
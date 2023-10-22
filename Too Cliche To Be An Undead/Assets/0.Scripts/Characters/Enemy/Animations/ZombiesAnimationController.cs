using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesAnimationController : AnimationControllerSingle
{
    [SerializeField] private NormalZombie owner;

    protected override void Setup()
    {
        //SetAnimation(owner.AnimationData., true);
    }
}

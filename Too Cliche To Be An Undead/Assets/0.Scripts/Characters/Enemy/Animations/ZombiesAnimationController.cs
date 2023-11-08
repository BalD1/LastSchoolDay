using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesAnimationController : AnimationControllerSingle
{
    [SerializeField] private BaseZombie owner;

    protected override void Setup()
    {
        //SetAnimation(owner.AnimationData., true);
    }
}

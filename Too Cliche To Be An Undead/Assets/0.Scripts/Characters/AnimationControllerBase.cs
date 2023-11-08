using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationControllerBase : MonoBehaviourEventsHandler
{
    [SerializeField] private GameObject ownerObj;
    protected IComponentHolder owner;

    protected virtual void Start()
    {
        owner = ownerObj.GetComponent<IComponentHolder>();
        Setup();
    }

    protected abstract void Setup();

    public abstract bool TryFlipSkeleton(bool lookAtRight);
    public abstract void FlipSkeleton();

    public abstract void SetAnimation(string animation, bool loop, float timeScale = 1);
    public abstract void SetAnimation(Spine.Animation animation, bool loop, float timeScale = 1);

    public abstract void AddAnimation(string animation, bool loop, float timeScale = 1);
    public abstract void AddAnimation(Spine.Animation animation, bool loop, float timeScale = 1);
    public abstract bool IsLookingAtRight();

    public abstract void SetAnimationSequence(S_AnimationSequenceSingle[] sequence, bool loopLast, float timeScale = 1);

    public abstract void SetSkeletonColor(Color color);
    public abstract void SetSkeletonAlpha(float alpha);

    [System.Serializable]
    public struct S_AnimationSequenceSingle
    {
        [field: SerializeField] public AnimationReferenceAsset Animation { get; private set; }
        [field: SerializeField] public float DelayBeforeNext { get; private set; }
        [field: SerializeField] public float TimescaleModifier { get; private set; }
    }
}

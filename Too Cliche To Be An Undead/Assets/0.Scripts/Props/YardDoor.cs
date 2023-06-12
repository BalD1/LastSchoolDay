using JetBrains.Annotations;
using Spine;
using Spine.Unity;
using System;
using UnityEngine;

public class YardDoor : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    [SerializeField] private AnimationReferenceAsset openAnimation;
    [SerializeField] private AnimationReferenceAsset animationWithNoZombies;
    [SerializeField] private AnimationReferenceAsset animationWithZombies;

    [SerializeField] private BoxCollider2D blockCollision;

    [SerializeField] private bool destroyScriptOnOpen = true;

    [SerializeField] private int entitiesInTrigger = 0;
    [SerializeField] private int zombiesInDetecter = 0;

    [SerializeField] private bool removeCollisionOnOpen = false;

    private LTDescr closeTween;

    public bool IsOpen { get; private set; }

    public event Action OnDoorOpen;
    public event Action OnPlayerEnteredCollider;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Entity entity = collision.GetComponent<Entity>();
        if (entity == null) return;

        if (entity is PlayerCharacter) OnPlayerEnteredCollider?.Invoke();

        entitiesInTrigger++;
        if (entitiesInTrigger > 1) return;

        OpenDoor();
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        Entity entity = collision.GetComponent<Entity>();
        if (entity == null) return;

        entitiesInTrigger--;
        if (entitiesInTrigger >= 1) return;

        CloseDoor();
    }

    public void OnZombieEnteredDetecter()
    {
        zombiesInDetecter++;
        if (zombiesInDetecter > 1) return;

        TrySetAnimationZombiesState(true);
    }

    public void OnZombieExitedDetecter()
    {
        zombiesInDetecter--;
        if (zombiesInDetecter >= 1) return;

        TrySetAnimationZombiesState(false);
    }

    public void TrySetAnimationZombiesState(bool withZombies)
    {
        if (!IsOpen)
            skeletonAnimation.AnimationState.SetAnimation(0, withZombies ? animationWithZombies : animationWithNoZombies, true);
    }

    public virtual void OpenDoor()
    {
        if (closeTween != null)
            LeanTween.cancel(closeTween.uniqueId);
        skeletonAnimation.AnimationState.SetAnimation(0, openAnimation, false);

        if (removeCollisionOnOpen && blockCollision != null)
            blockCollision.enabled = false;

        IsOpen = true;
    }

    public virtual void CloseDoor()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, openAnimation, false);

        if (closeTween != null)
            LeanTween.cancel(closeTween.uniqueId);
        closeTween = LeanTween.value(1f, 0f, openAnimation.Animation.Duration).setOnUpdate((float val) =>
        {
            TrackEntry currentMovementTrack = skeletonAnimation.AnimationState.Tracks.Items[0];
            currentMovementTrack.TimeScale = 0f; // so time isn't moved forward by SkeletonAnimation/AnimationState
            currentMovementTrack.AnimationLast = 0f; // this may cause multiple events to fire if you have those.
            currentMovementTrack.TrackTime = val; // you should decrement targetTime by a delta time (to act as the animation time cursor that moves backwards.)
            skeletonAnimation.state.Apply(skeletonAnimation.skeleton); // may be required, depending on project script execution order settings. It would be more efficient if this script runs before the SkeletonAnimation.cs and you won't need this.
        }).setOnComplete(() =>
        {
            TrySetAnimationZombiesState(zombiesInDetecter > 1);
        });

        if (removeCollisionOnOpen && blockCollision != null)
            blockCollision.enabled = true;

        IsOpen = false;
    }
}

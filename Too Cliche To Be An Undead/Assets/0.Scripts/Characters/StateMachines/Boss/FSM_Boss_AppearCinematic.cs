using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Boss_AppearCinematic : FSM_Base<FSM_Boss_Manager>
{
    private BossZombie owner;
    public Cinematic FallCinematic { get; private set; } = new Cinematic();
    private bool initialized = false;

    private float cameraShakeDuration = 2;

    public override void EnterState(FSM_Boss_Manager stateManager)
    {
        base.EnterState(stateManager);
        owner.SkeletonAnimation.AnimationState.SetAnimation(0, owner.animationData.JumpStartAnim, false);
        owner.SkeletonHolder.AddToLocalPositionY(5);
        Skeleton sk = owner.SkeletonAnimation.skeleton;

        if (!initialized)
        {
            initialized = true;
            FallCinematic = new Cinematic(
                    new CA_CinematicFadeSkeleton(1, .5f, sk),
                    new CA_CinematicActionMultiple(
                            new CA_CinematicMoveObject(owner.SkeletonHolder, Vector2.zero, .25f, true),
                            new CA_CinematicCustomAction(() =>
                            {
                                owner.animationController.SetAnimation(owner.animationData.JumpEndAnim, false);
                                owner.animationController.AddAnimation(owner.animationData.IdleAnim, true, owner.animationData.JumpEndAnim.Animation.Duration + .5f);
                            })
                        ),
                    new CA_CinematicActionMultiple(
                        new CA_CinematicCameraShake(5, cameraShakeDuration),
                        new CA_CinematicPlaySFX(owner.fall, owner.transform.position)
                        ),
                    new CA_CinematicWait(.1f),
                    new CA_CinematicActionMultiple(
                        new CA_CinematicCustomAction(() =>
                        {
                            owner.animationController.SetAnimation(owner.animationData.YellAnim, false);
                            LeanTween.delayedCall(10, () =>
                            {
                                owner.animationController.SetAnimation(owner.animationData.IdleAnim, true);
                            }).setIgnoreTimeScale(true);
                        }),
                        new CA_CinematicPlaySFX(owner.howl, owner.transform.position),
                        new CA_CinematicCameraZoom(0.5f, .5f)
                        ),
                    new CA_CinematicPlayMusic(SoundManager.E_MusicClipsTags.BossMusic)
                ).AllowChangeCinematicStateAtEnd(false);
            FallCinematic.OnCinematicEnded += () => owner.OnAppearAnimationEnded?.Invoke();
        }
        FallCinematic.StartCinematic();
    }


    public override void UpdateState(FSM_Boss_Manager stateManager)
    {
    }

    public override void FixedUpdateState(FSM_Boss_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Boss_Manager stateManager)
    {
    }

    public override void Conditions(FSM_Boss_Manager stateManager)
    {
    }

    protected override void EventsSubscriber(FSM_Boss_Manager stateManager)
    {
    }

    protected override void EventsUnsubscriber(FSM_Boss_Manager stateManager)
    {
    }

    public override void Setup(FSM_Boss_Manager stateManager)
    {
        owner = stateManager.Owner;
    }

    public override string ToString()
    {
        return "Dead";
    }
}

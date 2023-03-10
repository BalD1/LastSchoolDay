using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCinematic : MonoBehaviour, IInteractable
{
    [SerializeField] private BossZombie boss;

    [SerializeField] private SkeletonAnimation skeletonAnimation;

    [SerializeField] private AnimationReferenceAsset unlockedAnim;
    [SerializeField] private AnimationReferenceAsset openAnim;
    private float animDuration;

    [SerializeField]
    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    private string dialogueToPlay;

    private bool canBeOpened = false;

    private void Awake()
    {
        boss.d_OnDeath += SetCanBeOpened;

        animDuration = openAnim.Animation.Duration;
    }

    private void SetCanBeOpened()
    {
        UIManager.Instance.SetBlackBars(true);

        boss.animationController.SetAnimation(boss.animationData.DeathAnim, false);
        CameraManager.Instance.MoveCamera(boss.transform.position, () =>
        {
            LeanTween.delayedCall(1, () =>
            {
                DialogueManager.Instance.TryStartDialogue(dialogueToPlay);
            });
        }, .5f);

        canBeOpened = true;
        skeletonAnimation.AnimationState.SetAnimation(0, unlockedAnim, false);
    }

    public bool CanBeInteractedWith() => canBeOpened;

    public void EnteredInRange(GameObject interactor)
    {
    }

    public void ExitedRange(GameObject interactor)
    {
    }

    public float GetDistanceFrom(Transform target) => Vector2.Distance(this.transform.position, target.position);

    public void Interact(GameObject interactor)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, openAnim, false);

        LeanTween.delayedCall(animDuration, () =>
        {
            GameManager.Instance.GameState = GameManager.E_GameState.Win;
        });
    }
}

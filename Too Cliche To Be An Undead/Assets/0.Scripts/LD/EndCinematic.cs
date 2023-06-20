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
    private string onBossKillDialogue;

    [SerializeField]
    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    private string showExitDoorDialogue;

    private bool canBeOpened = false;

    private void Awake()
    {
        boss.d_OnDeath += SetCanBeOpened;

        animDuration = openAnim.Animation.Duration;
    }

    private void SetCanBeOpened()
    {
        /*
        UIManager.Instance.SetBlackBars(true);

        CameraManager.Instance.MoveCamera(boss.transform.position, () =>
        {
            LeanTween.delayedCall(1, () =>
            {
                DialogueManager.Instance.TryStartDialogue(onBossKillDialogue,
                    () => CameraManager.Instance.MoveCamera(this.transform.position, 
                        () => DialogueManager.Instance.TryStartDialogue(showExitDoorDialogue, true, () =>
                        {
                            CameraManager.Instance.MoveCamera(GameManager.Player1Ref.transform.position, StopCinematic);
                        }))
                    );
            });
        }, .5f);

        canBeOpened = true;
        skeletonAnimation.AnimationState.SetAnimation(0, unlockedAnim, false);
        */
    }

    private void StopCinematic()
    {
        CameraManager.Instance.EndCinematic();
        UIManager.Instance.SetBlackBars(false);
        GameManager.Instance.GameState = GameManager.E_GameState.InGame;
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
        if (!canBeOpened) return;

        skeletonAnimation.AnimationState.SetAnimation(0, openAnim, false);

        canBeOpened = false;

        LeanTween.delayedCall(animDuration, () =>
        {
            GameManager.Instance.GameState = GameManager.E_GameState.Win;
        });
    }
}

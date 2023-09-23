using Spine.Unity;
using System.Collections;
using UnityEngine;

public class EndCinematic : MonoBehaviour, IInteractable
{
    private BossZombie boss;

    [SerializeField] private SkeletonAnimation skeletonAnimation;

    [SerializeField] private AnimationReferenceAsset unlockedAnim;
    [SerializeField] private AnimationReferenceAsset openAnim;
    private float animDuration;

    [SerializeField] private SCRPT_SingleDialogue onBossKillDialogue;
    [SerializeField] private SCRPT_SingleDialogue showExitDoorDialogue;

    private Cinematic onBossKillCinematic;
    private bool cinematicInitialized;

    private bool canBeOpened = false;

    private void Awake()
    {
        animDuration = openAnim.Animation.Duration;
    }

    public void SetBoss(BossZombie newBoss)
    {
        if (this.boss != null) this.boss.OnDeath -= SetCanBeOpened;
        this.boss = newBoss;
        boss.OnDeath += DelayedSetCanBeOpened;

        if (!cinematicInitialized)
        {
            cinematicInitialized = true;
            onBossKillCinematic = new Cinematic(
                new CA_CinematicCameraMove(boss.transform),
                new CA_CinematicWait(.5f),
                new CA_CinematicDialoguePlayer(onBossKillDialogue),
                new CA_CinematicCameraMove(this.transform.position),
                new CA_CinematicWait(1),
                new CA_CinematicDialoguePlayer(showExitDoorDialogue)
                );
        }
    }

    private void DelayedSetCanBeOpened()
        => Invoke(nameof(SetCanBeOpened), 1);
    private void SetCanBeOpened()
    {
        canBeOpened = true;
        skeletonAnimation.AnimationState.SetAnimation(0, unlockedAnim, false);

        onBossKillCinematic.StartCinematic();
    }

    public bool CanBeInteractedWith() => canBeOpened;
    public void EnteredInRange(GameObject interactor) { }
    public void ExitedRange(GameObject interactor) { }
    public float GetDistanceFrom(Transform target) => Vector2.Distance(this.transform.position, target.position);

    public void Interact(GameObject interactor)
    {
        if (!canBeOpened) return;
        canBeOpened = false;
        StartCoroutine(DoorOpenCinematic());
    }

    private IEnumerator DoorOpenCinematic()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, openAnim, false);
        yield return new WaitForSeconds(animDuration);
        this.DoorOpened();
    }
}

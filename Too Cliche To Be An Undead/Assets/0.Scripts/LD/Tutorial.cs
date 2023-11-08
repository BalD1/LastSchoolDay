using System;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviourEventsHandler
{
    [InspectorButton(nameof(DelegateUpdateNmaesOnList), ButtonWidth = 300)]
    [SerializeField] private bool updateDialoguesNames;

    [SerializeField] private SCRPT_SingleDialogue zombiesDialogue;
    [SerializeField] private SCRPT_SingleDialogue cleanedRoomDialogue;
    [SerializeField] private SCRPT_SingleDialogue shopTutoDialogue;

    [SerializeField] private OpenableDoor doorToCloseOnZombies;
    [SerializeField] private BaseZombie[] tutorialZombies;

    [SerializeField] private List<Transform> onZombiesPlayersMovementGoals;
    [SerializeField] private Transform zombiesCinematicCameraGoal;

    [SerializeField] private CanvasGroup[] tutorialsArray;
    private Queue<CanvasGroup> tutorialsQueue;

    [SerializeField] private ReviveTutorial reviveTutorial;

    [SerializeField] private float tutorialImagesAnimScaleMultiplier = 2;

    private Cinematic zombiesFightCinematic;
    private Cinematic zombiesCleanedCinematic;

    private Cinematic shopTutoCinematic;
    private Cinematic shopClosedCinematic;

    private int zombiesCount;

    private bool enteredInTriggerFlag = false;

    private bool tutoFinished = false;

    private Shop currentShop;

    protected override void EventsSubscriber()
    {
        if (ShopEvents.CurrentShop != null) currentShop = ShopEvents.CurrentShop;
        else ShopEvents.OnShopSetup += OnShopSetup;

        ShopEvents.OnCloseShop += OnShopClosed;
    }

    protected override void EventsUnSubscriber()
    {
        ShopEvents.OnCloseShop -= OnShopClosed;
    }

    private void OnShopClosed()
    {
        ShopEvents.OnCloseShop -= OnShopClosed;
        shopClosedCinematic.StartCinematic();
    }

    private void OnShopSetup(Shop s)
    {
        currentShop = s;
        ShopEvents.OnShopSetup -= OnShopSetup;
    }

    protected override void Awake()
    {
        base.Awake();
        if (DataKeeper.Instance.skipTuto || DataKeeper.Instance.alreadyPlayedTuto)
        {
            Destroy(this.gameObject);
            Destroy(reviveTutorial.gameObject);
            return;
        }
        foreach (var item in tutorialZombies)
        {
            //item.OnDeath += OnZombieDeath;
        }

        zombiesCount = tutorialZombies.Length;
        foreach (var item in tutorialZombies)
        {
            item.gameObject.SetActive(false);
        }

        tutorialsQueue = new Queue<CanvasGroup>();
        foreach (var item in tutorialsArray)
        {
            item.alpha = 0;
            tutorialsQueue.Enqueue(item);
        }
    }

    private void Start()
    {
        this.TutorialStated();
        BuildZombiesFightCinematic();
        BuildShopTutoCinematic();
        BuildZombiesCleanedCinematic();
        BuildShopClosedCinematic();
    }

    private void BuildZombiesFightCinematic()
    {
        zombiesFightCinematic = new Cinematic().SetPlayers(IGPlayersManager.Instance.PlayersList);
        zombiesFightCinematic.AddActions(
            new CA_CinematicActionMultiple(
                            new CA_CinematicPlayersMove(onZombiesPlayersMovementGoals, true, false),
                            new CA_CinematicCameraMove(zombiesCinematicCameraGoal.position, 1.5f).SetLeanType(LeanTweenType.easeOutSine)
                ),

            new CA_CinematicCustomAction(doorToCloseOnZombies.Close),
            new CA_CinematicCustomAction(EnableTutorialZombies),
            new CA_CinematicDialoguePlayer(zombiesDialogue),
            new CA_CinematicCustomAction(() => UIManager.Instance.FadeTutoHUD(true)),
            new CA_CinematicCustomAction(() => AnimateNextTutorialMultiple(2)),
            new CA_CinematicWait(3)
            );
    }

    private void BuildZombiesCleanedCinematic()
    {
        zombiesCleanedCinematic = new Cinematic().SetPlayers(IGPlayersManager.Instance.PlayersList);
        zombiesCleanedCinematic.AddActions(
            new CA_CinematicDialoguePlayer(cleanedRoomDialogue),
            new CA_CinematicPlayMusic(SoundManager.E_MusicClipsTags.InLobby),
            new CA_CinematicScreenFade(false, .75f),
            new CA_CinematicCustomAction(() => currentShop.InteractionTrigger.enabled = false),
            new CA_CinematicPlayersMove(IGPlayersManager.Instance.igSpawnPoints, true, true),
            new CA_CinematicCameraMove(IGPlayersManager.Instance.PlayersList[0].transform, true),
            new CA_CinematicWait(1),
            new CA_CinematicScreenFade(true, .75f),
            new CA_CinematicCustomAction(() => this.TutorialEnded())
            ).SetChainCinematic(shopTutoCinematic);
    }

    private void BuildShopTutoCinematic()
    {
        shopTutoCinematic = new Cinematic().SetPlayers(IGPlayersManager.Instance.PlayersList);
        shopTutoCinematic.AddActions(
            new CA_CinematicCameraMove(Camera.main, currentShop.transform.position, .5f),
            new CA_CinematicDialoguePlayer(shopTutoDialogue),
            new CA_CinematicCustomAction(currentShop.OpenShop),
            new CA_CinematicCustomAction(() => currentShop.InteractionTrigger.enabled = true)
            ).AllowChangeCinematicStateAtEnd(false);
    }

    private void BuildShopClosedCinematic()
    {
        shopClosedCinematic = new Cinematic().SetPlayers(IGPlayersManager.Instance.PlayersList);
        shopClosedCinematic.AddActions(
            new CA_CinematicCameraMove(Camera.main, IGPlayersManager.Instance.PlayersList[0].transform, .5f)
            );
    }

    public void AnimateNextTutorial() => AnimateNextTutorial(null);
    public void AnimateNextTutorial(Action actionToPlayAtEnd)
    {
        if (tutorialsQueue.Count <= 0) return;

        CanvasGroup cg = tutorialsQueue.Dequeue();

        Transform cgTransform = cg.transform;
        Vector2 basePos = cgTransform.localPosition;

        cgTransform.localPosition = Vector2.zero;
        cgTransform.localScale *= tutorialImagesAnimScaleMultiplier;

        cg.LeanAlpha(1, .5f).setOnComplete(() =>
        {
            LeanTween.delayedCall(1, () =>
            {
                cgTransform.LeanScale(Vector2.one, 2).setEaseInOutQuart().setIgnoreTimeScale(true);
                cg.transform.LeanMoveLocal(basePos, 2).setEaseInOutQuart().setIgnoreTimeScale(true).setOnComplete(actionToPlayAtEnd);
            });
        });
    }

    public void AnimateNextTutorialMultiple(int count, Action actionToPlayAtEnd = null)
    {
        float x = 0;
        for (int i = 0; i < count; i++)
        {
            CanvasGroup cg = tutorialsQueue.Dequeue();

            Transform cgTransform = cg.transform;
            Vector2 basePos = cgTransform.localPosition;

            // Get the size of the element
            float transformSizeX = cg.GetComponent<RectTransform>().rect.width * tutorialImagesAnimScaleMultiplier;

            // Calculate the offset
            // Add to the left if at first half of loop
            // Add to the right if at 2nd half of loop
            x = i < count / 2 ? x - transformSizeX : x + transformSizeX;

            // Apply offset on x
            cgTransform.localPosition = new Vector2(
                x: x * (i + 1),
                y: 0);
            cgTransform.localScale *= tutorialImagesAnimScaleMultiplier;

            cg.LeanAlpha(1, .5f).setOnComplete(() =>
            {
                LeanTween.delayedCall(1, () =>
                {
                    cgTransform.LeanScale(Vector2.one, 2).setEaseInOutQuart().setIgnoreTimeScale(true);
                    cg.transform.LeanMoveLocal(basePos, 2).setEaseInOutQuart().setIgnoreTimeScale(true).setOnComplete(actionToPlayAtEnd);
                });
            });
        }
    }

    private void EnableTutorialZombies()
    {
        foreach (var item in tutorialZombies)
        {
            item.gameObject.SetActive(true);
            //item.Vision.TargetClosestPlayer();
        }
    }

    private void OnZombieDeath()
    {
        if (tutoFinished) return;
        zombiesCount--;
        if (zombiesCount <= 0)
        {
            tutoFinished = true;
            foreach (var item in tutorialZombies)
            {
                //item.OnDeath -= OnZombieDeath;
            }
            zombiesCleanedCinematic.StartCinematic();
        }
    }

    public void ForceEndTutorial() => zombiesCleanedCinematic.StartCinematic();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PlayerCharacter>() != null && enteredInTriggerFlag == false)
        {
            enteredInTriggerFlag = true;
            zombiesFightCinematic.StartCinematic();
        }
    }

    private void DelegateUpdateNmaesOnList()
    {
        DialogueManager.SearchAndUpdateDialogueList();
    }
}

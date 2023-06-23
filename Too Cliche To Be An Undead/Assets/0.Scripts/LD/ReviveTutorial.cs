using System.Collections.Generic;
using UnityEngine;

public class ReviveTutorial : MonoBehaviourEventsHandler
{
    [SerializeField] private SCRPT_SingleDialogue startDialogue;
    [SerializeField] private SCRPT_SingleDialogue reviveDialogue_Shirley;
    [SerializeField] private SCRPT_SingleDialogue reviveDialogue_Whitney;
    [SerializeField] private SCRPT_SingleDialogue reviveDialogue_Jason;
    [SerializeField] private SCRPT_SingleDialogue reviveDialogue_Nelson;

    [SerializeField] private Cinematic reviveCinematic;

    [SerializeField] private YardDoor door;

    private PlayerCharacter victimPlayer;

    private Queue<LTDescr> tweensQueue = new Queue<LTDescr>();

    protected override void EventsSubscriber()
    {
        IGPlayersManagerEvents.OnAllPlayersCreated += SetupVictimPlayer;
    }

    protected override void EventsUnSubscriber()
    {
        IGPlayersManagerEvents.OnAllPlayersCreated -= SetupVictimPlayer;
        if (victimPlayer == null) return;
        victimPlayer.OnOtherInteract -= StartDialogue;
    }

    private void Start()
    {
        if (DataKeeper.Instance.ShouldSkipTuto()) return;
        if (GameManager.Instance.PlayersCount <= 1)
        {
            door.OpenDoor();
            return;
        }
    }

    private void BuildCinematic(GameObject interactor)
    {
        CA_CinematicScreenFade screenFadeOut = new CA_CinematicScreenFade(false, .5f);
        CA_CinematicWait waitForFadeOut = new CA_CinematicWait(.5f);
        CA_CinematicCustomAction reviveVictim = new CA_CinematicCustomAction(() => victimPlayer.AskRevive());
        CA_CinematicCustomAction checkVictimFlip = new CA_CinematicCustomAction(
            () =>
            {
                bool flipRight = interactor.transform.position.x > victimPlayer.transform.position.x;
                victimPlayer.AnimationController.FlipSkeleton(flipRight);
            });
        CA_CinematicWait waitForRevive = new CA_CinematicWait(.25f);
        CA_CinematicScreenFade screenFadeIn = new CA_CinematicScreenFade(true, .5f);
        CA_CinematicCustomAction openDoor = new CA_CinematicCustomAction(() => door.OpenDoor());

        SCRPT_SingleDialogue dialogueToPlay = null;
        switch (victimPlayer.GetCharacterName())
        {
            case GameManager.E_CharactersNames.Shirley:
                dialogueToPlay = reviveDialogue_Shirley;
                break;

            case GameManager.E_CharactersNames.Whitney:
                dialogueToPlay = reviveDialogue_Whitney;
                break;

            case GameManager.E_CharactersNames.Jason:
                dialogueToPlay = reviveDialogue_Jason;
                break;

            case GameManager.E_CharactersNames.Nelson:
                dialogueToPlay = reviveDialogue_Nelson;
                break;

            default:
                dialogueToPlay = reviveDialogue_Shirley;
                break;
        }
        CA_CinematicDialoguePlayer revivedPlayerDialogue = new CA_CinematicDialoguePlayer(dialogueToPlay);
        CA_CinematicDialoguePlayer stDialogue = new CA_CinematicDialoguePlayer(startDialogue);

        reviveCinematic = new Cinematic(screenFadeOut, waitForFadeOut, reviveVictim, checkVictimFlip, waitForRevive, screenFadeIn, openDoor,
                                        revivedPlayerDialogue, stDialogue);
    }

    private void SetupVictimPlayer(List<PlayerCharacter> players)
    {
        IGPlayersManagerEvents.OnAllPlayersCreated -= SetupVictimPlayer;
        victimPlayer = players.RandomElement();

        FSM_Player_Manager victimStateManager = victimPlayer.StateManager;
        victimPlayer.AnimationController.FlipSkeleton(false);
        victimStateManager.ForceSetState<FSM_Player_Dying>(FSM_Player_Manager.E_PlayerState.Dying).SetAsFakeState();
        victimPlayer.transform.position = this.transform.position;
        victimPlayer.OnOtherInteract += StartDialogue;
    }

    private void StartDialogue(GameObject interactor)
    {
        BuildCinematic(interactor);
        victimPlayer.OnOtherInteract -= StartDialogue;
        interactor.GetComponent<PlayerInteractor>().PromptText.gameObject.SetActive(false);
        reviveCinematic.StartCinematic();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance.PlayersCount > 1) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        DialogueManager.Instance.TryStartDialogue(startDialogue, true);
        Destroy(this.gameObject);
    }
}

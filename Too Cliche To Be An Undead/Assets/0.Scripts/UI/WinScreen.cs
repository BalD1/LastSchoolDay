using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class WinScreen : MonoBehaviourEventsHandler
{
    [field: SerializeField] public CanvasGroup winGroup { get; private set; }

    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private CanvasGroup buttonsPanel;
    [SerializeField] private int panelShowCount = 1;

    [SerializeField] private CanvasGroup scorePanel;
    [SerializeField] private PlayersScorePanelsController scorePanelsController;

    [SerializeField] private Image youSurvivedImage;

    [SerializeField] private UIScreenBase endButtonsScreen;

    private Cinematic showUIScreen;

    protected override void Awake()
    {
        base.Awake();

        showUIScreen = new Cinematic(
            new CA_CinematicScreenFade(false, .5f),
            new CA_CinematicCustomAction(() => videoPlayer.Play()),
            new CA_CinematicCustomAction(() =>
            {
                winGroup.alpha = 1.0f;
                winGroup.interactable = true;
                winGroup.blocksRaycasts = true;
            }),
            new CA_CinematicScreenFade(true, .5f),
            new CA_CinematicWait(2),
            new CA_CinematicFadeImage(youSurvivedImage, 1, .25f),
            new CA_CinematicWait(2),
            new CA_CinematicActionMultiple(
                new CA_CinematicFadeImage(youSurvivedImage, 0, .25f),
                new CA_CinematicFadeCanvasGroup(scorePanel, 1, .25f)
                ),
            new CA_CinematicCustomAction(() => GameManager.Instance.GameState = GameManager.E_GameState.Pause),
            new CA_CinematicCustomAction(() => scorePanelsController.Begin())
            );
        showUIScreen.AllowChangeCinematicStateAtEnd(false);
    }

    protected override void EventsSubscriber()
    {
        EndCinematicEvents.OnDoorOpened += Begin;
        DebugConsoleEvents.OnForceWin += Begin;
    }

    protected override void EventsUnSubscriber()
    {
        EndCinematicEvents.OnDoorOpened -= Begin;
        DebugConsoleEvents.OnForceWin -= Begin;
    }

    public void Begin()
    {
        showUIScreen.StartCinematic();
    }

    public void ShowButtonsPanel()
    {
        scorePanel.LeanAlpha(0, .25f).setIgnoreTimeScale(true);
        endButtonsScreen.gameObject.SetActive(true);
        endButtonsScreen.Open();
    }
}

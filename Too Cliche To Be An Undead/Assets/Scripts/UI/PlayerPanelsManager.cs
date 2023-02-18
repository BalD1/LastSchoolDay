using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using static UIManager;
using static UnityEditor.Progress;

public class PlayerPanelsManager : MonoBehaviour
{

    [SerializeField] private PlayerPanel[] playerPanels;
    public PlayerPanel[] GetPlayerPanels { get => playerPanels; }

    [SerializeField] private int[] playerAssociatedCard = new int[4];
    public int[] PlayerAssociatedCard { get => playerAssociatedCard; }

    [field: SerializeField] public Sprite[] tokensByIndex { get; private set; }

    [SerializeField] private UIVideoPlayer videoPlayer;

    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private LoadingScreen loadingScreen;
    [SerializeField] private Toggle startButton;

    private void Start()
    {
        videoPlayer.SetClipWithoutPlaying(UIVideoPlayer.E_VideoTag.BookOpening);
    }

    public void Begin()
    {
        videoPlayer.StartVideo(WaitForAnimation);
    }

    public void WaitForAnimation(VideoPlayer vp)
    {
        foreach (var item in playerPanels) item.panelsManager = this;

        canvasGroup.alpha = 1;

        foreach (var item in playerPanels)
        {
            item.isEnabled = true;
        }

        PlayersManager.Instance.EnableActions();
    }

    public void JoinPanel(int idx, PlayerCharacter player)
    {
        playerPanels[0].JoinPanel(idx);
        player.D_horizontalArrowInput += OnPlayerHorizontalArrow;
        player.D_verticalArrowInput += OnPlayerVerticalArrow;
    }

    public void OnPlayerHorizontalArrow(bool rightArrow, int playerIdx)
    {
        int newIndex = playerAssociatedCard[playerIdx] + 2;

        if (newIndex >= playerPanels.Length) newIndex = newIndex - playerPanels.Length;
        if (newIndex < 0) newIndex = 2;

        JoinPanelIndex(playerIdx, newIndex);
    }

    public void OnPlayerVerticalArrow(bool upArrow, int playerIdx)
    {
        int currentIndex = playerAssociatedCard[playerIdx];
        bool add = currentIndex % 2 == 0;
        int newIndex = currentIndex + (add ? 1 : -1);

        if (newIndex >= playerPanels.Length) newIndex = 2;
        if (newIndex < 0) newIndex = 1;

        JoinPanelIndex(playerIdx, newIndex);
    }

    public void JoinPanelIndex(int playerIdx, int panelIdx)
    {
        playerPanels[playerAssociatedCard[playerIdx]].QuitPanel(playerIdx);
        playerPanels[panelIdx].JoinPanel(playerIdx);
    }

    public void RemoveAllJoined()
    {
        while (DataKeeper.Instance.playersDataKeep.Count > 1) DataKeeper.Instance.RemoveData(1);
        ResetPanels();
    }

    public void ResetPanels()
    {
        for (int i = 1; i < playerPanels.Length; i++)
        {
            playerPanels[i].ResetPanel(false);
        }
        PlayersManager.Instance.DisableActions();
    }

    public Sprite GetCharacterSprite(int idx)
    {
        foreach (var item in DataKeeper.Instance.GetCharactersSprites)
        {
            if ((int)item.characterName == idx) return item.characterSprite;
        }

        return null;
    }

    public void AskForStartGame(ButtonArgs_Scene sceneToLoad)
    {
        foreach (var item in playerPanels)
            if (item.isValid == false)
            {
                startButton.SetIsOnWithoutNotify(false);
                return;
            }

        for (int i = 0; i < DataKeeper.Instance.playersDataKeep.Count; i++)
        {
            int panelOfPlayerIndex = playerAssociatedCard[i];
            GameManager.E_CharactersNames characterOfPanelI = GetPlayerPanels[panelOfPlayerIndex].associatedCharacter;
            DataKeeper.Instance.playersDataKeep[i].character = characterOfPanelI;
        }

        UIManager.Instance.SelectButton("None");
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.StartLoad(sceneToLoad);
    }
}

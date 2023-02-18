using UnityEngine;
using UnityEngine.Video;

public class PlayerPanelsManager : MonoBehaviour
{

    [SerializeField] private PlayerPanel[] playerPanels;
    public PlayerPanel[] GetPlayerPanels { get => playerPanels; }

    [SerializeField] private int[] playerAssociatedCard = new int[4];
    public int[] PlayerAssociatedCard { get => playerAssociatedCard; }

    [field: SerializeField] public Sprite[] tokensByIndex { get; private set; }

    [SerializeField] private UIVideoPlayer videoPlayer;

    [SerializeField] private CanvasGroup canvasGroup;

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
        player.D_verticalArrowInput += OnPlayerHorizontalArrow;
    }

    public void OnPlayerHorizontalArrow(bool rightArrow, int playerIdx)
    {
        int newIndex = playerAssociatedCard[playerIdx] + (rightArrow ? 2 : -2);

        if (newIndex >= playerPanels.Length) newIndex = 0;
        if (newIndex < 0) newIndex = playerPanels.Length - 1;

        JoinPanelIndex(playerIdx, newIndex);
    }

    public void OnPlayerVerticalArrow(bool upArrow, int playerIdx)
    {
        int newIndex = playerAssociatedCard[playerIdx] + (upArrow ? -1 : 1);

        if (newIndex >= playerPanels.Length) newIndex = 0;
        if (newIndex < 0) newIndex = playerPanels.Length - 1;

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
}

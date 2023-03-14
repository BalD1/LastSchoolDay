using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.iOS;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayerPanelsManager : MonoBehaviour
{

    [SerializeField] private PlayerPanel[] playerPanels;
    public PlayerPanel[] GetPlayerPanels { get => playerPanels; }

    [field: SerializeField] public Sprite[] tokensByIndex { get; private set; }

    [SerializeField] private UIVideoPlayer videoPlayer;

    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private LoadingScreen loadingScreen;
    [SerializeField] private Toggle startButton;

    [SerializeField] private ButtonArgs_Scene sceneArgs;
    [SerializeField] private Button backButton;

    [System.Serializable]
    public struct S_ImageByCharacter
    {
        public GameManager.E_CharactersNames character;
        public Sprite image;
    }

    [field: SerializeField] public S_ImageByCharacter[] ImagesByCharacter { get; private set; } 

    private Coroutine animationCoroutine;

    private bool allowMovements = false;

    private void Start()
    {
        videoPlayer.SetClipWithoutPlaying(UIVideoPlayer.E_VideoTag.BookOpening);

        for (int i = 1; i < playerPanels.Length; i++)
        {
            playerPanels[i].Disable();
        }

        DataKeeper.Instance.D_playerCreated += JoinPanel;
    }

    private void OnValidateInput()
    {
        if (sceneArgs == null)
        {
            sceneArgs = new ButtonArgs_Scene();
            sceneArgs.args = GameManager.E_ScenesNames.MainScene;
        }

        AskForStartGame(sceneArgs);
    }

    private void OnBackInput()
    {
        backButton.onClick?.Invoke();

        GameManager.Player1Ref.D_validateInput -= OnValidateInput;
        GameManager.Player1Ref.D_cancelInput -= OnBackInput;
    }

    public void Begin()
    {
        playerPanels[0].SoftReset();

        videoPlayer.StartVideo();
        StartCoroutine(videoPlayer.WaitForAction(1.65f, WaitForAnimation));
    }

    public void WaitForAnimation()
    {
        UIManager.Instance.SelectButton("Lobby");
        foreach (var item in playerPanels) item.panelsManager = this;

        foreach (var item in playerPanels)
        {
            item.PanelButton.interactable = false;
            item.ButtonText.raycastTarget = false;
            item.transform.localScale = Vector2.zero;
        }

        JoinPanel(0, GameManager.Player1Ref);

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;

        animationCoroutine = StartCoroutine(PanelsAnimation());

        GameManager.Player1Ref.D_validateInput += OnValidateInput;
        GameManager.Player1Ref.D_cancelInput += OnBackInput;


        PlayersManager.Instance.EnableActions();
    }

    private IEnumerator PanelsAnimation()
    {
        foreach (var item in playerPanels)
        {
            item.transform.LeanRotateZ(5, .75f);
            item.transform.LeanScale(new Vector2(1.15f, 1.15f), .75f).setOnComplete(
            () =>
            {
                item.transform.LeanRotateZ(0, .75f);
                item.transform.LeanScale(Vector2.one, .75f).setOnComplete( 
                () =>
                {
                    item.PanelButton.interactable = true;
                    item.ButtonText.raycastTarget = true;
                    allowMovements = true;
                });

            });

            yield return new WaitForSecondsRealtime(.15f);
        }
    }

    public void JoinPanel(int idx, PlayerCharacter player)
    {
        playerPanels[idx].JoinPanel(idx);
        player.D_navigationArrowInput += OnPlayerNavigationInput;
        player.D_horizontalArrowInput += OnPlayerHorizontalArrow;
        player.D_verticalArrowInput += OnPlayerVerticalArrow;
    }

    private void OnPlayerNavigationInput(Vector2 value, int playerIdx)
    {
        switch(value)
        {
            case Vector2 v when value == Vector2.up:
                OnPlayerVerticalArrow(true, playerIdx);
                break;

            case Vector2 v when value == Vector2.down:
                OnPlayerVerticalArrow(false, playerIdx);
                break;

            case Vector2 v when value == Vector2.left:
                OnPlayerHorizontalArrow(true, playerIdx);
                break;

            case Vector2 v when value == Vector2.right:
                OnPlayerHorizontalArrow(false, playerIdx);
                break;
        }
    }

    public void OnPlayerHorizontalArrow(bool rightArrow, int playerIdx)
    {
        int newCharIdx = playerPanels[playerIdx].CurrentCharacterIdx;

        if (rightArrow) newCharIdx++;
        else newCharIdx--;

        int maxArrayIdx = ImagesByCharacter.Length - 1;
        if (newCharIdx > maxArrayIdx) newCharIdx = 0;
        else if (newCharIdx < 0) newCharIdx = maxArrayIdx;

        S_ImageByCharacter newChar = ImagesByCharacter[newCharIdx];

        playerPanels[playerIdx].ChangeCharacter(newChar, newCharIdx);
    }

    public void OnPlayerVerticalArrow(bool upArrow, int playerIdx)
    {

    }

    public void RemoveAllJoined()
    {
        while (DataKeeper.Instance.playersDataKeep.Count > 1) DataKeeper.Instance.RemoveData(1);
        ResetPanels();
    }

    public void ResetPanels()
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);

        allowMovements = false;

        GameManager.Player1Ref.SwitchControlMapToUI();

        foreach (var item in GameManager.Instance.playersByName)
        {
            item.playerScript.D_horizontalArrowInput -= OnPlayerHorizontalArrow;
            item.playerScript.D_verticalArrowInput -= OnPlayerVerticalArrow;
            item.playerScript.D_navigationArrowInput -= OnPlayerNavigationInput;
        }

        playerPanels[0].SoftReset();
        for (int i = 1; i < playerPanels.Length; i++)
        {
            LeanTween.cancel(playerPanels[i].gameObject);
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
            if (item.IsEnabled && !item.isValid)
            {
                startButton.SetIsOnWithoutNotify(false);
                return;
            }

        foreach (var item in playerPanels)
            if (item.IsEnabled) item.AssociateCharacterToPlayer();

        UIManager.Instance.SelectButton("None");
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.StartLoad(sceneToLoad);
    }

    private void OnDestroy()
    {
        DataKeeper.Instance.D_playerCreated -= JoinPanel;
    }
}

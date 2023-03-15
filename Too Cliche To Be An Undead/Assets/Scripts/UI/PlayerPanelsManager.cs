using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanelsManager : MonoBehaviour
{

    [SerializeField] private PlayerPanel[] playerPanels;
    public PlayerPanel[] GetPlayerPanels { get => playerPanels; }

    [field: SerializeField] public Sprite[] PlayerTokens { get; private set; }

    public Queue<Sprite> tokensQueue;

    [SerializeField] private UIVideoPlayer videoPlayer;

    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private GameObject preLoadingScreen;
    [SerializeField] private Toggle startButton;

    [SerializeField] private Button backButton;

    private bool p1Joined = false;

    private int[] panelsCharacterIdx = new int[4] { -1, -1, -1, -1 };

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
        PopulateTokensQueue();

        videoPlayer.SetClipWithoutPlaying(UIVideoPlayer.E_VideoTag.BookOpening);

        for (int i = 1; i < playerPanels.Length; i++)
        {
            playerPanels[i].Disable();
        }

        DataKeeper.Instance.D_playerCreated += JoinPanel;
        DataKeeper.Instance.D_playerDestroyed += QuitPanel;
    }

    private void PopulateTokensQueue()
    {
        tokensQueue = new Queue<Sprite>();

        foreach (var item in PlayerTokens)
        {
            tokensQueue.Enqueue(item);
        }
    }

    private void OnValidateInput()
    {
        AskForStartGame();
    }

    private void OnBackInput()
    {
        backButton.onClick?.Invoke();
    }

    public void Begin()
    {
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

        PopulateTokensQueue();
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
        bool isP1 = player == GameManager.Player1Ref;

        if (isP1 && p1Joined) return;

        foreach (var item in playerPanels)
        {
            if (item.IsEnabled == false)
            {
                if (isP1) p1Joined = true;

                panelsCharacterIdx[item.panelID] = 0;

                item.JoinPanel(idx, player);
                player.D_navigationArrowInput += OnPlayerNavigationInput;
                player.D_horizontalArrowInput += OnPlayerHorizontalArrow;
                player.D_verticalArrowInput += OnPlayerVerticalArrow;

                VerifyPanelsValidity();
                return;
            }
        }

    }

    public void QuitPanel(int idx, PlayerCharacter player)
    {
        foreach (var item in playerPanels)
        {
            if (item.CurrentPlayerIdx == idx)
            {
                panelsCharacterIdx[item.panelID] = -1;

                item.ChangeCharacter(ImagesByCharacter[0], 0);

                item.QuitPanel(idx);
                player.D_navigationArrowInput -= OnPlayerNavigationInput;
                player.D_horizontalArrowInput -= OnPlayerHorizontalArrow;
                player.D_verticalArrowInput -= OnPlayerVerticalArrow;

                VerifyPanelsValidity();
                return;
            }
        }
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
        // Get the panel idx associated to the targeted player
        int panelIdx = -1;
        for (int i = 0; i < playerPanels.Length; i++)
        {
            if (playerPanels[i].CurrentPlayerIdx == playerIdx)
            {
                panelIdx = i;
                break;
            }
        }

        if (panelIdx == -1) return;

        // get the current character on the panel
        int newCharIdx = playerPanels[panelIdx].CurrentCharacterIdx;

        // get next if right arrow
        if (rightArrow) newCharIdx++;
        else newCharIdx--;

        // array bounds
        int maxArrayIdx = ImagesByCharacter.Length - 1;
        if (newCharIdx > maxArrayIdx) newCharIdx = 0;
        else if (newCharIdx < 0) newCharIdx = maxArrayIdx;

        // get the new character image
        S_ImageByCharacter newChar = ImagesByCharacter[newCharIdx];

        // keep character idx in array
        panelsCharacterIdx[panelIdx] = newCharIdx;

        playerPanels[panelIdx].ChangeCharacter(newChar, newCharIdx);

        VerifyPanelsValidity();
    }

    private void VerifyPanelsValidity()
    {
        foreach (var item in playerPanels)
            if (item.IsEnabled) item.SetValidity(true);

        for (int i = 0; i < playerPanels.Length - 2; i++)
        {
            if (playerPanels[i].IsEnabled == false) continue;

            for (int j = 1; j < playerPanels.Length - 1; j++)
            {
                if (playerPanels[i + j].IsEnabled == false) continue;

                if (playerPanels[i].CurrentCharacterIdx == playerPanels[i + j].CurrentCharacterIdx)
                {
                    playerPanels[i].SetValidity(false);
                    playerPanels[i + j].SetValidity(false);
                }
            }
        }
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

        p1Joined = false;

        allowMovements = false;

        PopulateTokensQueue();

        GameManager.Player1Ref.SwitchControlMapToUI();

        GameManager.Player1Ref.D_validateInput -= OnValidateInput;
        GameManager.Player1Ref.D_cancelInput -= OnBackInput;

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
            playerPanels[i].ChangeCharacter(ImagesByCharacter[0], 0);
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

    public void Refocus()
    {
        GameManager.Player1Ref.D_validateInput += OnValidateInput;
        GameManager.Player1Ref.D_cancelInput += OnBackInput;
        PlayersManager.Instance.EnableActions();

        foreach (var item in GameManager.Instance.playersByName)
        {
            item.playerScript.D_horizontalArrowInput += OnPlayerHorizontalArrow;
            item.playerScript.D_verticalArrowInput += OnPlayerVerticalArrow;
            item.playerScript.D_navigationArrowInput += OnPlayerNavigationInput;
        }
    }

    public void AskForStartGame()
    {
        foreach (var item in playerPanels)
            if (item.IsEnabled && !item.isValid)
            {
                startButton.SetIsOnWithoutNotify(false);
                return;
            }

        foreach (var item in GameManager.Instance.playersByName)
        {
            item.playerScript.D_horizontalArrowInput -= OnPlayerHorizontalArrow;
            item.playerScript.D_verticalArrowInput -= OnPlayerVerticalArrow;
            item.playerScript.D_navigationArrowInput -= OnPlayerNavigationInput;
        }

        GameManager.Player1Ref.D_validateInput -= OnValidateInput;
        GameManager.Player1Ref.D_cancelInput -= OnBackInput;

        foreach (var item in playerPanels)
            if (item.IsEnabled) item.AssociateCharacterToPlayer();

        startButton.SetIsOnWithoutNotify(false);

        PlayersManager.Instance.DisableActions();

        UIManager.Instance.SelectButton("None");
        preLoadingScreen.SetActive(true);
        preLoadingScreen.GetComponent<PreloadScreen>().BeginScreen();
    }

    private void AttachOnArrowsToAllPlayers()
    {
        foreach (var item in GameManager.Instance.playersByName)
        {
            AttachArrowsToPlayer(item.playerScript);
        }
    }

    private void DetachOnArrowsToAllPlayers()
    {
        foreach (var item in GameManager.Instance.playersByName)
        {
            DetachArrowsToPlayer(item.playerScript);
        }
    }

    private void AttachArrowsToPlayer(PlayerCharacter p)
    {
         p.D_horizontalArrowInput += OnPlayerHorizontalArrow;
         p.D_verticalArrowInput += OnPlayerVerticalArrow;
         p.D_navigationArrowInput += OnPlayerNavigationInput;
    }
    private void DetachArrowsToPlayer(PlayerCharacter p)
    {
        p.D_horizontalArrowInput -= OnPlayerHorizontalArrow;
        p.D_verticalArrowInput -= OnPlayerVerticalArrow;
        p.D_navigationArrowInput -= OnPlayerNavigationInput;
    }

    private void AttachInputsToP1()
    {
        GameManager.Player1Ref.D_validateInput += OnValidateInput;
        GameManager.Player1Ref.D_cancelInput += OnBackInput;
    }
    private void DetachInputsToP1()
    {
        GameManager.Player1Ref.D_validateInput -= OnValidateInput;
        GameManager.Player1Ref.D_cancelInput = OnBackInput;
    }

    private void OnDestroy()
    {
        DataKeeper.Instance.D_playerCreated -= JoinPanel;
        DataKeeper.Instance.D_playerDestroyed -= QuitPanel;
    }
}

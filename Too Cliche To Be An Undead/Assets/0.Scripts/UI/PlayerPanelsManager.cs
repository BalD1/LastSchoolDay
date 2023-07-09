using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerPanelsManager : UIScreenBase
{

    [SerializeField] private PlayerPanel[] playerPanels;
    public PlayerPanel[] GetPlayerPanels { get => playerPanels; }

    [field: SerializeField] public Sprite[] PlayerTokens { get; private set; }

    public Queue<Sprite> tokensQueue;

    [SerializeField] private UIVideoPlayer videoPlayer;

    [SerializeField] private GameObject preLoadingScreen;
    [SerializeField] private Toggle startButton;

    [SerializeField] private Button backButton;

    private int[] panelsCharacterIdx = new int[4] { -1, -1, -1, -1 };

    private float[] timeOfLastChange = new float[4];
    [SerializeField] private float characterChangeCooldown = .5f;

    [System.Serializable]
    public struct S_ImageByCharacter
    {
        public GameManager.E_CharactersNames character;
        public Sprite image;
    }

    [field: SerializeField] public S_ImageByCharacter[] ImagesByCharacter { get; private set; } 

    private Coroutine animationCoroutine;

    private bool allowClose = false;

    private void Start()
    {
        PopulateTokensQueue();

        videoPlayer.SetClipWithoutPlaying(UIVideoPlayer.E_VideoTag.BookOpening);

        for (int i = 1; i < playerPanels.Length; i++)
        {
            playerPanels[i].Disable();
        }
    }

    private void PopulateTokensQueue()
    {
        tokensQueue = new Queue<Sprite>();

        foreach (var item in PlayerTokens)
        {
            tokensQueue.Enqueue(item);
        }
    }

    private void OnValidateInput(int idx)
    {
        AskForStartGame();
    }

    private void OnBackInput(int idx)
    {
        if (idx != 0) return;
        backButton.onClick?.Invoke();
    }

    public override void Open()
    {
        foreach (var item in playerPanels) item.transform.localScale = Vector2.zero;
        videoPlayer.StartVideo();
        this.OpenScreen();
        StartCoroutine(videoPlayer.WaitForAction(1.65f, WaitForAnimation));
    }

    public override void Hide()
    {
        base.Hide();
        DetachInputs();
    }

    public override void Show()
    {
        base.Show();
        AttachInputs();
    }

    public override void Close()
    {
        if (!allowClose) return;
        allowClose = false;
        StopAllCoroutines();
        foreach (var item in playerPanels)
        {
            LeanTween.cancel(item.gameObject);
            item.transform.localScale = Vector2.zero;
        }
        DetachInputs();
        ResetPanels();
        videoPlayer.GetVideoPlayer.Stop();
        videoPlayer.FadeVideo(false, 0);
        base.Close();
    }

    public void WaitForAnimation()
    {
        foreach (var item in playerPanels) item.panelsManager = this;

        foreach (var item in playerPanels)
        {
            item.PanelButton.interactable = false;
            item.ButtonText.raycastTarget = false;
            item.transform.localScale = Vector2.zero;
        }

        PopulateTokensQueue();
        JoinPanel(PlayerInputsManager.Instance.GetPlayerInputs(0));

        EventSystem.current.SetSelectedGameObject(ObjectToSelectOnOpen);
        foreach (var item in screenTweens) item.StartTweenIn();

        PlayerInputsEvents.OnCancelButton += Close;

        if (ObjectToSelectOnOpen != null)
            EventSystem.current.SetSelectedGameObject(ObjectToSelectOnOpen);

        animationCoroutine = StartCoroutine(PanelsAnimation());

        allowClose = true;
        AttachInputs();
        AttachArrowsToPlayer();
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
                });

            });

            yield return new WaitForSecondsRealtime(.15f);
        }
    }

    public void JoinPanel(PlayerInputs playerInputs)
    {
        foreach (var item in playerPanels)
        {
            if (item.IsEnabled == false)
            {
                panelsCharacterIdx[item.panelID] = 0;

                item.JoinPanel(playerInputs);

                VerifyPanelsValidity();
                return;
            }
            else if (item.CurrentInputsIdx == playerInputs.InputsID)
            {
                QuitPanel(playerInputs.InputsID);
                return;
            }
        }

    }

    public void QuitPanel(int idx)
    {
        foreach (var item in playerPanels)
        {
            if (item.CurrentInputsIdx == idx)
            {
                panelsCharacterIdx[item.panelID] = -1;
                item.QuitPanel();
                VerifyPanelsValidity();
                break;
            }
        }
        RearrangePanels();
    }

    private void OnPlayerNavigationInput(Vector2 value, int playerIdx)
    {
        if (value == Vector2.left) OnPlayerHorizontalArrow(true, playerIdx);
        else if (value == Vector2.right) OnPlayerHorizontalArrow(false, playerIdx);
    }

    public void OnPlayerHorizontalArrow(bool rightArrow, int playerIdx)
    {
        float currentTime = Time.timeSinceLevelLoad;

        if (currentTime - timeOfLastChange[playerIdx] < characterChangeCooldown) return;

        timeOfLastChange[playerIdx] = currentTime;

        // Get the panel idx associated to the targeted player
        int panelIdx = -1;
        for (int i = 0; i < playerPanels.Length; i++)
        {
            if (playerPanels[i].CurrentInputsIdx == playerIdx)
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

    public void ResetPanels()
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);

        PopulateTokensQueue();

        DetachInputs();
        DetachArrowsToPlayer();

        playerPanels[0].SoftReset();
        for (int i = 1; i < playerPanels.Length; i++)
        {
            LeanTween.cancel(playerPanels[i].gameObject);
            playerPanels[i].QuitPanel();
        }
    }

    public Sprite GetCharacterSprite(int idx)
    {
        foreach (var item in DataKeeper.Instance.GetCharactersSprites)
        {
            if ((int)item.characterName == idx) return item.characterSprite;
        }

        return null;
    }

    public void AskForStartGame()
    {
        foreach (var item in playerPanels)
            if (item.IsEnabled && !item.isValid)
            {
                startButton.SetIsOnWithoutNotify(false);
                return;
            }

        DetachArrowsToPlayer();
        DetachInputs();

        foreach (var item in playerPanels)
            if (item.IsEnabled) item.AssociateCharacterToPlayer();

        startButton.SetIsOnWithoutNotify(false);

        UIManager.Instance.SelectButton("None");
        preLoadingScreen.GetComponent<PreloadScreen>().Open();
    }

    private void AttachArrowsToPlayer()
    {
        PlayerInputsEvents.OnNavigate += OnPlayerNavigationInput;
    }
    private void DetachArrowsToPlayer()
    {
        PlayerInputsEvents.OnNavigate -= OnPlayerNavigationInput;
    }

    private void RearrangePanels()
    {
        for (int i = 1; i < playerPanels.Length - 1; i++)
        {
            if (playerPanels[i].LinkedInputs != null) continue;

            if (playerPanels[i + 1].LinkedInputs != null)
            {
                playerPanels[i].JoinPanel(playerPanels[i + 1].LinkedInputs);
                playerPanels[i + 1].GetCharacter(out PlayerPanelsManager.S_ImageByCharacter character, out int characterIdx);
                playerPanels[i].ChangeCharacter(character, characterIdx);
                playerPanels[i + 1].QuitPanel(false);
            }
            else playerPanels[i].ResetPanel();
        }
    }

    private void AttachInputs()
    {
        this.PanelsActive();
        PlayerInputsEvents.OnValidateButton += OnValidateInput;
        PlayerInputsEvents.OnCancelButton += OnBackInput;
        PlayerInputsEvents.OnPlayerInputsCreated += JoinPanel;
        PlayerInputsEvents.OnPlayerInputsDestroyed += QuitPanel;
    }
    private void DetachInputs()
    {
        this.PanelsInacative();
        PlayerInputsEvents.OnValidateButton -= OnValidateInput;
        PlayerInputsEvents.OnCancelButton -= OnBackInput;
        PlayerInputsEvents.OnPlayerInputsCreated -= JoinPanel;
        PlayerInputsEvents.OnPlayerInputsDestroyed -= QuitPanel;
    }
}

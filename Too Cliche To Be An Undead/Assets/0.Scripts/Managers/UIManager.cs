using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text;
using System;
using Unity.VisualScripting;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private EventSystem eventSystem;

    #region Buttons & Menus

    [field: SerializeField] public Canvas MainCanvas { get; private set; }
    public static Canvas GetMainCanvas
    {
        get
        {
            if (Instance == null || Instance.MainCanvas == null) return GameObject.FindObjectOfType<Canvas>();
            return Instance.MainCanvas;
        }
    }
    public static Vector2 CanvasSize { get; private set; }

    [SerializeField] private Button firstSelectedButton_MainMenu;

    [SerializeField] private UIScreenBase pauseMenu;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject gameoverMenu;
    [SerializeField] private GameObject localHUD;
    [SerializeField] private GameObject mainMenu_mainPanel;

    [SerializeField] private Image fadeImage;

    [SerializeField] private Toggle OPTION_DashOnMovementsToggle;

    #endregion

    #region HUD

    private RectTransform hudRect;
    private bool isHUDTransparent = false;
    private bool isBossHUDTransparent = false;

    [SerializeField] private float hudTransparencyValue = .3f;
    [SerializeField] private float hudTransparencyTime = .3f;

    public Collider2D[] playersColliders = new Collider2D[8];
    public List<Collider2D> bossesColliders = new List<Collider2D>();

    [SerializeField] private RectTransform hudContainerRect;

    [SerializeField] private Image[] hudKeycards;
    public Image[] HudKeycards { get => hudKeycards; }

    [SerializeField] private GameObject coinsContainer;
    public GameObject CoinsContainer { get => coinsContainer; }

    [SerializeField] private TextMeshProUGUI moneyCounter;

    [SerializeField] private GameObject stampTimer;
    public GameObject StampTimer { get => stampTimer; }
    [SerializeField] private RectTransform stampWorldTextSpawnPos;
    public RectTransform StampWorldTextSpawnPos { get => stampWorldTextSpawnPos; }

    #endregion

    #region UI Refs

    [SerializeField] private Scrollbar shopBar;

    [SerializeField] private Button optionButton_Pause;
    [SerializeField] private Button mainMenuButton_Pause;

    [SerializeField] private GameObject minimap;
    [SerializeField] private GameObject minimapButton;

    [SerializeField] private UIBlackBars blackBars;

    [SerializeField] private GameObject keycardsContainer;
    [SerializeField] private RectTransform keycardsContainerRect;
    [SerializeField] private TextMeshProUGUI keycardsCounter;

    [SerializeField] private CanvasGroup hudContainer;
    public CanvasGroup HUDContainer { get => hudContainer; }
    [SerializeField] private CanvasGroup tutoHUD;

    public CanvasGroup TutoHUD { get => tutoHUD; }

    [SerializeField] private CanvasGroup dialogueContainer;

    #endregion

    #region Players UI Components

    [System.Serializable]
    public struct PlayerHUD
    {
        public GameObject container;
        public RectTransform rect;
        public Image portrait;
        public Image hpBar;
        public Image hpBarContainer;
        public TextMeshProUGUI hpText;

        public RectTransform skillContainer;
        public Image skillFill;
        public Image skillThumbnail;
        public TextMeshProUGUI skillTimerTXT;

        public RectTransform dashContainer;
        public Image dashFill;
        public Image dashThumbnail;
        public TextMeshProUGUI dashTimerTXT;
    }

    [SerializeField] private CharacterPortrait[] characterPortrait;
    public CharacterPortrait[] CharacterPortraits { get => characterPortrait; }

    [System.Serializable]
    public struct CharacterPortrait
    {
        public GameManager.E_CharactersNames characterName;


        public CharacterPortraitByHP[] characterPortraitsByHP;
    }

    [System.Serializable]
    public struct CharacterPortraitByHP
    {
        public float percentage;
        public Sprite portrait;
    }

    #endregion

    [SerializeField] private PlayerPanelsManager panelsManager;

    private Stack<UIScreenBase> openMenusQueues = new Stack<UIScreenBase>();
    private bool uiOpenedByExternalMenu = false;

    public delegate void D_CloseMenu();
    public D_CloseMenu D_closeMenu;

#if UNITY_EDITOR
    [SerializeField] private List<UIScreenBase> EDITOR_openMenusQueues;
#endif

    private GameObject lastSelected;

    private Scrollbar currentVerticalScrollbar;
    private Scrollbar currentHorizontalScrollbar;

    #region Getters

    public PlayerPanelsManager PanelsManager { get => panelsManager; }

    public Stack<UIScreenBase> OpenMenusQueues { get => openMenusQueues; }

    public GameObject KeycardContainer { get => keycardsContainer; }

    public TextMeshProUGUI KeycardsCounters { get => keycardsCounter; }

    public Scrollbar CurrentHorizontalScrollbar { get => currentHorizontalScrollbar; }

    public Scrollbar CurrentVerticalScrollbar { get => currentVerticalScrollbar; }

    #endregion

    public const int maxPBImagesByRows = 6;

    public const float scrollbarSensibility = .1f;

    private bool firstGameStatePassFlag = false;
    private bool isInTuto;

    private LTDescr currentScreenFade;

    #region Awake / Start / Updates

    protected override void EventsSubscriber()
    {
        GameManagerEvents.OnGameStateChange += OnGameStateChange;
        PlayerInputsEvents.OnScrollCurrentHBLeftCall += ScrollCurrentHorizontalBarLeft;
        PlayerInputsEvents.OnScrollCurrentHBRightCall += ScrollCurrentHorizontalBarRight;
        PlayerInputsEvents.OnScrollCurrentVBDownCall += ScrollCurrentVerticalBarDown;
        PlayerInputsEvents.OnScrollCurrentVBUpCall += ScrollCurrentVerticalBarUp;
        PlayerInputsEvents.OnCancelButton += CloseLastMenu;
        PlayerInputsEvents.OnPauseCall += OpenPauseMenu;
        PlayerInputsEvents.OnStart += CloseLastMenu;
        DialogueManagerEvents.OnStartDialogue += OnStartDialogue;
        DialogueManagerEvents.OnEndDialogue += OnEndDialogue;
        TutorialEvents.OnTutorialStarted += OnTutorialStarted;
        TutorialEvents.OnTutorialEnded += OnTutorialEnded;
        CinematicManagerEvents.OnChangeCinematicState += OnChangeCinamticState;
        UIScreenBaseEvents.OnOpenScreen += OnScreenOpened;
        UIScreenBaseEvents.OnCloseScreen += OnScreenClosed;
        UIScreenBaseEvents.OnShowScreen += OnScreenShow;
        UIScreenBaseEvents.OnHideScreen += OnScreenHide;
        SpawnersManagerEvents.OnSpawnedKeycards += UpdateKeycardsText;
        SpawnersManagerEvents.OnSpawnedKeycardSingle += SetupCard;
    }

    protected override void EventsUnSubscriber()
    {
        GameManagerEvents.OnGameStateChange -= OnGameStateChange;
        PlayerInputsEvents.OnScrollCurrentHBLeftCall -= ScrollCurrentHorizontalBarLeft;
        PlayerInputsEvents.OnScrollCurrentHBRightCall -= ScrollCurrentHorizontalBarRight;
        PlayerInputsEvents.OnScrollCurrentVBDownCall -= ScrollCurrentVerticalBarDown;
        PlayerInputsEvents.OnScrollCurrentVBUpCall -= ScrollCurrentVerticalBarUp;
        PlayerInputsEvents.OnCancelButton -= CloseLastMenu;
        PlayerInputsEvents.OnPauseCall -= OpenPauseMenu;
        PlayerInputsEvents.OnStart -= CloseLastMenu;
        DialogueManagerEvents.OnStartDialogue -= OnStartDialogue;
        DialogueManagerEvents.OnEndDialogue -= OnEndDialogue;
        TutorialEvents.OnTutorialStarted -= OnTutorialStarted;
        TutorialEvents.OnTutorialEnded -= OnTutorialEnded;
        CinematicManagerEvents.OnChangeCinematicState -= OnChangeCinamticState;
        UIScreenBaseEvents.OnOpenScreen -= OnScreenOpened;
        UIScreenBaseEvents.OnCloseScreen -= OnScreenClosed;
        UIScreenBaseEvents.OnShowScreen -= OnScreenShow;
        UIScreenBaseEvents.OnHideScreen -= OnScreenHide;
        SpawnersManagerEvents.OnSpawnedKeycards -= UpdateKeycardsText;
        SpawnersManagerEvents.OnSpawnedKeycardSingle -= SetupCard;
    }

    protected override void Awake()
    {
        base.Awake();
        instance = this;

        if (MainCanvas == null) MainCanvas = GameObject.FindObjectOfType<Canvas>();
        RectTransform canvasTransform = MainCanvas.transform as RectTransform;
        CanvasSize = new Vector2(canvasTransform.rect.width, canvasTransform.rect.height);

        if (hudContainer != null) hudContainer.alpha = 0;

        if (eventSystem == null) eventSystem = EventSystem.current;
    }

    protected override void Start()
    {
        base.Start();
        FadeScreen(false);
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu))
        {   
        }
        else
        {
            hudRect = localHUD.GetComponent<RectTransform>();

            if (DataKeeper.Instance.skipTuto || DataKeeper.Instance.alreadyPlayedTuto)
            {
                FadeAllHUD(true);
            }
            else tutoHUD.gameObject.SetActive(true);
            keycardsContainer.SetActive(false);

            moneyCounter.text = "x " + 0;
        }
    }

    #endregion

    private void OnGameStateChange(GameManager.E_GameState newState)
    {
        switch (newState)
        {
            case GameManager.E_GameState.MainMenu:
                break;

            case GameManager.E_GameState.InGame:
                if (currentHorizontalScrollbar != null) UnsetCurrentHorizontalScrollbar();

                if (!firstGameStatePassFlag) firstGameStatePassFlag = true;
                else
                {
                    if (GameManager.Instance.IsInTutorial) FadeTutoHUD(fadeIn: true);
                    else FadeInGameHUD(true);
                }
                break;

            case GameManager.E_GameState.Pause:
                FadeAllHUD(fadeIn: false);
                PostproManager.Instance.SetBlurState(true);
                break;

            case GameManager.E_GameState.Restricted:
                break;

            case GameManager.E_GameState.Win:
                break;

            case GameManager.E_GameState.GameOver:
                break;

            case GameManager.E_GameState.Cinematic:
                break;

            default:
                Debug.LogError(newState + "not found in switch statement.");
                break;
        }
    }

    private void OnTutorialStarted() => isInTuto = true;
    private void OnTutorialEnded()
    {
        this.tutoHUD.gameObject.SetActive(false);
        keycardsContainer.gameObject.SetActive(true);
        this.isInTuto = false;
    }

    private void OnStartDialogue(bool fromCinematic)
    {
        if (fromCinematic) return;
        SetBlackBars(true, .15f);
        FadeAllHUD(false);
    }
    private void OnEndDialogue(bool fromCinematic)
    {
        if (fromCinematic) return;
        SetBlackBars(false, .15f);

        if (isInTuto) this.FadeTutoHUD(!fromCinematic);
        else this.FadeAllHUD(!fromCinematic);
    }

    private void OnChangeCinamticState(bool isInCinematic)
    {
        this.SetBlackBars(isInCinematic);
        if (isInTuto) this.FadeTutoHUD(!isInCinematic);
        else this.FadeAllHUD(!isInCinematic);

    }

    public void SetOptionsState()
    {
        OPTION_DashOnMovementsToggle?.SetIsOnWithoutNotify(GameManager.OPTION_DashToMouse);
    }

    #region Players HUD

    private void SetupCard(Keycard card)
    {
        card.onAnimationEnded += () => UpdateKeycardsText();
        for (int i = 0; i < HudKeycards.Length; i++)
        {
            Image uiImage = HudKeycards[i];
            if (uiImage.color == Color.white)
            {
                card.Setup(uiImage, uiImage.transform as RectTransform);
                break;
            }
        }
    }

    public void UpdateKeycardsText()
    {
        keycardsContainer.gameObject.SetActive(true);
        StringBuilder sb = new StringBuilder();
        sb.Append(GameManager.Instance.AcquiredCards);
        sb.Append(" / ");
        sb.Append(GameManager.Instance.NeededCards);
        KeycardsCounters.text = sb.ToString();
    }

    #endregion

    #region Scrollbars

    public void SetCurrentVerticalScrollbar(Scrollbar bar) => currentVerticalScrollbar = bar;
    public void UnsetCurrentVerticalScrollbar() => currentVerticalScrollbar = null;

    public void SetCurrentHorizontalScrollbar(Scrollbar bar) => currentHorizontalScrollbar = bar;
    public void UnsetCurrentHorizontalScrollbar() => currentHorizontalScrollbar = null;

    public void ScrollCurrentVerticalBarDown()
    {
        if (currentVerticalScrollbar != null) currentVerticalScrollbar.value -= scrollbarSensibility;
    }
    public void ScrollCurrentVerticalBarUp()
    {
        if (currentVerticalScrollbar != null) currentVerticalScrollbar.value += scrollbarSensibility;
    }

    public void ScrollCurrentHorizontalBarLeft()
    {
        if (currentHorizontalScrollbar != null) currentHorizontalScrollbar.value -= scrollbarSensibility;
    }
    public void ScrollCurrentHorizontalBarRight()
    {
        if (currentHorizontalScrollbar != null) currentHorizontalScrollbar.value += scrollbarSensibility;
    }

    #endregion

    #region Menus Managers

    private void OpenPauseMenu()
    {
        if (openMenusQueues.Count == 0) pauseMenu.Open();
    }
    private void CloseLastMenu(InputAction.CallbackContext ctx, PlayerInputHandler input)
    {
        if (!ctx.performed) return;
        if (openMenusQueues.TryPeek(out UIScreenBase currentScreen) && currentScreen.AllowCloseOnStart)
        {
            bool ignoreTween = openMenusQueues.Count > (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu) ? 2 : 1);
            currentScreen.Close(ignoreTween);
        }
    }
    private void CloseLastMenu(int playerIdx)
    {
        if (openMenusQueues.TryPeek(out UIScreenBase currentScreen) && currentScreen.AllowCloseOnBack)
        {
            bool ignoreTween = openMenusQueues.Count > (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu) ? 2 : 1);
            currentScreen.Close(ignoreTween);
        }
    }

    private void OnScreenOpened(UIScreenBase openedScreen, bool ignoreTweens)
    {
        if (openMenusQueues.TryPeek(out UIScreenBase lastScreen))
            lastScreen.Hide(ignoreTweens);
        else
        {
            if (openedScreen != pauseMenu) uiOpenedByExternalMenu = true;
            this.EnteredUI();
        }
        openMenusQueues.Push(openedScreen);
    }
    private void OnScreenClosed(UIScreenBase closedScreen, bool ignoreTweens)
    {
        if (openMenusQueues.TryPeek(out UIScreenBase currentScreen))
            openMenusQueues.Pop();
        if (openMenusQueues.TryPeek(out UIScreenBase lastScreen))
            lastScreen.Show(ignoreTweens);
        else
        {
            this.ExitedUI();
            uiOpenedByExternalMenu = false;
        }
    }

    private void OnScreenShow(UIScreenBase showedScreen, bool ignoreTweens)
    {

    }
    private void OnScreenHide(UIScreenBase hidedScreen, bool ignoreTweens)
    {

    }

    /// <summary> <para>
    /// Handles the selection of buttons on menu changes. <paramref name="context"/> is used to decided which button should be selected. 
    /// </para> <para>
    /// <paramref name="context"/> Should be formated like "Context"
    /// </para> </summary>
    /// <param name="context"></param>
    public void SelectButton(string context)
    {
        GameObject currentSelected = eventSystem.currentSelectedGameObject;

        switch (context)
        {
            case "None":
                if (eventSystem != null)
                    eventSystem.SetSelectedGameObject(null);
                break;
            default:
                Debug.LogError(context + " was not found in switch statement.");
                break;
        }

        lastSelected = currentSelected;
    }

    public void ShowGameOverScreen()
    {
        gameoverMenu.SetActive(true);
    }

    public void HideGameOverScreen()
    {
        gameoverMenu.SetActive(false);
    }


    public void FadeAllHUD(bool fadeIn, float time = .2f)
    {
        FadeInGameHUD(fadeIn, time);
        FadeTutoHUD(fadeIn, time);
    }

    public void FadeInGameHUD(bool fadeIn, float time = .2f)
    {
        hudContainer.LeanAlpha(fadeIn ? 1 : 0, time).setIgnoreTimeScale(true);
    }
    public void FadeTutoHUD(bool fadeIn, float time = .2f)
    {
        tutoHUD.LeanAlpha(fadeIn ? 1 : 0, time).setIgnoreTimeScale(true);
    }

    #endregion

    public void SetMinimapActiveState(bool active)
    {
        minimap.SetActive(active);
        minimapButton.SetActive(active);
    }

    public void FadeScreen(bool fadeOut, float time = .5f)
    {
        if (currentScreenFade != null)
        {
            LeanTween.cancel(currentScreenFade.uniqueId);
            currentScreenFade = null;
        }
        if (fadeImage == null) return;

        currentScreenFade = LeanTween.alpha(fadeImage.rectTransform, fadeOut ? 1 : 0, time).setIgnoreTimeScale(true).setOnComplete(() => currentScreenFade = null);
    }
    public void FadeScreen(bool fadeOut, Action onCompleteAction, float time = .5f)
    {
        if (currentScreenFade != null)
        {
            LeanTween.cancel(currentScreenFade.uniqueId);
            currentScreenFade = null;
        }
        if (fadeImage == null) return;

        if (onCompleteAction == null)
        {
            FadeScreen(fadeOut, time);
            return;
        }
        onCompleteAction += () => currentScreenFade = null;
        currentScreenFade = LeanTween.alpha(fadeImage.rectTransform, fadeOut ? 1 : 0, time).setIgnoreTimeScale(true).
            setOnComplete(onCompleteAction);
    }
    public void InstantFadeScreen(bool fadeOut)
    {
        if (fadeImage == null) return;

        Color c = fadeImage.color;
        c.a = fadeOut ? 1 : 0;
        fadeImage.color = c;
    }

    public void SetBlackBars(bool appear, float time = 1f)
    {
        blackBars?.SetBlackBars(appear, time);
    }

    public Sprite GetBasePortrait(GameManager.E_CharactersNames character)
    {
        foreach (var item in characterPortrait)
        {
            if (item.characterName.Equals(character)) return item.characterPortraitsByHP[0].portrait;
        }

        return characterPortrait[0].characterPortraitsByHP[0].portrait;
    }

    public void PlayerQuitLobby(int id)
    {
    }

    public void SkipTutoToggle(bool value) => DataKeeper.Instance.skipTuto = value;

#if UNITY_EDITOR

    public void EDITOR_PopulateInspectorOpenedMenus()
    {
        EDITOR_openMenusQueues.Clear();
        foreach (var item in openMenusQueues)
        {
            EDITOR_openMenusQueues.Add(item);
        }
    }

#endif
}

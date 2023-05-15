using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text;
using System;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            //if (instance == null) Debug.LogError("UIManager instance was not found.");

            return instance;
        }
    }

    [SerializeField] private EventSystem eventSystem;

    #region Buttons & Menus

    [SerializeField] private Button firstSelectedButton_MainMenu;
    [SerializeField] private Button firstSelectedButton_Pause;
    [SerializeField] private Button firstSelectedButton_Win;
    [SerializeField] private Button firstSelectedButton_Gameover;
    [SerializeField] private Button firstSelectedButton_Shop;
    [SerializeField] private Button firstSelectedButton_Lobby;
    [SerializeField] private Slider firstSelectedButton_Options;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private WinScreen winScreen;
    [SerializeField] private GameObject gameoverMenu;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject shopContentMenu;
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

    #endregion

    #region UI Refs

    [SerializeField] private Scrollbar shopBar;

    [SerializeField] private Button optionButton_Pause;
    [SerializeField] private Button mainMenuButton_Pause;

    [SerializeField] private PlayerHUD[] playerHUDs;
    [SerializeField] private GameObject minimap;

    [SerializeField] private Image[] blackBars;

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

    private Stack<GameObject> openMenusQueues = new Stack<GameObject>();

    public delegate void D_CloseMenu();
    public D_CloseMenu D_closeMenu;

    public delegate void D_ExitPause();
    public D_ExitPause D_exitPause;

#if UNITY_EDITOR
    [SerializeField] private List<GameObject> EDITOR_openMenusQueues;
#endif

    private GameObject lastSelected;

    private Scrollbar currentVerticalScrollbar;
    private Scrollbar currentHorizontalScrollbar;

    #region Getters

    public PlayerPanelsManager PanelsManager { get => panelsManager; }

    public GameObject ShopMenu { get => shopMenu; }

    public GameObject ShopContentMenu { get => shopContentMenu; }

    public Stack<GameObject> OpenMenusQueues { get => openMenusQueues; }

    public PlayerHUD[] PlayerHUDs { get => playerHUDs; }

    public GameObject KeycardContainer { get => keycardsContainer; }

    public TextMeshProUGUI KeycardsCounters { get => keycardsCounter; }

    public Scrollbar CurrentHorizontalScrollbar { get => currentHorizontalScrollbar; }

    public Scrollbar CurrentVerticalScrollbar { get => currentVerticalScrollbar; }

    #endregion

    public const int maxPBImagesByRows = 6;

    public const float scrollbarSensibility = .1f;

    private bool firstGameStatePassFlag = false;

    #region Awake / Start / Updates

    private void Awake()
    {
        instance = this;

        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainScene))
        {
            GameManager.Instance.D_onPlayerIsSetup += SetPlayerColliderArray;
        }

        if (hudContainer != null) hudContainer.alpha = 0;
    }

    private void Start()
    {
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu))
        {   
        }
        else
        {
            hudRect = localHUD.GetComponent<RectTransform>();

            if (DataKeeper.Instance.skipTuto || DataKeeper.Instance.alreadyPlayedTuto) FadeAllHUD(true);
            else tutoHUD.gameObject.SetActive(true);

            keycardsContainer.SetActive(false);

            moneyCounter.text = "x " + 0;
        }
    }

    private void LateUpdate()
    {
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu) == false)
            CheckIfPlayerOrBossAreCoveredByHUD();
    } 

    #endregion

    public void SetOptionsState()
    {
        OPTION_DashOnMovementsToggle?.SetIsOnWithoutNotify(GameManager.OPTION_DashToMouse);
    }

    #region Players HUD

    private void SetPlayerColliderArray(int idx)
    {
        // TEMP
        PlayerCharacter player = DataKeeper.Instance.GetPlayerFromIndex(idx);
        playersColliders[idx] = player.HUDBoundsTrigger;
    }

    public void AddBossCollider(Collider2D collider)
    {
        bossesColliders.Add(collider);
    }
    public void RemoveBossCollider(Collider2D collider)
    {
        bossesColliders.Remove(collider);
    }

    public void AddMakersInCollidersArray(BoxCollider2D[] collider2Ds)
    {
        for (int i = GameManager.Instance.PlayersCount + 3; i < collider2Ds.Length + 4; i++)
        {
            playersColliders[i] = collider2Ds[i - (GameManager.Instance.PlayersCount + 3)];
        }
    }

    public float right = 1.3f;
    public float top = 1;
    public float bottom = 1.3f;

    private void CheckIfPlayerOrBossAreCoveredByHUD()
    {
        CheckHUD(hudContainerRect, FadeHUD);

        if (BossHUDManager.Instance.GetBossHUDsCount() > 0)
            CheckHUD(BossHUDManager.Instance.hudFadeTarget, FadeBossHUD, right, top, bottom);
    }

    private void CheckHUD(RectTransform rt, Action<bool> fadeHUDAction, float rightOffsetMul = 1.3f, float topOffsetMul = 1, float bottomOffsetMul = 1.3f)
    {
        bool fadeHUD = false;
        foreach (var item in playersColliders)
        {
            if (item == null) continue;
            if (item.isActiveAndEnabled == false) continue;

            fadeHUD = CheckCollider(item, rt, fadeHUDAction, rightOffsetMul, topOffsetMul, bottomOffsetMul);
            if (fadeHUD) return;
        }

        if (GameManager.Instance.currentAliveBossesCount > 0)
        {
            foreach (var item in bossesColliders)
            {
                if (item == null) continue;
                if (item.isActiveAndEnabled == false) continue;

                fadeHUD = CheckCollider(item, rt, fadeHUDAction, rightOffsetMul, topOffsetMul, bottomOffsetMul);
                if (fadeHUD) return;
            }
        }

        if (!fadeHUD)
            fadeHUDAction(false);
    }

    private bool CheckCollider(Collider2D collider, RectTransform rt, Action<bool> fadeHUDAction, float rightOffsetMul = 1.3f, float topOffsetMul = 1, float bottomOffsetMul = 1.3f)
    {
        Vector2 point = RectTransformUtility.WorldToScreenPoint(Camera.main, collider.transform.position);

        Rect rect = rt.rect;

        float leftSide = rt.anchoredPosition.x;
        float rightSide = rt.anchoredPosition.x + (rect.width * rightOffsetMul);
        float topSide = rt.anchoredPosition.y + (rect.height * topOffsetMul);
        float bottomSide = rt.anchoredPosition.y - (rect.height * bottomOffsetMul);

        if (point.x >= leftSide &&
           point.x <= rightSide &&
           point.y >= bottomSide &&
           point.y <= topSide)
        {
            fadeHUDAction(true);
            return true;
        }

        return false;
    }

    private void FadeHUD(bool makeTransparent)
    {
        if (isHUDTransparent == makeTransparent) return;
        isHUDTransparent = makeTransparent;

        LeanTween.alphaCanvas(localHUD.GetComponent<CanvasGroup>(), makeTransparent ? hudTransparencyValue : 1, hudTransparencyTime);
        LeanTween.alphaCanvas(keycardsContainer.GetComponent<CanvasGroup>(), makeTransparent ? hudTransparencyValue : 1, hudTransparencyTime);
    }

    private void FadeBossHUD(bool makeTransparent)
    {
        if (isBossHUDTransparent == makeTransparent) return;
        isBossHUDTransparent = makeTransparent;

        LeanTween.alphaCanvas(BossHUDManager.Instance.hudContainer, makeTransparent ? hudTransparencyValue : 1, hudTransparencyTime);
    }

    public void UpdateKeycardsCounter(int idx)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(GameManager.Instance.AcquiredCards);
        sb.Append(" / ");
        sb.Append(GameManager.Instance.NeededCards);
        KeycardsCounters.text = sb.ToString();

        if (GameManager.Instance.AcquiredCards == 0 || idx < 0 || idx >= HudKeycards.Length) return;

        Image keycard = HudKeycards[idx];
        keycard.color = Color.white;

        LeanTween.scale(keycard.rectTransform, new Vector2(1.6f, 1.6f), .5f).setEase(LeanTweenType.easeInSine);
        LeanTween.rotate(keycard.rectTransform, new Vector3(0, 0, 3f), .5f).setEase(LeanTweenType.easeInSine).setOnComplete(() =>
        {
            LeanTween.scale(keycard.rectTransform, Vector2.one, .5f).setEase(LeanTweenType.easeOutSine);
            LeanTween.rotate(keycard.rectTransform, new Vector3(0, 0, -3f), .5f).setEase(LeanTweenType.easeOutSine);
        });
    }

    public void UpdateMoney()
    {
        moneyCounter.text = "x " + PlayerCharacter.GetMoney();
    }

    #endregion

    #region Scrollbars

    public void SetCurrentVerticalScrollbar(Scrollbar bar) => currentVerticalScrollbar = bar;
    public void UnsetCurrentVerticalScrollbar() => currentVerticalScrollbar = null;

    public void SetCurrentHorizontalScrollbar(Scrollbar bar) => currentHorizontalScrollbar = bar;
    public void UnsetCurrentHorizontalScrollbar() => currentHorizontalScrollbar = null;

    public void ScrollCurrentVerticalBarDown(InputAction.CallbackContext context)
    {
        if (/*context.performed && */currentVerticalScrollbar != null) currentVerticalScrollbar.value -= scrollbarSensibility;
    }
    public void ScrollCurrentVerticalBarUp(InputAction.CallbackContext context)
    {
        if (/*context.performed && */currentVerticalScrollbar != null) currentVerticalScrollbar.value += scrollbarSensibility;
    }

    public void ScrollCurrentHorizontalBarLeft(InputAction.CallbackContext context)
    {
        if (/*context.performed && */currentHorizontalScrollbar != null) currentHorizontalScrollbar.value -= scrollbarSensibility;
    }
    public void ScrollCurrentHorizontalBarRight(InputAction.CallbackContext context)
    {
        if (/*context.performed && */currentHorizontalScrollbar != null) currentHorizontalScrollbar.value += scrollbarSensibility;
    }

    #endregion

    #region Menus Managers

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
            case "MainMenu":
                firstSelectedButton_MainMenu?.Select();
                break;

            case "Options":
                firstSelectedButton_Options?.Select();
                break;

            case "Pause":
                firstSelectedButton_Pause?.Select();
                break;

            case "Win":
                firstSelectedButton_Win?.Select();
                break;

            case "Gameover":
                firstSelectedButton_Gameover?.Select();
                break;

            case "Last":
                if (lastSelected != null)
                    eventSystem.SetSelectedGameObject(lastSelected);
                break;

            case "Shop":
                firstSelectedButton_Shop?.Select();
                break;

            case "Lobby":
                firstSelectedButton_Lobby?.Select();
                break;

            case "None":
                if (eventSystem != null)
                    eventSystem.SetSelectedGameObject(null);
                break;

            case "Context":

                switch (GameManager.Instance.GameState)
                {
                    case GameManager.E_GameState.Pause:
                        SelectButton("Pause");
                        break;

                    case GameManager.E_GameState.Win:
                        SelectButton("Win");
                        break;

                    case GameManager.E_GameState.GameOver:
                        SelectButton("Gameover");
                        break;
                }

                break;

            default:
                Debug.LogError(context + " was not found in switch statement.");
                break;
        }

        lastSelected = currentSelected;
    }

    /// <summary>
    /// Manages the windows display using <paramref name="newState"/>.
    /// </summary>
    /// <param name="newState"></param>
    public void WindowsManager(GameManager.E_GameState newState)
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

                PostproManager.Instance.SetBlurState(false);
                break;

            case GameManager.E_GameState.Pause:
                FadeAllHUD(fadeIn: false);
                PostproManager.Instance.SetBlurState(true);

                OpenMenuInQueue(pauseMenu);
                SelectButton("Pause");

                break;

            case GameManager.E_GameState.Restricted:
                break;

            case GameManager.E_GameState.Win:
                winScreen.Begin();
                break;

            case GameManager.E_GameState.GameOver:
                break;

            default:
                Debug.LogError(newState + "not found in switch statement.");
                break;
        }
    }

    public void ShowGameOverScreen()
    {
        gameoverMenu.SetActive(true);
        SelectButton("Gameover");
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

    public void OpenMenuInQueue(GameObject newMenu)
    {
        newMenu.SetActive(true);
        openMenusQueues.Push(newMenu);
    }
    public void CloseYoungerMenu()
    {
        D_closeMenu?.Invoke();

        GameObject closedMenu = null;
        if (openMenusQueues.Count > 0)
            closedMenu = openMenusQueues.Pop();

        if (closedMenu != null)
        {
            if (closedMenu.Equals(shopMenu))
                CloseShopFromBack();

            closedMenu.SetActive(false);
        }

        if (openMenusQueues.Count > 0)
            openMenusQueues.Peek().SetActive(true);
        else
        {
            switch (GameManager.Instance.GameState)
            {
                case GameManager.E_GameState.Pause:
                    GameManager.Instance.GameState = GameManager.E_GameState.InGame;
                    D_exitPause?.Invoke();
                    break;

                case GameManager.E_GameState.InGame:
                    FadeAllHUD(fadeIn: true);
                    PostproManager.Instance.SetBlurState(false);

                    break;

                case GameManager.E_GameState.MainMenu:
                    mainMenu_mainPanel.SetActive(true);
                    break;
            }
        }

        SelectButton("Last");
    }

    public void OpenShop(bool makeFade = true)
    {
        OpenMenuInQueue(shopMenu);
        SelectButton("Shop");

        if (makeFade) FadeAllHUD(fadeIn: false);
        PostproManager.Instance.SetBlurState(true);

        //SetCurrentVerticalScrollbar(shopBar);

        PlayersManager.Instance.SetAllPlayersControlMapToUI();
    }

    public void CloseShop()
    {
        CloseYoungerMenu();
    } 
    private void CloseShopFromBack()
    {
        FadeInGameHUD(fadeIn: true);
        PostproManager.Instance.SetBlurState(false);

        PlayersManager.Instance.SetAllPlayersControlMapToInGame();
        //UnsetCurrentVerticalScrollbar();

        GameManager.Instance.GameState = GameManager.E_GameState.InGame;

        GameManager.Instance.GetShop.CloseShopFromUI();
    }

    #endregion

    public void SetMinimapActiveState(bool active)
    {
        minimap.SetActive(active);
    }

    public void FadeScreen(bool fadeOut, float time = .5f)
    {
        if (fadeImage == null) return;

        LeanTween.alpha(fadeImage.rectTransform, fadeOut ? 1 : 0, time).setIgnoreTimeScale(true);
    }
    public void FadeScreen(bool fadeOut, Action onCompleteAction, float time = .5f)
    {
        if (fadeImage == null) return;

        if (onCompleteAction == null)
        {
            FadeScreen(fadeOut, time);
            return;
        }
        LeanTween.alpha(fadeImage.rectTransform, fadeOut ? 1 : 0, time).setIgnoreTimeScale(true).
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
        foreach (var item in blackBars)
        {
            LeanTween.value(item.fillAmount, appear ? 1 : 0, time).setIgnoreTimeScale(true).setOnUpdate( 
            (float val) =>
            {
                item.fillAmount = val;
            });
        }
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

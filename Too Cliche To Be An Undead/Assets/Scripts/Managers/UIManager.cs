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
    [SerializeField] private GameObject gameoverMenu;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject shopContentMenu;
    [SerializeField] private GameObject localHUD;
    [SerializeField] private GameObject mainMenu_mainPanel;

    [SerializeField] private Image fadeImage;

    [SerializeField] private Toggle OPTION_DashOnMovementsToggle;
    [SerializeField] private Toggle skipTutorialToggle;

    #endregion

    #region HUD

    private RectTransform hudRect;
    private bool isHUDTransparent = false;

    [SerializeField] private float hudTransparencyValue = .3f;
    [SerializeField] private float hudTransparencyTime = .3f;

    public Collider2D[] playersColliders = new Collider2D[8];

    #endregion

    #region UI Refs

    [SerializeField] private Scrollbar pbContainerBar;
    [SerializeField] private Scrollbar shopBar;

    [SerializeField] private Button optionButton_Pause;
    [SerializeField] private Button mainMenuButton_Pause;

    [SerializeField] private PlayerHUD[] playerHUDs;
    [SerializeField] private GameObject minimap;

    [SerializeField] private Button[] pbContainersButtons;
    [SerializeField] private GameObject[] pbContainers;

    [SerializeField] private Image[] blackBars;

    [SerializeField] private GameObject keycardsContainer;
    [SerializeField] private RectTransform keycardsContainerRect;
    [SerializeField] private TextMeshProUGUI keycardsCounter;

    [SerializeField] private CanvasGroup hudContainer;
    [SerializeField] private CanvasGroup dialogueContainer;

    #endregion

    #region Players UI Components

    [System.Serializable]
    public struct PlayerHUD
    {
        public GameObject container;
        public Image portrait;
        public Image hpBar;
        public TextMeshProUGUI hpText;

        public Image skillContainer;
        public Image skillThumbnail;

        public Image dashContainer;
        public Image dashThumbnail;
    }

    [field: SerializeField] public Image skillButton_active { get; private set; }
    [field: SerializeField] public Image skillButton_inactive { get; private set; }
    [field: SerializeField] public Image dashButton_active { get; private set; }
    [field: SerializeField] public Image dashButton_inactive { get; private set; }

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

    private List<PBThumbnail> pbThumbnails = new List<PBThumbnail>();

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

    public GameObject[] PbContainers { get => pbContainers; }

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

        if (hudContainer != null) hudContainer.alpha = 0;
    }

    private void Start()
    {
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu))
        {   
            skipTutorialToggle.SetIsOnWithoutNotify(DataKeeper.Instance.skipTuto);
        }
        else
        {
            SetPlayersCollidersArray();
            if (DataKeeper.Instance.playersDataKeep.Count > 1)
            {
                for (int i = 0; i < DataKeeper.Instance.playersDataKeep.Count; i++)
                {
                    Sprite im = GetBasePortrait(DataKeeper.Instance.playersDataKeep[i].character);
                    pbContainersButtons[i].image.sprite = im;
                }
            }

            if (DataKeeper.Instance.skipTuto || DataKeeper.Instance.alreadyPlayedTuto) FadeAllHUD(true);

            keycardsContainer.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu) == false)
            CheckIfPlayerIsCoveredByHUD();
    } 

    #endregion

    public void SetOptionsState()
    {
        OPTION_DashOnMovementsToggle?.SetIsOnWithoutNotify(GameManager.OPTION_DashToMouse);
    }

    #region Players HUD

    private void SetPlayersCollidersArray()
    {
        int playersCount = GameManager.Instance.PlayersCount;

        hudRect = localHUD.GetComponent<RectTransform>();

        playersColliders = new Collider2D[8];

        for (int i = 0; i < playersCount; i++)
        {
            // TODO : faire un meilleur moyen de récup les colliders psq là c'est dégueulasse
            playersColliders[i] = DataKeeper.Instance.playersDataKeep[i].playerInput.GetComponentInParent<PlayerCharacter>().GetSprite.GetComponent<BoxCollider2D>();
        }
    }

    public void AddMakersInCollidersArray(BoxCollider2D[] collider2Ds)
    {
        for (int i = GameManager.Instance.PlayersCount + 3; i < collider2Ds.Length + 4; i++)
        {
            playersColliders[i] = collider2Ds[i - (GameManager.Instance.PlayersCount + 3)];
        }
    }

    private void CheckIfPlayerIsCoveredByHUD()
    {
        foreach (var item in playersColliders)
        {
            if (item == null) continue;
            if (item.isActiveAndEnabled == false) continue;

            Vector3 boundsMin = RectTransformUtility.WorldToScreenPoint(Camera.main, item.bounds.min);
            Vector3 boundsMax = RectTransformUtility.WorldToScreenPoint(Camera.main, item.bounds.max);

            for (int i = 0; i < GameManager.Instance.PlayersCount; i++)
            {
                bool minP = RectTransformUtility.RectangleContainsScreenPoint(playerHUDs[i].portrait.rectTransform, boundsMin);
                bool maxP = RectTransformUtility.RectangleContainsScreenPoint(playerHUDs[i].portrait.rectTransform, boundsMax);

                if (minP && maxP)
                {
                    FadeHUD(true);
                    return;
                }
            }

            bool minK = RectTransformUtility.RectangleContainsScreenPoint(keycardsContainerRect, boundsMin);
            bool maxK = RectTransformUtility.RectangleContainsScreenPoint(keycardsContainerRect, boundsMax);

            if (minK && maxK)
            {
                FadeHUD(true);
                return;
            }
        }

        FadeHUD(false);
    }

    private void FadeHUD(bool makeTransparent)
    {
        if (isHUDTransparent == makeTransparent) return;
        isHUDTransparent = makeTransparent;

        LeanTween.alphaCanvas(localHUD.GetComponent<CanvasGroup>(), makeTransparent ? hudTransparencyValue : 1, hudTransparencyTime);
        LeanTween.alphaCanvas(keycardsContainer.GetComponent<CanvasGroup>(), makeTransparent ? hudTransparencyValue : 1, hudTransparencyTime);
    }

    public void SetDashIconState(int playerID, bool active)
    {
        playerHUDs[playerID].dashContainer = active ? dashButton_active : dashButton_inactive;
    }

    public void SetSkillIconState(int playerID, bool active)
    {
        playerHUDs[playerID].skillContainer = active ? skillButton_active : skillButton_inactive;
    }

    public void UpdateKeycardsCounter()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(GameManager.AcquiredCards);
        sb.Append(" / ");
        sb.Append(GameManager.NeededCards);
        KeycardsCounters.text = sb.ToString();

        if (GameManager.AcquiredCards != 0)
        {
            LeanTween.scale(KeycardContainer, new Vector2(1.6f, 1.6f), .5f).setEase(LeanTweenType.easeInSine);
            LeanTween.rotate(KeycardContainer, new Vector3(0, 0, 3f), .5f).setEase(LeanTweenType.easeInSine).setOnComplete(() =>
            {
                LeanTween.scale(KeycardContainer, Vector2.one, .5f).setEase(LeanTweenType.easeOutSine);
                LeanTween.rotate(KeycardContainer, new Vector3(0, 0, -3f), .5f).setEase(LeanTweenType.easeOutSine);
            });
        }
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

                if (DataKeeper.Instance.playersDataKeep.Count > 1)
                {
                    for (int i = 0; i < DataKeeper.Instance.playersDataKeep.Count; i++)
                    {
                        pbContainersButtons[i].gameObject.SetActive(false);
                    }
                }

                if (!firstGameStatePassFlag) firstGameStatePassFlag = true;
                else if (!GameManager.Instance.IsInTutorial) FadeAllHUD(fadeIn: true);

                PostproManager.Instance.SetBlurState(false);
                break;

            case GameManager.E_GameState.Pause:
                FadeAllHUD(fadeIn: false);
                PostproManager.Instance.SetBlurState(true);

                OpenMenuInQueue(pauseMenu);
                SelectButton("Pause");
                SetCurrentHorizontalScrollbar(pbContainerBar);

                if (DataKeeper.Instance.playersDataKeep.Count > 1)
                {
                    for (int i = 0; i < DataKeeper.Instance.playersDataKeep.Count; i++)
                    {
                        pbContainersButtons[i].gameObject.SetActive(true);
                    }
                }
                break;

            case GameManager.E_GameState.Restricted:
                break;

            case GameManager.E_GameState.Win:
                winMenu.SetActive(true);
                SelectButton("Win");
                break;

            case GameManager.E_GameState.GameOver:
                gameoverMenu.SetActive(true);
                SelectButton("Gameover");
                break;

            default:
                Debug.LogError(newState + "not found in switch statement.");
                break;
        }
    }

    public void FadeAllHUD(bool fadeIn, float time = .2f)
    {
        if (hudContainer == null) return;
        hudContainer.LeanAlpha(fadeIn ? 1 : 0, time).setIgnoreTimeScale(true);
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
            {
                FadeAllHUD(fadeIn: true);
                PostproManager.Instance.SetBlurState(false);

                PlayersManager.Instance.SetAllPlayersControlMapToInGame();

                GameManager.Instance.GetShop.SetIsShopOpen(false);

                GameManager.Instance.GameState = GameManager.E_GameState.InGame;
            }
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

    public void OpenShop()
    {
        OpenMenuInQueue(shopMenu);
        SelectButton("Shop");

        FadeAllHUD(fadeIn: false);
        PostproManager.Instance.SetBlurState(true);

        //SetCurrentVerticalScrollbar(shopBar);

        PlayersManager.Instance.SetAllPlayersControlMapToUI();
    }

    public void CloseShop()
    {
        CloseYoungerMenu();

        FadeAllHUD(fadeIn: true);
        PostproManager.Instance.SetBlurState(false);

        PlayersManager.Instance.SetAllPlayersControlMapToInGame();
        //UnsetCurrentVerticalScrollbar();

        GameManager.Instance.GameState = GameManager.E_GameState.InGame;
    } 

    #endregion

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

    public void AddPBToContainer(SCRPT_PB pb, int playerIdx = 0)
    {
        // create the PB and add it to the container
        PBThumbnail gO = PBThumbnail.Create(pb, pbContainers[playerIdx].transform.childCount);
        gO.transform.SetParent(pbContainers[playerIdx].transform);

        int childIdx = pbContainers[playerIdx].transform.childCount;

        // Get the navigation component of the new PB
        Button addedPB = gO.GetComponent<Button>();
        Navigation nav = addedPB.navigation;

        nav.selectOnRight = firstSelectedButton_Pause;

        // if the added PB is the first, set the navigation with the pause buttons
        if (pbContainers[playerIdx].transform.childCount <= 1)
        {
            nav.selectOnLeft = firstSelectedButton_Pause;

            Navigation fsbNav = firstSelectedButton_Pause.navigation;
            fsbNav.selectOnLeft = addedPB;
            fsbNav.selectOnRight = addedPB;
            firstSelectedButton_Pause.navigation = fsbNav;

            Navigation obpNav = optionButton_Pause.navigation;
            obpNav.selectOnLeft = addedPB;
            obpNav.selectOnRight = addedPB;
            optionButton_Pause.navigation = obpNav;

            Navigation mmbpNav = mainMenuButton_Pause.navigation;
            mmbpNav.selectOnLeft = addedPB;
            mmbpNav.selectOnRight = addedPB;
            mainMenuButton_Pause.navigation = mmbpNav;
        }
        else // set the navigation with the neighbours
        {
            // Get the left neighbour of the new PB
            Button leftNeighbour = pbContainers[playerIdx].transform.GetChild(childIdx - 2).GetComponent<Button>();

            // Add the new PB as the right nav of neighbour
            Navigation neighbourNav = leftNeighbour.navigation;

            // if neighbour is not last of row
            if ((childIdx - 1) % maxPBImagesByRows != 0 || childIdx - 2 <= 0)
                neighbourNav.selectOnRight = addedPB;
            else 
                neighbourNav.selectOnRight = firstSelectedButton_Pause;

            leftNeighbour.navigation = neighbourNav;

            nav.selectOnLeft = leftNeighbour;
        }

        // set the vertical navigation if there is more than one row
        if (childIdx > maxPBImagesByRows)
        {
            Button upNeighbour = pbContainers[playerIdx].transform.GetChild(childIdx - (maxPBImagesByRows + 1)).GetComponent<Button>();

            Navigation neighbourNav = upNeighbour.navigation;
            neighbourNav.selectOnDown = addedPB;
            upNeighbour.navigation = neighbourNav;

            nav.selectOnUp = upNeighbour;
        }
        addedPB.navigation = nav;

        pbThumbnails.Add(gO);
    }

    public Sprite GetBasePortrait(GameManager.E_CharactersNames character)
    {
        foreach (var item in characterPortrait)
        {
            if (item.characterName.Equals(character)) return item.characterPortraitsByHP[0].portrait;
        }

        return characterPortrait[0].characterPortraitsByHP[0].portrait;
    }

    public void PlayerLeftArrowOnPanel(int id)
    {
        panelsManager?.GetPlayerPanels[id].ChangePreset(true);
    }

    public void PlayerRightArrowOnPanel(int id)
    {
        panelsManager?.GetPlayerPanels[id].ChangePreset(false);
    }

    public void PlayerQuitLobby(int id)
    {
        panelsManager?.RemovePanel(id);
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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null) Debug.LogError("UIManager instance was not found.");

            return instance;
        }
    }

    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private Button firstSelectedButton_MainMenu;
    [SerializeField] private Button firstSelectedButton_Pause;
    [SerializeField] private Button firstSelectedButton_Win;
    [SerializeField] private Button firstSelectedButton_Gameover;
    [SerializeField] private Button firstSelectedButton_Shop;
    [SerializeField] private Slider firstSelectedButton_Options;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pbContainer;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject gameoverMenu;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject shopContentMenu;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject mainMenu_mainPanel;

    [SerializeField] private PlayerPanelsManager panelsManager;

    public PlayerPanelsManager PanelsManager { get => panelsManager; }

    public GameObject ShopMenu { get => shopMenu; }

    public GameObject ShopContentMenu { get => shopContentMenu; }

    private Stack<GameObject> openMenusQueues = new Stack<GameObject>();
    public Stack<GameObject> OpenMenusQueues { get => openMenusQueues; }

    private List<PBThumbnail> pbThumbnails = new List<PBThumbnail>();

    public delegate void D_CloseMenu();
    public D_CloseMenu D_closeMenu;

    public delegate void D_ExitPause();
    public D_ExitPause D_exitPause;

#if UNITY_EDITOR
    [SerializeField] private List<GameObject> EDITOR_openMenusQueues;
#endif

    private GameObject lastSelected;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu)) SelectButton("MainMenu");
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
                break;

            case GameManager.E_GameState.Pause:
                OpenMenuInQueue(pauseMenu);
                SelectButton("Pause");
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
                hud.SetActive(true);
                GameManager.Player1Ref.SwitchControlMapToInGame();
                GameManager.Instance.GetShop.SetIsShopOpen(false);
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
                    hud.SetActive(true);
                    break;

                case GameManager.E_GameState.MainMenu:
                    mainMenu_mainPanel.SetActive(true);
                    break;
            }
        }

        SelectButton("Last");
    }

    public void AddPBToContainer(SCRPT_PB pb)
    {
        PBThumbnail gO = PBThumbnail.Create(pb);
        gO.transform.SetParent(pbContainer.transform);
        pbThumbnails.Add(gO);
    }

    public void OpenShop()
    {
        OpenMenuInQueue(shopMenu);
        SelectButton("Shop");
        hud.SetActive(false);
        GameManager.Player1Ref.SwitchControlMapToUI();
    }

    public void CloseShop()
    {
        CloseYoungerMenu();
        hud.SetActive(true);
        GameManager.Player1Ref.SwitchControlMapToInGame();
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

using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem.Users;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerInputsManager : PersistentSingleton<PlayerInputsManager>
{
    public const string ACTIONMAP_INGAME = "InGame";
    public const string ACTIONMAP_UI = "UI";
    public const string ACTIONMAP_DIALOGUE = "Dialogue";

    public const string SCHEME_KEYBOARD = "Keyboard&Mouse";
    public const string SCHEME_GAMEPAD = "Gamepad";

    public static int PlayersCount
    {
        get
        {
            if (instance == null) return -1;
            if (instance.PlayerInputsList == null) return -1;
            return instance.PlayerInputsList.Count;
        }
    }
    [field: SerializeField] public List<PlayerInputs> PlayerInputsList { get; private set; }
    public static PlayerInputs P1Inputs
    {
        get
        {
            if (instance == null) return null;
            if (instance.PlayerInputsList.Count > 0) return instance.PlayerInputsList[0];
            else return null;
        }
    }
    [SerializeField] private PlayerInputs inputsPF;


    public enum E_Devices
    {
        Keyboard,
        Xbox,
        DualShock,
        Switch,
        None,
    }

    public static E_Devices CurrentPlayer1Device { get; private set; }
    public static void SetPlayer1Device(E_Devices device)
        => CurrentPlayer1Device = device;

    protected override void EventsSubscriber()
    {
        base.EventsSubscriber();
        
        PlayerPanelsManagerEvents.OnPanelsActive += PanelsActive;
        PlayerPanelsManagerEvents.OnPanelsInactive += PanelsInactive;
        PlayerInputsEvents.OnPlayerInputsCreated += OnPlayerInputsCreated;
        PlayerInputsEvents.OnPlayerInputsDestroyed += OnPlayerInputsDestroyed;
        GameManagerEvents.OnGameStateChange += OnGameStateChange;
    }

    protected override void EventsUnSubscriber()
    {
        base.EventsUnSubscriber();

        PlayerPanelsManagerEvents.OnPanelsActive -= PanelsActive;
        PlayerPanelsManagerEvents.OnPanelsInactive -= PanelsInactive;
        PlayerInputsEvents.OnPlayerInputsCreated -= OnPlayerInputsCreated;
        PlayerInputsEvents.OnPlayerInputsDestroyed -= OnPlayerInputsDestroyed;
        GameManagerEvents.OnGameStateChange -= OnGameStateChange;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu)) Initialize();
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainScene)) CheckIfP1Exist();
    }

    protected override void OnSceneUnloaded(Scene scene)
    {
    }

    protected override void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        base.Awake();
    }

    private void Start()
    {
        GiveUnpairedDevicesToP1();
    }

    private void Initialize()
    {
        if (PlayerInputsList.Count > 1) 
        {
            for (int i = 1; i < PlayerInputsList.Count; i++)
                Destroy(PlayerInputsList[i].gameObject);
            return;
        }
        CheckIfP1Exist();
    }

    private void CheckIfP1Exist()
    {
        if (P1Inputs == null)
        {
            PlayerInputsList = new List<PlayerInputs>();
            PlayerInputs p1Inputs = inputsPF?.Create();
        }
    }

    public void OnPlayerJoined(PlayerInput player)
    {

    }

    public void OnPlayerLeft(PlayerInput player)
    {

    }

    private void OnGameStateChange(GameManager.E_GameState state)
    {
        switch (state)
        {
            case GameManager.E_GameState.MainMenu:
                P1Inputs.SwitchControlMapToUI();
                break;

            case GameManager.E_GameState.InGame:
                SwitchAllControlMapsToIG();
                break;

            case GameManager.E_GameState.Pause:
                SwitchAllControlMapsToUI();
                break;

            case GameManager.E_GameState.Restricted:
                SwitchAllControlMapsToUI();
                break;

            case GameManager.E_GameState.Win:
                SwitchAllControlMapsToUI();
                break;

            case GameManager.E_GameState.GameOver:
                SwitchAllControlMapsToUI();
                break;
        }
    }

    #region ControlMaps

    private void PanelsActive() => PlayerInputsEvents.OnStart += OnAskedJoin;
    private void PanelsInactive() => PlayerInputsEvents.OnStart -= OnAskedJoin;

    private void SwitchAllControlMapsToUI()
    {
        foreach (var item in PlayerInputsList)
            item.SwitchControlMapToUI();
    }
    private void SwitchAllControlMapsToIG()
    {
        foreach (var item in PlayerInputsList)
            item.SwitchControlMapToInGame();
    }
    private void SwitchAllControlMapsToDialogue()
    {
        foreach (var item in PlayerInputsList)
            item.SwitchControlMapToDialogue();
    }
    private void SwitchAllControlMapsTo(string map)
    {
        foreach (var item in PlayerInputsList)
            item.SwitchControlMap(map);
    }

    #endregion

    #region PlayerInputsC&D
    private void OnPlayerInputsCreated(PlayerInputs inputs)
    {
        PlayerInputsList.Add(inputs);
        inputs.ChangeIndex(PlayerInputsList.Count - 1);
    }
    private void OnPlayerInputsDestroyed(int idx)
    {
        if (PlayerInputsList.Count <= idx) return; 
        PlayerInputsList.RemoveAt(idx);
        for (int i = 0; i < PlayerInputsList.Count; i++)
        {
            PlayerInputsList[i].ChangeIndex(i);
        }
        GiveUnpairedDevicesToP1();
        this.EndedChangingIndexes();
    } 
    #endregion

    private void OnAskedJoin(InputAction.CallbackContext context, PlayerInputs inputs)
    {
        if (inputs.InputsID > 0)
        {
            foreach (var item in PlayerInputsList)
            {
                if (item == inputs)
                {
                    Destroy(inputs.gameObject);
                    GiveUnpairedDevicesToP1();
                    return; 
                }
            }
        }
        if (!context.performed) return;
        if (PlayerInputsList.Count >= PlayersManager.MAX_PLAYERS) return;

        if (GameManager.Instance.GameState != GameManager.E_GameState.MainMenu) return;

        var usedDevice = context.control.device;

        P1Inputs.Input.user.UnpairDevices();
        for (int i = 1; i < PlayerInput.all.Count; i++)
        {
            PlayerInput playerInput = PlayerInput.all[i];
            if (playerInput.devices.Contains(usedDevice))
            {
                if (GameManager.Instance.allowQuitLobby == false)
                {
                    GiveUnpairedDevicesToP1();
                    return;
                }
                playerInput.user.UnpairDevices();

                GiveUnpairedDevicesToP1();
                return;
            }
        }

        PlayerInputs newInputs = inputsPF?.Create();
        InputUser.PerformPairingWithDevice(usedDevice, newInputs.Input.user, InputUserPairingOptions.UnpairCurrentDevicesFromUser);

        GiveUnpairedDevicesToP1();
    }

    private void GiveUnpairedDevicesToP1()
    {
        if (P1Inputs == null) return;
        InputUser p1 = P1Inputs.Input.user;
        List<InputDevice> unpairedDevices = new List<InputDevice>();
        foreach (var item in InputUser.GetUnpairedInputDevices())
        {
            unpairedDevices.Add(item);
        }
        foreach (var item in unpairedDevices)
        {
            InputUser.PerformPairingWithDevice(item, p1);
        }
    }

    public PlayerInputs GetPlayerInputs(int idx)
    {
        if (PlayerInputsList.Count <= idx) return null;
        return PlayerInputsList[idx];
    }
}
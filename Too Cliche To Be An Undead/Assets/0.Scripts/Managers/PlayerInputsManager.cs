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
    private List<InputDevice> knownDevices = new List<InputDevice>();
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
        DialogueManagerEvents.OnStartDialogue += OnDialogueStarted;
        DialogueManagerEvents.OnEndDialogue += OnDialogueEnded;
        ShopEvents.OnOpenShop += OnOpenShop;
        ShopEvents.OnCloseShop += OnClosedShop;
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    protected override void EventsUnSubscriber()
    {
        base.EventsUnSubscriber();

        PlayerPanelsManagerEvents.OnPanelsActive -= PanelsActive;
        PlayerPanelsManagerEvents.OnPanelsInactive -= PanelsInactive;
        PlayerInputsEvents.OnPlayerInputsCreated -= OnPlayerInputsCreated;
        PlayerInputsEvents.OnPlayerInputsDestroyed -= OnPlayerInputsDestroyed;
        GameManagerEvents.OnGameStateChange -= OnGameStateChange;
        DialogueManagerEvents.OnStartDialogue -= OnDialogueStarted;
        DialogueManagerEvents.OnEndDialogue -= OnDialogueEnded;
        ShopEvents.OnOpenShop -= OnOpenShop;
        ShopEvents.OnCloseShop -= OnClosedShop;
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu)) Initialize();
        else CheckIfP1Exist();
    }

    protected override void OnSceneUnloaded(Scene scene) { }

    protected override void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        GiveUnpairedDevicesToP1();
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange changeState)
    {
        switch (changeState)
        {
            case InputDeviceChange.Added:
                if (knownDevices.Contains(device)) return;
                knownDevices.Add(device);
                this.DeviceAdded(device);
                P1Inputs.AddDevice(device);
                this.Log($"Device {device} was added");
                break;

            case InputDeviceChange.Reconnected:
                foreach (var item in PlayerInputsList)
                {
                    Debug.Log(item);
                    if (item.MainDevice != device)
                        continue;
                    this.Log($"Device {device} was reconnected (owner : P{item.InputsID}, {item.Owner.GetCharacterName()})");

                    return;
                }
                break;

            case InputDeviceChange.Disconnected:
                this.Log($"Device {device} was disconnected");
                this.DeviceDisconnected(device);
                break;

            case InputDeviceChange.SoftReset: return;
            case InputDeviceChange.HardReset: return;
            case InputDeviceChange.Removed: return;
            case InputDeviceChange.Enabled: return;
            case InputDeviceChange.Disabled: return;

            default:
                this.Log($"Undefined device modification on {device} ({changeState})");
                break;
        }
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

    private void OnOpenShop()
        => this.SwitchAllControlMapsToUI();

    private void OnClosedShop()
        => this.SwitchAllControlMapsToIG();

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

            case GameManager.E_GameState.Cinematic:
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

    private void OnDialogueStarted(bool fromCinematic) => SwitchAllControlMapsToDialogue();
    private void OnDialogueEnded(bool fromCinematic)
    {
        if (!fromCinematic) SwitchAllControlMapsToIG();
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

        InputDevice usedDevice = context.control.device;

        P1Inputs.Input.user.UnpairDevices();
        for (int i = 1; i < PlayerInput.all.Count; i++)
        {
            PlayerInput playerInput = PlayerInput.all[i];
            if (playerInput.devices.Contains(usedDevice))
            {
                if (GameManager.Instance.AllowQuitLobby == false)
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
        if (!knownDevices.Contains(usedDevice)) knownDevices.Add(usedDevice);
        newInputs.SetMainDevice(usedDevice);

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
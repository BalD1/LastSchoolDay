using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem.Users;
using System.Linq;

public class PlayerInputsManager : PersistentSingleton<PlayerInputsManager>
{
    public const string ACTIONMAP_INGAME = "InGame";
    public const string ACTIONMAP_UI = "UI";
    public const string ACTIONMAP_DIALOGUE = "Dialogue";

    public const string SCHEME_KEYBOARD = "Keyboard&Mouse";
    public const string SCHEME_GAMEPAD = "Gamepad";

    [field: SerializeField] public List<PlayerInputs> PlayerInputsList { get; private set; }
    [field: SerializeField] public PlayerInputs P1Inputs
    {
        get
        {
            if (PlayerInputsList.Count > 0) return PlayerInputsList[0];
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
    }

    protected override void EventsUnSubscriber()
    {
        base.EventsUnSubscriber();

        PlayerPanelsManagerEvents.OnPanelsActive -= PanelsActive;
        PlayerPanelsManagerEvents.OnPanelsInactive -= PanelsInactive;
        PlayerInputsEvents.OnPlayerInputsCreated -= OnPlayerInputsCreated;
        PlayerInputsEvents.OnPlayerInputsDestroyed -= OnPlayerInputsDestroyed;
    }

    protected override void Awake()
    {
        base.Awake();
        PlayerInputsList = new List<PlayerInputs>();
        PlayerInputs p1Inputs = inputsPF?.Create();
    }

    private void Start()
    {
        GiveUnpairedDevicesToP1();
    }

    public void OnPlayerJoined(PlayerInput player)
    {

    }

    public void OnPlayerLeft(PlayerInput player)
    {

    }

    private void PanelsActive() => PlayerInputsEvents.OnStart += OnAskedJoin;
    private void PanelsInactive() => PlayerInputsEvents.OnStart -= OnAskedJoin;

    private void OnPlayerInputsCreated(PlayerInputs inputs)
    {
        PlayerInputsList.Add(inputs);
        inputs.ChangeIndex(PlayerInputsList.Count - 1);
    }
    private void OnPlayerInputsDestroyed(int idx)
    {
        PlayerInputsList.RemoveAt(idx);
        for (int i = 0; i < PlayerInputsList.Count; i++)
        {
            PlayerInputsList[i].ChangeIndex(i);
        }
        GiveUnpairedDevicesToP1();
        this.EndedChangingIndexes();
    }

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
        if (PlayerInputsList.Count >= PlayerInputManager.instance.maxPlayerCount) return;

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

    public PlayerInputs GetPlayerInputs(int idx) => PlayerInputsList[idx];
}
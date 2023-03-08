using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Rendering;

public class PlayersManager : MonoBehaviour
{
    private static PlayersManager instance;
    public static PlayersManager Instance
    {
        get
        {
            //if (instance == null) Debug.LogError("PlayersManager Instance not found.");

            return instance;
        }
    }

    [SerializeField] private PlayerPanelsManager panelsManager;

    [SerializeField] private PlayerCharacter player1;

    [ReadOnly] [SerializeField] private List<PlayerInput> playerInputs = new List<PlayerInput>();

    [SerializeField] private InputAction joinAction;
    [SerializeField] private InputAction leaveAction;

    public event Action<PlayerInput> PlayerJoined;
    public event Action<PlayerInput> PlayerLeft;

    [SerializeField] private int alivePlayersCount;
    public int AlivePlayersCount { get => alivePlayersCount; }

    [System.Serializable]
    public struct GamepadShakeData
    {
        public float lowFrequency;
        public float highFrequency;
        public float duration;
    }

    [System.Serializable]
    public struct PlayerCharacterComponents
    {
        public GameManager.E_CharactersNames character;
        public SCRPT_Dash dash;
        public SCRPT_Skill skill;
        public SCRPT_EntityStats stats;
        public Sprite sprite;
        public SCRPT_PlayersAnimData animData;
    }

    [SerializeField] private PlayerCharacterComponents[] characterComponents;
    public PlayerCharacterComponents[] CharacterComponents { get => characterComponents; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        alivePlayersCount = DataKeeper.Instance.playersDataKeep.Count;

        if (scene.name.Equals(GameManager.E_ScenesNames.MainScene)) 
            DisableActions();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake()
    {
        if (instance == null) 
            instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }

        Vector2 spawnPos = GameManager.Instance.GetSpawnPoint(0).position;
        PlayerCharacter p1 = Instantiate(GameAssets.Instance.PlayerPF, spawnPos, Quaternion.identity).GetComponent<PlayerCharacter>();
        player1 = p1;
        GameManager.Instance.SetPlayer1(p1);

        joinAction.performed += context => JoinAction(context);

        leaveAction.performed += context => LeaveAction(context);

        SceneManager.sceneLoaded += OnSceneLoaded;

        this.transform.SetParent(null);
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu)) return;

        if (player1 != null)
            CameraManager.Instance.SetTriggerParent(player1.transform);
    }

    public void SetAllPlayersControlMapToInGame()
    {
        foreach (var item in playerInputs)
        {
            item.SwitchCurrentActionMap("InGame");
        }
    }
    public void SetAllPlayersControlMapToUI()
    {
        foreach (var item in playerInputs)
        {
            item.SwitchCurrentActionMap("UI");
        }
    }
    public void SetAllPlayersControlMap(string map)
    {
        foreach (var item in playerInputs)
        {
            item.SwitchCurrentActionMap(map);
        }
    }

    public void EnableActions()
    {
        joinAction.Enable();
        leaveAction.Enable();
    }

    public void DisableActions()
    {
        joinAction.Disable();
        leaveAction.Disable();
    }

    public void SetupPanels(int idx, PlayerCharacter player)
    {
        if (panelsManager == null) panelsManager = UIManager.Instance.PanelsManager;
        if (panelsManager != null) panelsManager.JoinPanel(idx, player);
    }

    private void OnPlayerJoined(PlayerInput input)
    {
        playerInputs.Add(input);

        if (PlayerJoined != null) PlayerJoined(input);
    }

    private void OnPlayerLeft(PlayerInput input)
    {
        playerInputs.Remove(input);
        DataKeeper.Instance.RemoveData(input);

        if (PlayerLeft != null) PlayerLeft(input);
    }

    public void JoinAction(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.MainMenu) return;

        var d = context.control.device;

        GameManager.Player1Ref.Inputs.user.UnpairDevices();
        for (int i = 1; i < PlayerInput.all.Count; i++)
        {
            if (PlayerInput.all[i].devices.Contains(d)) return;
        }

        PlayerInputManager.instance.JoinPlayerFromAction(context);

        bool hasGamepad = false;
        List<InputDevice> unpairedDevices = new List<InputDevice>();

        foreach (var item in InputUser.GetUnpairedInputDevices())
        {
            unpairedDevices.Add(item);
            if (item as Gamepad != null) hasGamepad = true;
        }

        if (!hasGamepad) GameManager.Player1Ref.Inputs.SwitchCurrentControlScheme("Keyboard&Mouse");

        foreach (var item in unpairedDevices)
        {
            InputUser.PerformPairingWithDevice(item, GameManager.Player1Ref.Inputs.user);
        }
    }

    public void LeaveAction(InputAction.CallbackContext context)
    {
    }


    public void CleanInputs()
    {
        List<PlayerInput> inputsToRemove = new List<PlayerInput>();
        foreach (var item in playerInputs)
        {
            if (item == null) inputsToRemove.Add(item);
        }

        playerInputs.RemoveAll(x => inputsToRemove.Contains(x));
    }

    public PlayerCharacterComponents GetCharacterComponents(GameManager.E_CharactersNames _character)
    {
        foreach (var item in characterComponents)
        {
            if (item.character.Equals(_character)) return item;
        }

        return characterComponents[0];
    }


    public void AddAlivePlayer()
    {
        alivePlayersCount = Mathf.Clamp(alivePlayersCount + 1, 0, DataKeeper.Instance.playersDataKeep.Count);
    }
    public void RemoveAlivePlayer()
    {
        alivePlayersCount--;
        if (alivePlayersCount <= 0) GameManager.Instance.GameState = GameManager.E_GameState.GameOver;
    }

    public void DefinitiveDeath(PlayerCharacter player)
    {
        CameraManager.Instance.TG_Players.RemoveMember(player.transform);
    }
}

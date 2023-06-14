using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayersManager : MonoBehaviourEventsHandler
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

    private List<PlayerCharacter> deadPlayers;

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
        public SCRPT_PlayerAudio audioClips;
        public SCRPT_PlayersAnimData animData;
    }

    [SerializeField] private PlayerCharacterComponents[] characterComponents;
    public PlayerCharacterComponents[] CharacterComponents { get => characterComponents; }

    [field: SerializeField] public Color[] PlayersColor { get; private set; }

    public Queue<Color> playersColorQueue;

    public Transform LastDeadPlayerTransform { get; private set; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        deadPlayers = new List<PlayerCharacter>();

        PopulateColorsQueue();

        if (!GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu))
        {
            DisableActions();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }

        if (GameManager.Player1Ref == null && !GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu))
            CreateP1();

        joinAction.performed += context => JoinAction(context);

        leaveAction.performed += context => LeaveAction(context);


        this.transform.SetParent(null);
        DontDestroyOnLoad(this);

        PopulateColorsQueue();
    }

    protected override void EventsSubscriber()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        FSM_Player_Events.OnEnteredDeath += DefinitiveDeath;
        FSM_Player_Events.OnEnteredDying += RemoveAlivePlayer;
        FSM_Player_Events.OnExitedDying += AddAlivePlayer;
    }

    protected override void EventsUnSubscriber()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        FSM_Player_Events.OnEnteredDeath -= DefinitiveDeath;
        FSM_Player_Events.OnEnteredDying -= RemoveAlivePlayer;
        FSM_Player_Events.OnExitedDying -= AddAlivePlayer;
    }

    private void PopulateColorsQueue()
    {
        playersColorQueue = new Queue<Color>();

        foreach (var item in PlayersColor)
        {
            playersColorQueue.Enqueue(item);
        }
    }

    public void CreateP1()
    {
        Vector2 spawnPos = GameManager.Instance.GetSpawnPoint(0).position;
        PlayerCharacter p1 = Instantiate(GameAssets.Instance.PlayerPF, spawnPos, Quaternion.identity).GetComponent<PlayerCharacter>();
        player1 = p1;
        GameManager.Instance.SetPlayer1(p1);
    }

    public void SetAllPlayersControlMapToInGame()
        => SetAllPlayersControlMap("InGame");
    public void SetAllPlayersControlMapToUI()
        => SetAllPlayersControlMap("UI");
    public void SetAllPlayersControlMapToDialogue()
        => SetAllPlayersControlMap("Dialogue");
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

        InputUser p1 = GameManager.Player1Ref.Inputs.user;
        p1.UnpairDevices();
        for (int i = 1; i < PlayerInput.all.Count; i++)
        {
            PlayerInput playerInput = PlayerInput.all[i];
            if (playerInput.devices.Contains(d))
            {
                if (GameManager.Instance.allowQuitLobby == false)
                {
                    GiveUnpairedDevicesToP1();
                    return;
                }
                playerInput.user.UnpairDevices();

                DataKeeper.Instance.RemoveData(playerInput);

                GiveUnpairedDevicesToP1();
                return;
            }
        }

        PlayerInputManager.instance.JoinPlayerFromAction(context);

        GiveUnpairedDevicesToP1();

        void GiveUnpairedDevicesToP1()
        {
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
                InputUser.PerformPairingWithDevice(item, p1);
            }
        }
    }

    public void LeaveAction(InputAction.CallbackContext context)
    {
        Debug.Log(context);
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


    public void AddAlivePlayer(PlayerCharacter player)
    {
        deadPlayers.Remove(player);
    }
    public void RemoveAlivePlayer(PlayerCharacter player)
    {
        if (!deadPlayers.Contains(player)) deadPlayers.Add(player);

        if (deadPlayers.Count >= GameManager.Instance.PlayersCount)
        {
            LastDeadPlayerTransform = player.transform;
            GameManager.Instance.GameState = GameManager.E_GameState.GameOver;
        }
    }

    public void DefinitiveDeath(PlayerCharacter player)
    {
        RemoveAlivePlayer(player);
    }
}

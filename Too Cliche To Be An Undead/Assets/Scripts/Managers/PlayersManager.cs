using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayersManager : MonoBehaviour
{
    private static PlayersManager instance;
    public static PlayersManager Instance
    {
        get
        {
            if (instance == null) Debug.LogError("PlayersManager Instance not found.");

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
    public struct PlayerCharacterComponents
    {
        public GameManager.E_CharactersNames character;
        public SCRPT_Dash dash;
        public SCRPT_Skill skill;
        public SCRPT_EntityStats stats;
        public Sprite sprite;
    }

    [SerializeField] private PlayerCharacterComponents[] characterComponents;
    public PlayerCharacterComponents[] CharacterComponents { get => characterComponents; }

    private void Awake()
    {
        if (instance == null) 
            instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }

        PlayerCharacter p1 = Instantiate(GameAssets.Instance.PlayerPF).GetComponent<PlayerCharacter>();
        player1 = p1;
        GameManager.Instance.SetPlayer1(p1);

        joinAction.performed += context => JoinAction(context);

        leaveAction.performed += context => LeaveAction(context);

        DontDestroyOnLoad(this);
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

    public void SetupPanels(int idx)
    {
        if (panelsManager != null)
        {
            panelsManager.SetupPanel(idx);
        }
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

    private void JoinAction(InputAction.CallbackContext context)
    {
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context);
    }

    private void LeaveAction(InputAction.CallbackContext context)
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

    public void AddAlivePlayer() => alivePlayersCount++;
    public void RemoveAlivePlayer()
    {
        alivePlayersCount--;
        if (alivePlayersCount <= 0) GameManager.Instance.GameState = GameManager.E_GameState.GameOver;
    }

    public void DefinitiveDeath(PlayerCharacter player)
    {
        Camera.main.GetComponent<CameraController>().RemovePlayerFromList(player.transform);
    }
}

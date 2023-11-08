using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DataKeeper : PersistentSingleton<DataKeeper>
{
    [SerializeField] private CharactersSprites[] charactersSprites;
    public CharactersSprites[] GetCharactersSprites { get => charactersSprites; }

    [System.Serializable]
    public struct CharactersSprites
    {
        public GameManager.E_CharactersNames characterName;
        public Sprite characterSprite;
    }

    [System.Serializable]
    public class PlayerDataKeep
    {
        public string playerName;
        public PlayerInputHandler playerInput;
        public GameManager.E_CharactersNames character;

        public PlayerDataKeep(string _playerName, PlayerInputHandler _playerInput, GameManager.E_CharactersNames _character)
        {
            playerName = _playerName;
            playerInput = _playerInput;
            character = _character;
        }
    }

    public delegate void D_PlayerCreated(int playerIndx, PlayerCharacter player);
    public D_PlayerCreated D_playerCreated;

    public delegate void D_PlayerDestroyed(int playerIdx, PlayerCharacter player);
    public D_PlayerDestroyed D_playerDestroyed;

    public List<PlayerDataKeep> playersDataKeep = new List<PlayerDataKeep>();
    public List<int> unlockedLevels = new List<int>();
    public int money;
    public int maxLevel;

    public bool firstPassInMainMenu = true;
    public int runsCount = 0;

    public bool skipTuto = true;
    public bool alreadyPlayedTuto = false;

    public bool allowGamepadShake = true;

    protected override void EventsSubscriber()
    {
        base.EventsSubscriber();
        PlayerInputsEvents.OnPlayerInputsCreated += CreateInput;
        PlayerInputsEvents.OnPlayerInputsDestroyed += RemoveInput;
        PlayerInputsEvents.OnChangedIndex += RenameSingle;
        PlayerInputsManagerEvents.OnEndedChangingIndexes += RenameAllInList;
        TutorialEvents.OnTutorialEnded += OnTutorialEnded;
    }

    protected override void EventsUnSubscriber()
    {
        base.EventsUnSubscriber();
        PlayerInputsEvents.OnPlayerInputsCreated -= CreateInput;
        PlayerInputsEvents.OnPlayerInputsDestroyed -= RemoveInput;
        PlayerInputsEvents.OnChangedIndex -= RenameSingle;
        PlayerInputsManagerEvents.OnEndedChangingIndexes -= RenameAllInList;
        TutorialEvents.OnTutorialEnded -= OnTutorialEnded;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    protected override void OnSceneUnloaded(Scene scene)
    {
        CinematicManagerEvents.ForceSetIsInCinematic(false);
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

    public void OnTutorialEnded() => alreadyPlayedTuto = true;
    public bool ShouldSkipTuto() => skipTuto || alreadyPlayedTuto;

    private void RenameSingle(int lastIdx, int newIdx)
    {
        playersDataKeep[newIdx].playerName = "PlayerInputs " + newIdx;
    }
    private void RenameAllInList()
    {
        foreach (var item in playersDataKeep)
        {
            item.playerName = "PlayerInputs " + item.playerInput.InputsID; 
        }
    }

    private void CreateInput(PlayerInputHandler newInput)
    {
        PlayerDataKeep pdk = new PlayerDataKeep("PlayerInputs " + newInput.InputsID, newInput, GameManager.E_CharactersNames.Shirley);
        playersDataKeep.Add(pdk);
    }

    private void RemoveInput(int idx)
    {
        if (idx >= playersDataKeep.Count) return;
        playersDataKeep.RemoveAt(idx);
    }

    public bool IsPlayerDataKeepSet() => (playersDataKeep != null && playersDataKeep.Count > 0);

    public int GetIndex(string _playerName)
    {
        foreach (var item in playersDataKeep)
        {
            if (item.playerName.Equals(_playerName)) return playersDataKeep.IndexOf(item);
        }

        return -1;
    }
    public int GetIndex(PlayerInput playerInput)
    {
        foreach (var item in playersDataKeep)
        {
            if (item.playerInput.Equals(playerInput)) return playersDataKeep.IndexOf(item);
        }

        return -1;
    }

    public PlayerCharacter GetPlayerFromIndex(int idx)
    {
        PlayerDataKeep playerData = playersDataKeep[idx];
        return playerData.playerInput.GetComponentInParent<PlayerCharacter>();
    }

    public static bool StartInTutorial()
    {
        return (!Instance.skipTuto) && (!Instance.alreadyPlayedTuto);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class DataKeeper : MonoBehaviour
{
    private static DataKeeper instance;
    public static DataKeeper Instance
    {
        get
        {
            return instance;
        }
    }

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
        public PlayerInput playerInput;
        public GameManager.E_CharactersNames character;

        public PlayerDataKeep(string _playerName, PlayerInput _playerInput, GameManager.E_CharactersNames _character)
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

    public bool skipTuto = true;
    public bool alreadyPlayedTuto = false;

    public bool allowGamepadShake = true;

    public int CreateData(PlayerCharacter newPlayer)
    {
        foreach (var item in playersDataKeep)
        {
            if (item.playerInput.Equals(newPlayer.Inputs)) return playersDataKeep.IndexOf(item) - 1;
        }
        newPlayer.name = $"Player {playersDataKeep.Count}";

        PlayerDataKeep pdk = new PlayerDataKeep(newPlayer.name, newPlayer.Inputs, GameManager.E_CharactersNames.Shirley);
        playersDataKeep.Add(pdk);

        int playerIdx = playersDataKeep.Count - 1;

        D_playerCreated?.Invoke(playerIdx, newPlayer);

        return playerIdx;
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

    public void RemoveData(string _playerName)
    {
        for (int i = 0; i < playersDataKeep.Count; i++)
        {
            if (playersDataKeep[i].playerName.Equals(_playerName)) RemoveData(i);
        }
    }
    public void RemoveData(PlayerInput _playerInput)
    {
        for (int i = 0; i < playersDataKeep.Count; i++)
        {
            if (playersDataKeep[i].playerInput.Equals(_playerInput)) RemoveData(i);
        }
    }
    public void RemoveData(int idx)
    {
        if (GameManager.isAppQuitting) return;

        if (idx < 0 || idx >= playersDataKeep.Count) return;

        // Get the object's root
        PlayerCharacter playerScript = playersDataKeep[idx].playerInput.GetComponentInParent<PlayerCharacter>();
        GameObject player = playerScript.gameObject;
        playersDataKeep.RemoveAt(idx);

        D_playerDestroyed?.Invoke(idx, playerScript);

        Destroy(player);
        PlayersManager.Instance.CleanInputs();

        if (playersDataKeep.Count > 0)
        {
            for (int i = 0; i < playersDataKeep.Count; i++)
            {
                playersDataKeep[i].playerInput.GetComponentInParent<PlayerCharacter>().ForceSetIndex(i);
            }
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            this.transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameManager;

public class DataKeeper : MonoBehaviour
{
    private static DataKeeper instance;
    public static DataKeeper Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("DataKeeper instance was not set.");
                //instance = FindObjectOfType<DataKeeper>();
                //if (instance == null)
                //{
                //    Debug.LogError("Could not find DataKeeper object. Force creation");
                //    GameObject obj = new GameObject();
                //    obj.name = typeof(DataKeeper).Name;
                //    instance = obj.AddComponent<DataKeeper>();
                //}
                //else Debug.LogError("Found DataKeeper object");
            }
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
        public int money;
        public int maxLevel;
        public PlayerInput playerInput;
        public E_CharactersNames character;

        public PlayerDataKeep(string _playerName, PlayerInput _playerInput, E_CharactersNames _character, int _money = 0, int _maxLevel = 0)
        {
            playerName = _playerName;
            money = _money;
            maxLevel = _maxLevel;
            playerInput = _playerInput;
            character = _character;
        }
    }
    public List<PlayerDataKeep> playersDataKeep = new List<PlayerDataKeep>();

    public int CreateData(PlayerCharacter newPlayer)
    {
        foreach (var item in playersDataKeep)
        {
            if (item.playerInput.Equals(newPlayer.Inputs)) return playersDataKeep.IndexOf(item) - 1;
        }
        newPlayer.name = $"Player {playersDataKeep.Count}";

        PlayerDataKeep pdk = new PlayerDataKeep(newPlayer.name, newPlayer.Inputs, E_CharactersNames.Shirley, newPlayer.Money, newPlayer.Level);
        playersDataKeep.Add(pdk);

        return playersDataKeep.Count - 1;
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
        PlayerDataKeep pdk = null;
        foreach (var item in playersDataKeep)
        {
            if (item.playerName.Equals(_playerName))
            {
                pdk = item;
                return;
            }
        }

        if (pdk != null)
            playersDataKeep.Remove(pdk);
    }
    public void RemoveData(PlayerInput _playerInput)
    {
        PlayerDataKeep pdk = null;
        foreach (var item in playersDataKeep)
        {
            if (item.playerInput.Equals(_playerInput))
            {
                pdk = item;
                return;
            }
        }

        if (pdk != null)
            playersDataKeep.Remove(pdk);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}

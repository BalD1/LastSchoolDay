using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataKeeper : MonoBehaviour
{
    private static DataKeeper instance;
    public static DataKeeper Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("DataKeeper instance was not set. Searching...");
                instance = FindObjectOfType<DataKeeper>();
                if (instance == null)
                {
                    Debug.LogError("Could not find DataKeeper object. Force creation");
                    GameObject obj = new GameObject();
                    obj.name = typeof(DataKeeper).Name;
                    instance = obj.AddComponent<DataKeeper>();
                }
                else Debug.LogError("Found DataKeeper object");
            }
            return instance;
        }
    }

    [System.Serializable]
    public class PlayerDataKeep
    {
        public string playerName;
        public int money;
        public int maxLevel;

        public PlayerDataKeep(string _playerName, int _money = 0, int _maxLevel = 0)
        {
            playerName = _playerName;
            money = _money;
            maxLevel = _maxLevel;
        }
    }
    public List<PlayerDataKeep> playersDataKeep = new List<PlayerDataKeep>();
        
    public bool IsPlayerDataKeepSet() => (playersDataKeep != null && playersDataKeep.Count > 0);

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

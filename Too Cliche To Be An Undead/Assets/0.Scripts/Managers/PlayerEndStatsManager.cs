using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEndStatsManager : MonoBehaviourEventsHandler
{
	private static PlayerEndStatsManager instance;
	public static PlayerEndStatsManager Instance
	{
		get
		{
			if (instance == null) Debug.LogError("PlayerEndStatsManager instance could not be found.");

			return instance;
		}
	}

    [System.Serializable]
    public class PlayerEndStats
    {
        public PlayerCharacter relatedPlayer;
        public int KillsCount;
        public int DamagesDealt;
        public int DamagesTaken;

        public PlayerEndStats(PlayerCharacter _relatedPlayer)
        {
            relatedPlayer = _relatedPlayer;
            KillsCount = 0;
            DamagesDealt = 0;
            DamagesTaken = 0;
        }
    }

    private bool allowGameTimeIncrease = false;
    private bool isValid = false;

    [field: SerializeField] public PlayerEndStats[] PlayersEndStatsArray { get; private set; }

    [field: SerializeField] public float GameTime { get; private set; }

    protected override void Awake()
	{
        if (instance == null)
        {
            instance = this;

            this.transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
            isValid = true;
        }
        else
        {
            Destroy(this.gameObject);
            isValid = false;
            return;
        }
            
        SceneManager.sceneLoaded += OnSceneLoaded;
	}

    private void Start()
    {
#if UNITY_EDITOR
        if (!isValid) return;

        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainScene))
            SetupArray(); 
#endif
    }

    private void Update()
    {
        if (!allowGameTimeIncrease) return;
        if (!(GameManager.Instance.GameState == GameManager.E_GameState.InGame)) return;

        GameTime += Time.deltaTime; 
    }

    protected override void EventsSubscriber()
    {
        GameManagerEvents.OnRunStarted += OnRunStarted;
    }

    protected override void EventsUnSubscriber()
    {
        GameManagerEvents.OnRunStarted -= OnRunStarted;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => SetupArray();
    private void SetupArray()
    {
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu))
        {
            PlayersEndStatsArray = new PlayerEndStats[0];
            return;
        }

        if (IsArrayInitialized()) return;

        PlayersEndStatsArray = new PlayerEndStats[GameManager.Instance.PlayersCount];
        GameTime = 0;

        for (int i = 0; i < GameManager.Instance.PlayersCount; i++)
        {
            PlayersEndStatsArray[i] = new PlayerEndStats(GameManager.Instance.playersByName[i].playerScript);
        }

        allowGameTimeIncrease = false;
    }

    private void OnRunStarted()
        => allowGameTimeIncrease = true;

    public void KeepScores()
    {
        for (int i = 0; i < GameManager.Instance.PlayersCount; i++)
        {
            PlayerEndStats playerStats = PlayersEndStatsArray[i];
            PlayerCharacter player = GameManager.Instance.playersByName[i].playerScript;

            playerStats.DamagesDealt += player.DamagesDealt;
            playerStats.DamagesTaken += player.DamagesTaken;
            playerStats.KillsCount += player.KillsCount;
        }
    }

    private bool IsArrayInitialized()
    {
        return PlayersEndStatsArray != null && PlayersEndStatsArray.Length > 0;
    }

    public PlayerEndStats GetPlayerEndStats(PlayerCharacter player)
    {
        foreach (var item in PlayersEndStatsArray)
        {
            if (item.relatedPlayer == player) return item;
        }

        return null;
    }
}
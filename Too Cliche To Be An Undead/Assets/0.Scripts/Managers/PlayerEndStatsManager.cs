using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEndStatsManager : PersistentSingleton<PlayerEndStatsManager>
{
    [System.Serializable]
    public class PlayerEndStats
    {
        public int KillsCount;
        public float DamagesDealt;
        public float DamagesTaken;

        public PlayerEndStats()
        {
            KillsCount = 0;
            DamagesDealt = 0;
            DamagesTaken = 0;
        }
    }

    private bool allowGameTimeIncrease = false;

    [field: SerializeField] public Dictionary<PlayerCharacter, PlayerEndStats> PlayersStats { get; private set; } = new Dictionary<PlayerCharacter, PlayerEndStats>();

    [field: SerializeField] public float GameTime { get; private set; }

    protected override void Start()
    {
        base.Start();
        PlayersStats = new Dictionary<PlayerCharacter, PlayerEndStats>();
        SetupArray();
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

        EntityEvents.OnPlayerTookDamages += OnPlayerTookDamages;
        EntityEvents.OnEnemyTookDamages += OnEnemyTookDamages;
        EntityEvents.OnEnemyDeath += OnEnemyDeath;
    }

    protected override void EventsUnSubscriber()
    {
        GameManagerEvents.OnRunStarted -= OnRunStarted;

        EntityEvents.OnPlayerTookDamages -= OnPlayerTookDamages;
        EntityEvents.OnEnemyTookDamages -= OnEnemyTookDamages;
        EntityEvents.OnEnemyDeath -= OnEnemyDeath;
    }

    public void ResetScores()
    {
        foreach (var item in PlayersStats)
        {
            item.Value.DamagesDealt = 0;
            item.Value.DamagesTaken = 0;
            item.Value.KillsCount = 0;
        }
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        => SetupArray();

    protected override void OnSceneUnloaded(Scene scene) { }

    private void SetupArray()
    {
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu))
        {
            PlayersStats.Clear();
            return;
        }

        if (PlayersStats.Count == IGPlayersManager.PlayersCount) return;
        PlayersStats.Clear();
        GameTime = 0;

        foreach (var item in IGPlayersManager.Instance.PlayersList)
        {
            PlayersStats.Add(item, new PlayerEndStats());
        }

        allowGameTimeIncrease = false;
    }

    private void OnRunStarted()
        => allowGameTimeIncrease = true;

    private void OnPlayerTookDamages(EntityEvents.OnEntityDamagesData<PlayerCharacter> data)
    {
        if (!PlayersStats.TryGetValue(data.DamagedEntity, out PlayerEndStats stats))
        {
            this.Log("Could not find player " + data.DamagedEntity + " in PlayerEndStats.", LogsManager.E_LogType.Error);
            return;
        }

        stats.DamagesTaken += data.TakenDamages;
        //this.Log($"Added {data.TakenDamages} Taken D to {data.DamagedEntity.GetCharacterName()} - Total : {stats.DamagesTaken}");
    }

    private void OnEnemyTookDamages(EntityEvents.OnEntityDamagesData<EnemyBase> data)
    {
        PlayerCharacter player = data.Damager as PlayerCharacter;
        if (player == null) return;

        if (!PlayersStats.TryGetValue(player, out PlayerEndStats stats))
        {
            this.Log("Could not find player " + data.DamagedEntity + " in PlayerEndStats.", LogsManager.E_LogType.Error);
            return;
        }

        stats.DamagesDealt += data.TakenDamages;

        //this.Log($"Added {data.TakenDamages} Dealt D to {player.GetCharacterName()} - Total : {stats.DamagesDealt}");
    }

    private void OnEnemyDeath(EntityEvents.OnEntityDamagesData<EnemyBase> data)
    {
        PlayerCharacter player = data.Damager as PlayerCharacter;
        if (player == null) return;

        if (!PlayersStats.TryGetValue(player, out PlayerEndStats stats))
        {
            this.Log("Could not find player " + data.DamagedEntity + " in PlayerEndStats.", LogsManager.E_LogType.Error);
            return;
        }

        stats.KillsCount++;
        //this.Log($"Added 1 kill to {player.GetCharacterName()} - Total : {stats.KillsCount}");
    }
}
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
        ResetArray();

#if UNITY_EDITOR
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu))
            return;
        if (PlayersStats.Count > 0) 
            return;

        ResetArray();
        if (IGPlayersManager.ST_TryGetPlayer(0, out PlayerCharacter p1))
            OnPlayerCreated(p1);
#endif
    }

    private void Update()
    {
        if (!allowGameTimeIncrease) return;
        if (! GameManager.ST_InstanceExists() || !(GameManager.Instance.GameState == GameManager.E_GameState.InGame)) return;

        GameTime += Time.deltaTime; 
    }

    protected override void EventsSubscriber()
    {
        base.EventsSubscriber();
        GameManagerEvents.OnRunStarted += OnRunStarted;

        EntityEvents.OnPlayerTookDamages += OnPlayerTookDamages;
        EntityEvents.OnEnemyTookDamages += OnEnemyTookDamages;
        EntityEvents.OnEnemyDeath += OnEnemyDeath;

        IGPlayersManagerEvents.OnPlayerCreated += OnPlayerCreated;
    }

    protected override void EventsUnSubscriber()
    {
        base.EventsUnSubscriber();
        GameManagerEvents.OnRunStarted -= OnRunStarted;

        EntityEvents.OnPlayerTookDamages -= OnPlayerTookDamages;
        EntityEvents.OnEnemyTookDamages -= OnEnemyTookDamages;
        EntityEvents.OnEnemyDeath -= OnEnemyDeath;

        IGPlayersManagerEvents.OnPlayerCreated -= OnPlayerCreated;
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
        => ResetArray();

    protected override void OnSceneUnloaded(Scene scene) { }

    private void ResetArray()
    {
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu))
        {
            PlayersStats.Clear();
            return;
        }

        if (PlayersStats.Count == IGPlayersManager.PlayersCount) return;
        PlayersStats.Clear();
        GameTime = 0;

        allowGameTimeIncrease = false;
    }

    private void OnPlayerCreated(PlayerCharacter player)
    {
        if (!PlayersStats.TryAdd(player, new PlayerEndStats()))
            this.Log($"Could not add {player} ({player.GetCharacterName()}) in array.", LogsManager.E_LogType.Error);
    }

    private void OnRunStarted()
        => allowGameTimeIncrease = true;

    private void OnPlayerTookDamages(EntityEvents.OnEntityDamagesData<PlayerCharacter> data)
    {
        if (!PlayersStats.TryGetValue(data.DamagedEntity, out PlayerEndStats stats))
        {
            this.Log($"Could not find player {data.DamagedEntity} ({data.DamagedEntity.GetCharacterName()}) in PlayerEndStats.", LogsManager.E_LogType.Error);
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
            this.Log($"Could not find player {player} ({player.GetCharacterName()}) in PlayerEndStats.", LogsManager.E_LogType.Error);
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
            this.Log($"Could not find player {player} ({player.GetCharacterName()}) in PlayerEndStats.", LogsManager.E_LogType.Error);
            return;
        }

        stats.KillsCount++;
        //this.Log($"Added 1 kill to {player.GetCharacterName()} - Total : {stats.KillsCount}");
    }
}
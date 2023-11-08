using System.Collections.Generic;
using UnityEngine;

public class NewPlayersManager : Singleton<NewPlayersManager>
{
    [SerializeField] private NewPlayerCharacter playerPF;

    [field: SerializeField] public List<NewPlayerCharacter> PlayersList { get; private set; } = new List<NewPlayerCharacter>();

    [field: SerializeField] public Transform[] tutoSpawnPoints;
    [field: SerializeField] public Transform[] igSpawnPoints;

    public static int PlayersCount { get => PlayerInputsManager.PlayersCount; }

    protected override void EventsSubscriber()
    {
        GymnasiumCinematicEvents.OnGymnasiumCinematicStarted += ReviveAllDyingPlayers;
    }

    protected override void EventsUnSubscriber()
    {
        GymnasiumCinematicEvents.OnGymnasiumCinematicStarted -= ReviveAllDyingPlayers;
    }

    protected override void Start()
    {
        base.Start();
        List<NewPlayerInputsHandler> inputs = new List<NewPlayerInputsHandler>(NewPlayerInputsManager.Instance.PlayerInputsList);
        bool playTuto = !DataKeeper.Instance.skipTuto && !DataKeeper.Instance.alreadyPlayedTuto;
        for (int i = 0; i < inputs.Count; i++)
        {
            Vector2 spawnPos = playTuto ? tutoSpawnPoints[i].position : igSpawnPoints[i].position;
            NewPlayerCharacter newPlayer = playerPF?.Create(spawnPos);
            newPlayer.Setup(inputs[i], i);
            PlayersList.Add(newPlayer);
            this.PlayerCreated(newPlayer);
        }

        this.AllPlayersCreated(PlayersList);
    }

    public void ReviveAllDyingPlayers()
    {
        foreach (var player in PlayersList)
        {
            //if (player.StateManager.CurrentState.ToString() == "Dying")
            //    player.AskRevive();
        }
    }

    public static bool ST_TryGetPlayer(int index, out NewPlayerCharacter player)
    {
        player = null;
        if (!ST_InstanceExists()) return false;
        if (index < 0 || index >= Instance.PlayersList.Count) return false;

        player = Instance.PlayersList[index];
        return true;
    }

    public void TeleportPlayer(int playerIndex, Vector2 position)
    {
        PlayersList[playerIndex].transform.position = position;
    }
}

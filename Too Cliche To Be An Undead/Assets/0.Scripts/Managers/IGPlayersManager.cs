using System.Collections.Generic;
using UnityEngine;

public class IGPlayersManager : Singleton<IGPlayersManager>
{
    [SerializeField] private PlayerCharacter playerPF;

    [field: SerializeField] public List<PlayerCharacter> PlayersList { get; private set; } = new List<PlayerCharacter>();

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
        List<PlayerInputs> inputs = new List<PlayerInputs>(PlayerInputsManager.Instance.PlayerInputsList);
        bool playTuto = !DataKeeper.Instance.skipTuto && !DataKeeper.Instance.alreadyPlayedTuto;
        for (int i = 0; i < inputs.Count; i++)
        {
            Vector2 spawnPos = playTuto ? tutoSpawnPoints[i].position : igSpawnPoints[i].position;
            PlayerCharacter newPlayer = playerPF?.Create(spawnPos); 
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
            if (player.StateManager.CurrentState.ToString() == "Dying")
                player.AskRevive();
        }
    }

    public static bool ST_TryGetPlayer(int index, out PlayerCharacter player)
    {
        player = null;
        if (!IGPlayersManager.ST_InstanceExists()) return false;
        if (index < 0 || index >= Instance.PlayersList.Count) return false;

        player = Instance.PlayersList[index];
        return true;
    }

    public void TeleportPlayer(int playerIndex, Vector2 position)
    {
        PlayersList[playerIndex].transform.position = position;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class IGPlayersManager : Singleton<IGPlayersManager>
{
    [SerializeField] private PlayerCharacter playerPF;

    [field: SerializeField] public List<PlayerCharacter> PlayersList { get; private set; } = new List<PlayerCharacter>();

    [field: SerializeField] public Transform[] tutoSpawnPoints;
    [field: SerializeField] public Transform[] igSpawnPoints;

    public static int PlayersCount { get => PlayerInputsManager.PlayersCount; }

    private void Start()
    {
        List<PlayerInputs> inputs = new List<PlayerInputs>(PlayerInputsManager.Instance.PlayerInputsList);
        bool playTuto = !DataKeeper.Instance.skipTuto && !DataKeeper.Instance.alreadyPlayedTuto;
        for (int i = 0; i < inputs.Count; i++)
        {
            Vector2 spawnPos = playTuto ? tutoSpawnPoints[i].position : igSpawnPoints[i].position;
            PlayerCharacter newPlayer = playerPF?.Create(spawnPos); 
            newPlayer.Setup(inputs[i]);
            PlayersList.Add(newPlayer);
            this.PlayerCreated(newPlayer);
        }

        this.AllPlayersCreated(PlayersList);
    }
}

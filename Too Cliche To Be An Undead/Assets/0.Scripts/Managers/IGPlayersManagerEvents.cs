using System;
using System.Collections.Generic;
using UnityEngine;

public static class IGPlayersManagerEvents
{
    public static event Action<PlayerCharacter> OnPlayerCreated;
    public static void PlayerCreated(this IGPlayersManager manager, PlayerCharacter player)
        => OnPlayerCreated?.Invoke(player);

    public static event Action<List<PlayerCharacter>> OnAllPlayersCreated;
    public static void AllPlayersCreated(this IGPlayersManager manager, List<PlayerCharacter> allPlayers)
        => OnAllPlayersCreated?.Invoke(allPlayers);
}

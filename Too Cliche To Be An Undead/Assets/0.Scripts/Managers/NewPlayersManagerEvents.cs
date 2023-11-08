using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NewPlayersManagerEvents
{
    public static event Action<NewPlayerCharacter> OnPlayerCreated;
    public static void PlayerCreated(this NewPlayersManager manager, NewPlayerCharacter player)
        => OnPlayerCreated?.Invoke(player);

    public static event Action<List<NewPlayerCharacter>> OnAllPlayersCreated;
    public static void AllPlayersCreated(this NewPlayersManager manager, List<NewPlayerCharacter> allPlayers)
        => OnAllPlayersCreated?.Invoke(allPlayers);
}

using System;
using UnityEngine;

public static class IGPlayersManagerEvents
{
    public static event Action<PlayerCharacter> OnPlayerCreated;
    public static void PlayerCreated(this IGPlayersManager manager, PlayerCharacter player)
        => OnPlayerCreated?.Invoke(player);
}


using System;

public static class PlayerCharacterEvents
{
    public static event Action<PlayerCharacter> OnPlayerSetup;
    public static void PlayerSetup(this PlayerCharacter player)
        => OnPlayerSetup?.Invoke(player);
}

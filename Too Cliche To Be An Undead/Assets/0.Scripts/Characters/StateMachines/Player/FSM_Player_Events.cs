using System;

public static class FSM_Player_Events
{
    public static event Action<PlayerCharacter> OnEnteredDying;
    public static void EnteredDying(this FSM_Player_Dying fsm, PlayerCharacter player) => OnEnteredDying?.Invoke(player);

    public static event Action<PlayerCharacter> OnExitedDying;
    public static void ExitedDying(this FSM_Player_Dying fsm, PlayerCharacter player) => OnExitedDeath?.Invoke(player);

    public static event Action<PlayerCharacter> OnEnteredDeath;
    public static void EnteredDeath(this FSM_Player_Dead fsm, PlayerCharacter player) => OnEnteredDeath?.Invoke(player);

    public static event Action<PlayerCharacter> OnExitedDeath;
    public static void ExitedDeath(this FSM_Player_Dead fsm, PlayerCharacter player) => OnExitedDeath?.Invoke(player);

}

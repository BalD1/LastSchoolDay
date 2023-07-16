using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManagerEvents
{
    public static event Action<GameManager.E_GameState> OnGameStateChange;
    public static void GameStateChange(this GameManager manager, GameManager.E_GameState state)
        => OnGameStateChange?.Invoke(state);

    public static event Action OnRunStarted;
    public static void RunStarted(this GameManager manager)
        => OnRunStarted?.Invoke();
}

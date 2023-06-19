using System;
using UnityEngine;

public static class CinematicManagerEvents
{
    public static event Action<Vector2> OnPlayersAskMove;
    public static void PlayersAskMove(this Cinematic cinematic, Vector2 targetPos) 
        => OnPlayersAskMove?.Invoke(targetPos);

    public static event Action<bool> OnChangeCinematicState;
    public static void ChangeCinematicState(this Cinematic cinematic, bool newState)
        => OnChangeCinematicState?.Invoke(newState);
}

using System;
using UnityEngine;

public static class TutorialEvents
{
    public static event Action OnTutorialStarted;
    public static void TutorialStated(this Tutorial tuto)
        => OnTutorialStarted?.Invoke();

    public static event Action OnTutorialEnded;
    public static void TutorialEnded(this Tutorial tuto)
        => OnTutorialEnded?.Invoke();
}

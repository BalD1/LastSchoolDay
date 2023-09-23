using System;
using System.Collections;
using UnityEngine;

public static class DebugConsoleEvents
{
    public static event Action OnForceWin;
    public static void ForceWin(this DebugConsole console)
        => OnForceWin?.Invoke();
}

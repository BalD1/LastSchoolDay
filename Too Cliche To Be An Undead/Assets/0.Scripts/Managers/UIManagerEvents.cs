using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIManagerEvents
{
    public static event Action OnEnteredUI;
    public static void EnteredUI(this UIManager manager) => OnEnteredUI?.Invoke();

    public static event Action OnExitedUI;
    public static void ExitedUI(this UIManager manager) => OnExitedUI?.Invoke();
}

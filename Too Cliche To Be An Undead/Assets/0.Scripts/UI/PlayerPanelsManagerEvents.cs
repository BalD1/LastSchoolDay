using System;
using UnityEngine;

public static class PlayerPanelsManagerEvents
{
    public static event Action OnPanelsActive;
    public static void PanelsActive(this PlayerPanelsManager manager) => OnPanelsActive?.Invoke();

    public static event Action OnPanelsInactive;
    public static void PanelsInacative(this PlayerPanelsManager manager) => OnPanelsInactive?.Invoke();
}

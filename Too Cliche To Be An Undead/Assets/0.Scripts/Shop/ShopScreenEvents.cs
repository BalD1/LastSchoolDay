using System;
using UnityEngine;

public static class ShopScreenEvents
{
    public static event Action OnOpenUI;
    public static void OpenUI(this ShopScreen screen)
        => OnOpenUI?.Invoke();

    public static event Action OnCloseUI;
    public static void CloseUI(this ShopScreen screen)
        => OnCloseUI?.Invoke();
}

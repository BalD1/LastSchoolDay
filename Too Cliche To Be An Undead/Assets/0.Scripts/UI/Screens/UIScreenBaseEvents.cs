using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIScreenBaseEvents 
{
    public static event Action<UIScreenBase> OnOpenScreen;
    public static void OpenScreen(this UIScreenBase screen)
        => OnOpenScreen?.Invoke(screen);

    public static event Action<UIScreenBase> OnShowScreen;
    public static void ShowScreen(this UIScreenBase screen)
        => OnShowScreen?.Invoke(screen);

    public static event Action<UIScreenBase> OnCloseScreen;
    public static void CloseScreen(this UIScreenBase screen)
        => OnCloseScreen?.Invoke(screen);

    public static event Action<UIScreenBase> OnHideScreen;
    public static void HideScreen(this UIScreenBase screen)
        => OnHideScreen?.Invoke(screen);

    public static event Action OnScreenTweenEnded;
    public static void ScreenTweenEnd(this UIScreenBase screen)
        => OnScreenTweenEnded?.Invoke();
}

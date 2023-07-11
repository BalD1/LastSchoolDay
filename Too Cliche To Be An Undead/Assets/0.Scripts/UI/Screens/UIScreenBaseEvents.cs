using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIScreenBaseEvents 
{
    public static event Action<UIScreenBase, bool> OnOpenScreen;
    public static void OpenScreen(this UIScreenBase screen, bool ignoreTweens)
        => OnOpenScreen?.Invoke(screen, ignoreTweens);

    public static event Action<UIScreenBase, bool> OnShowScreen;
    public static void ShowScreen(this UIScreenBase screen, bool ignoreTweens)
        => OnShowScreen?.Invoke(screen, ignoreTweens);

    public static event Action<UIScreenBase, bool> OnCloseScreen;
    public static void CloseScreen(this UIScreenBase screen, bool ignoreTweens)
        => OnCloseScreen?.Invoke(screen, ignoreTweens);

    public static event Action<UIScreenBase, bool> OnHideScreen;
    public static void HideScreen(this UIScreenBase screen, bool ignoreTweens)
        => OnHideScreen?.Invoke(screen, ignoreTweens);

    public static event Action OnScreenTweenEnded;
    public static void ScreenTweenEnd(this UIScreenBase screen)
        => OnScreenTweenEnded?.Invoke();
}

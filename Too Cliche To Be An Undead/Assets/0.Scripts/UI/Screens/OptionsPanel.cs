using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsPanel : UIScreenBase
{
    private UIScreenBase lastScreen;

    protected override void EventsSubscriber()
    {
        base.EventsSubscriber();
        UIScreenBaseEvents.OnOpenScreen += OnOpenScreen;
    }

    protected override void EventsUnSubscriber()
    {
        base.EventsUnSubscriber();
        UIScreenBaseEvents.OnOpenScreen -= OnOpenScreen;
    }

    public override void Close(int playerIdx)
    {
        bool ignoreTweens = lastScreen is PausePanel;
        Close(playerIdx, ignoreTweens);
    }

    private void OnOpenScreen(UIScreenBase screen, bool ignoreTweens)
    {
        if (screen == this) return;
        lastScreen = screen;
    }
}

using System;

public static class PlayerInputsEvents
{
    public static event Action OnPauseCall;
    public static void PauseCall(this PlayerInputs playerInputs) => OnPauseCall?.Invoke();

    public static event Action OnCloseMenuCall;
    public static void CloseMenuCall(this PlayerInputs playerInputs) => OnCloseMenuCall?.Invoke();

    public static event Action OnScrollCurrentVBDownCall;
    public static void ScrollCurrentVBDownCall(this PlayerInputs playerInputs) => OnScrollCurrentVBDownCall?.Invoke();

    public static event Action OnScrollCurrentVBUpCall;
    public static void ScrollCurrentVBUpCall(this PlayerInputs playerInputs) => OnScrollCurrentVBUpCall?.Invoke();

    public static event Action OnScrollCurrentHBLeftCall;
    public static void ScrollCurrentHBLeftCall(this PlayerInputs playerInputs) => OnScrollCurrentHBLeftCall?.Invoke();

    public static event Action OnScrollCurrentHBRightCall;
    public static void ScrollCurrentHBRightCall(this PlayerInputs playerInputs) => OnScrollCurrentHBRightCall?.Invoke();

    public static event Action OnTryNextLineCall;
    public static void TryNextLineCall(this PlayerInputs playerInputs) => OnTryNextLineCall?.Invoke();

    public static event Action OnSkipDialogueCall;
    public static void SkipDialogueCall(this PlayerInputs playerInputs) => OnSkipDialogueCall?.Invoke();
}

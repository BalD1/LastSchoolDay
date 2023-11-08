using System;
using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerInputsEvents
{
    public static event Action<PlayerInputHandler> OnPlayerInputsCreated;
    public static void PlayerInputsCreated(this PlayerInputHandler inputs) => OnPlayerInputsCreated?.Invoke(inputs);

    public static event Action<int, int> OnChangedIndex;
    public static void ChangedIndex(this PlayerInputHandler inputs, int previousIndex, int newIndex) => OnChangedIndex?.Invoke(previousIndex, newIndex);

    public static event Action<int> OnPlayerInputsDestroyed;
    public static void PlayerInputsDestroyed(this PlayerInputHandler inputs, int idx) => OnPlayerInputsDestroyed?.Invoke(idx);

    public static event Action OnPauseCall;
    public static void PauseCall(this PlayerInputHandler playerInputs) => OnPauseCall?.Invoke();

    public static event Action OnCloseMenuCall;
    public static void CloseMenuCall(this PlayerInputHandler playerInputs) => OnCloseMenuCall?.Invoke();

    public static event Action<bool, int> OnHorizontalArrows;
    public static void HorizontalArrows(this PlayerInputHandler playerInputs, bool rightArrow, int idx)
        => OnHorizontalArrows?.Invoke(rightArrow, idx);

    public static event Action<bool, int> OnVerticalArrows;
    public static void VerticalArrows(this PlayerInputHandler playerInputs, bool upArrow, int idx)
        => OnVerticalArrows?.Invoke(upArrow, idx);

    public static event Action<int> OnQuitLobby;
    public static void QuitLobby(this PlayerInputHandler playerInputs, int idx)
    => OnQuitLobby?.Invoke(idx);

    public static event Action<InputAction.CallbackContext, PlayerInputHandler> OnStart;
    public static void Start(this PlayerInputHandler playerInputs, InputAction.CallbackContext context)
        => OnStart?.Invoke(context, playerInputs);

    public static event Action<Vector2, int> OnNavigate;
    public static void Navigate(this PlayerInputHandler playerInputs, Vector2 value, int idx)
        => OnNavigate?.Invoke(value, idx);

    public static event Action<int> OnValidateButton;
    public static void ValidateButton(this PlayerInputHandler playerInputs, int idx)
        => OnValidateButton?.Invoke(idx);

    public static event Action<int> OnCancelButton;
    public static void CancelButton(this PlayerInputHandler playerInputs, int idx)
        => OnCancelButton?.Invoke(idx);

    public static event Action<int> OnThirdAction;
    public static void ThirdAction(this PlayerInputHandler playerInputs, int idx)
        => OnThirdAction?.Invoke(idx);

    public static event Action<int> OnFourthAction;
    public static void FourthAction(this PlayerInputHandler playerInputs, int idx)
        => OnFourthAction?.Invoke(idx);

    public static event Action<int> OnSecondContext;
    public static void SecondContext(this PlayerInputHandler playerInputs, int idx)
        => OnSecondContext?.Invoke(idx);

    public static event Action OnScrollCurrentVBDownCall;
    public static void ScrollCurrentVBDownCall(this PlayerInputHandler playerInputs) => OnScrollCurrentVBDownCall?.Invoke();

    public static event Action OnScrollCurrentVBUpCall;
    public static void ScrollCurrentVBUpCall(this PlayerInputHandler playerInputs) => OnScrollCurrentVBUpCall?.Invoke();

    public static event Action OnScrollCurrentHBLeftCall;
    public static void ScrollCurrentHBLeftCall(this PlayerInputHandler playerInputs) => OnScrollCurrentHBLeftCall?.Invoke();

    public static event Action OnScrollCurrentHBRightCall;
    public static void ScrollCurrentHBRightCall(this PlayerInputHandler playerInputs) => OnScrollCurrentHBRightCall?.Invoke();

    public static event Action OnTryNextLineCall;
    public static void TryNextLineCall(this PlayerInputHandler playerInputs) => OnTryNextLineCall?.Invoke();

    public static event Action OnSkipDialogueCall;
    public static void SkipDialogueCall(this PlayerInputHandler playerInputs) => OnSkipDialogueCall?.Invoke();

    public static event Action<PlayerInputsManager.E_Devices> OnDeviceChange;
    public static void DeviceChange(this PlayerInputHandler playerInputs, PlayerInputsManager.E_Devices newDevice) => OnDeviceChange?.Invoke(newDevice);
}

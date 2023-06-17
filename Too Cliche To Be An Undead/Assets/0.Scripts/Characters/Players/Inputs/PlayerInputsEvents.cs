using System;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerInputsEvents
{
    public static event Action<PlayerInputs> OnPlayerInputsCreated;
    public static void PlayerInputsCreated(this PlayerInputs inputs) => OnPlayerInputsCreated?.Invoke(inputs);

    public static event Action<int, int> OnChangedIndex;
    public static void ChangedIndex(this PlayerInputs inputs, int previousIndex, int newIndex) => OnChangedIndex?.Invoke(previousIndex, newIndex);

    public static event Action<int> OnPlayerInputsDestroyed;
    public static void PlayerInputsDestroyed(this PlayerInputs inputs, int idx) => OnPlayerInputsDestroyed?.Invoke(idx);

    public static event Action OnPauseCall;
    public static void PauseCall(this PlayerInputs playerInputs) => OnPauseCall?.Invoke();

    public static event Action OnCloseMenuCall;
    public static void CloseMenuCall(this PlayerInputs playerInputs) => OnCloseMenuCall?.Invoke();

    public static event Action<bool, int> OnHorizontalArrows;
    public static void HorizontalArrows(this PlayerInputs playerInputs, bool rightArrow, int idx)
        => OnHorizontalArrows?.Invoke(rightArrow, idx);

    public static event Action<bool, int> OnVerticalArrows;
    public static void VerticalArrows(this PlayerInputs playerInputs, bool upArrow, int idx)
        => OnVerticalArrows?.Invoke(upArrow, idx);

    public static event Action<int> OnQuitLobby;
    public static void QuitLobby(this PlayerInputs playerInputs, int idx)
    => OnQuitLobby?.Invoke(idx);

    public static event Action<InputAction.CallbackContext, PlayerInputs> OnStart;
    public static void Start(this PlayerInputs playerInputs, InputAction.CallbackContext context)
        => OnStart?.Invoke(context, playerInputs);

    public static event Action<Vector2, int> OnNavigate;
    public static void Navigate(this PlayerInputs playerInputs, Vector2 value, int idx)
        => OnNavigate?.Invoke(value, idx);

    public static event Action<int> OnValidateButton;
    public static void ValidateButton(this PlayerInputs playerInputs, int idx)
        => OnValidateButton?.Invoke(idx);

    public static event Action<int> OnCancelButton;
    public static void CancelButton(this PlayerInputs playerInputs, int idx)
        => OnCancelButton?.Invoke(idx);

    public static event Action<int> OnThirdAction;
    public static void ThirdAction(this PlayerInputs playerInputs, int idx)
        => OnThirdAction?.Invoke(idx);

    public static event Action<int> OnFourthAction;
    public static void FourthAction(this PlayerInputs playerInputs, int idx)
        => OnFourthAction?.Invoke(idx);

    public static event Action<int> OnSecondContext;
    public static void SecondContext(this PlayerInputs playerInputs, int idx)
        => OnSecondContext?.Invoke(idx);

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

    public static event Action<PlayerInputsManager.E_Devices> OnDeviceChange;
    public static void DeviceChange(this PlayerInputs playerInputs, PlayerInputsManager.E_Devices newDevice) => OnDeviceChange?.Invoke(newDevice);
}

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public static class NewPlayerInputsHandlerEvents
{
    public static event Action<NewPlayerInputsHandler> OnPlayerInputsCreated;
    public static void PlayerInputsCreated(this NewPlayerInputsHandler inputs) => OnPlayerInputsCreated?.Invoke(inputs);

    public static event Action<int, int> OnChangedIndex;
    public static void ChangedIndex(this NewPlayerInputsHandler inputs, int previousIndex, int newIndex) => OnChangedIndex?.Invoke(previousIndex, newIndex);

    public static event Action<int> OnPlayerInputsDestroyed;
    public static void PlayerInputsDestroyed(this NewPlayerInputsHandler inputs, int idx) => OnPlayerInputsDestroyed?.Invoke(idx);

    public static event Action OnPauseCall;
    public static void PauseCall(this NewPlayerInputsHandler playerInputs) => OnPauseCall?.Invoke();

    public static event Action OnCloseMenuCall;
    public static void CloseMenuCall(this NewPlayerInputsHandler playerInputs) => OnCloseMenuCall?.Invoke();

    public static event Action<bool, int> OnHorizontalArrows;
    public static void HorizontalArrows(this NewPlayerInputsHandler playerInputs, bool rightArrow, int idx)
        => OnHorizontalArrows?.Invoke(rightArrow, idx);

    public static event Action<bool, int> OnVerticalArrows;
    public static void VerticalArrows(this NewPlayerInputsHandler playerInputs, bool upArrow, int idx)
        => OnVerticalArrows?.Invoke(upArrow, idx);

    public static event Action<int> OnQuitLobby;
    public static void QuitLobby(this NewPlayerInputsHandler playerInputs, int idx)
    => OnQuitLobby?.Invoke(idx);

    public static event Action<InputAction.CallbackContext, NewPlayerInputsHandler> OnStart;
    public static void Start(this NewPlayerInputsHandler playerInputs, InputAction.CallbackContext context)
        => OnStart?.Invoke(context, playerInputs);

    public static event Action<Vector2, int> OnNavigate;
    public static void Navigate(this NewPlayerInputsHandler playerInputs, Vector2 value, int idx)
        => OnNavigate?.Invoke(value, idx);

    public static event Action<int> OnValidateButton;
    public static void ValidateButton(this NewPlayerInputsHandler playerInputs, int idx)
        => OnValidateButton?.Invoke(idx);

    public static event Action<int> OnCancelButton;
    public static void CancelButton(this NewPlayerInputsHandler playerInputs, int idx)
        => OnCancelButton?.Invoke(idx);

    public static event Action<int> OnThirdAction;
    public static void ThirdAction(this NewPlayerInputsHandler   playerInputs, int idx)
        => OnThirdAction?.Invoke(idx);

    public static event Action<int> OnFourthAction;
    public static void FourthAction(this NewPlayerInputsHandler playerInputs, int idx)
        => OnFourthAction?.Invoke(idx);

    public static event Action<int> OnSecondContext;
    public static void SecondContext(this NewPlayerInputsHandler playerInputs, int idx)
        => OnSecondContext?.Invoke(idx);

    public static event Action OnScrollCurrentVBDownCall;
    public static void ScrollCurrentVBDownCall(this NewPlayerInputsHandler playerInputs) => OnScrollCurrentVBDownCall?.Invoke();

    public static event Action OnScrollCurrentVBUpCall;
    public static void ScrollCurrentVBUpCall(this NewPlayerInputsHandler playerInputs) => OnScrollCurrentVBUpCall?.Invoke();

    public static event Action OnScrollCurrentHBLeftCall;
    public static void ScrollCurrentHBLeftCall(this NewPlayerInputsHandler playerInputs) => OnScrollCurrentHBLeftCall?.Invoke();

    public static event Action OnScrollCurrentHBRightCall;
    public static void ScrollCurrentHBRightCall(this NewPlayerInputsHandler playerInputs) => OnScrollCurrentHBRightCall?.Invoke();

    public static event Action OnTryNextLineCall;
    public static void TryNextLineCall(this NewPlayerInputsHandler playerInputs) => OnTryNextLineCall?.Invoke();

    public static event Action OnSkipDialogueCall;
    public static void SkipDialogueCall(this NewPlayerInputsHandler playerInputs) => OnSkipDialogueCall?.Invoke();

    public static event Action<PlayerInputsManager.E_Devices> OnDeviceChange;
    public static void DeviceChange(this NewPlayerInputsHandler playerInputs, PlayerInputsManager.E_Devices newDevice) => OnDeviceChange?.Invoke(newDevice);
}

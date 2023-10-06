using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.XInput;
using static PlayerInputsManager;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputs : MonoBehaviourEventsHandler
{
    [field: ReadOnly, SerializeField] public PlayerCharacter Owner { get; private set; }
    [field: SerializeField] public PlayerInput Input { get; private set; }

    public InputDevice MainDevice { get; private set; }

    public E_Devices currentDeviceType { get; private set; }
    private string currentDeviceName = "";

    [field: ReadOnly] [field: SerializeField] public int InputsID { get; private set; }

    private void Reset()
    {
        Input = this.GetComponent<PlayerInput>();
    }

    protected override void EventsSubscriber()
    {
    }

    protected override void EventsUnSubscriber()
    {
    }

    protected override void Awake()
    {
        this.gameObject.transform.parent = null;
        DontDestroyOnLoad(this.gameObject);
        base.Awake();
        CheckCurrentDevice();

        SetControlMap();
        this.PlayerInputsCreated();
    }

    private void Update()
    {
        CheckCurrentDevice();
    }

    private void SetControlMap()
    {
        if (Owner == null)
        {
            SwitchControlMapToUI();
            return;
        }

        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainScene))
        {
            SwitchControlMapToInGame();
            return;
        }

        SwitchControlMapToUI();
    }

    public void ChangeIndex(int newIndex)
    {
        int lastIdx = InputsID;
        InputsID = newIndex;
        this.gameObject.name = "PlayerInputs " + newIndex;
        this.ChangedIndex(lastIdx, newIndex);
    }

    public void SetOwner(PlayerCharacter character)
    {
        Owner = character;
        SwitchControlMapToInGame();
    }

    public void AddDevice(InputDevice device)
    {
        InputUser.PerformPairingWithDevice(device, Input.user);
    }

    public void SetMainDevice(InputDevice device)
    {
        MainDevice = device;
    }

    public bool IsOnKeyboard() => currentDeviceType == E_Devices.Keyboard;

    #region ControlMaps

    public void SwitchControlMap(string map)
    {
        Input.SwitchCurrentActionMap(map);
    }
    public void SwitchControlMapToInGame()
    {
        if (Owner == null) return;
        SwitchControlMap(ACTIONMAP_INGAME);
    }
    public void SwitchControlMapToUI() =>
        SwitchControlMap(ACTIONMAP_UI);
    public void SwitchControlMapToDialogue() =>
        SwitchControlMap(ACTIONMAP_DIALOGUE);

    #endregion

    #region Device

    private void CheckCurrentDevice()
    {
        if (InputsID != 0) return;

        InputDevice device = null;

        double mostRecent = -1;
        foreach (var item in Input.devices)
        {
            if (item.lastUpdateTime > mostRecent)
            {
                mostRecent = item.lastUpdateTime;
                device = item.device;
            }
        }

        if (device == null) return;
        if (currentDeviceName == device.name) return;
        currentDeviceName = device.name;

        E_Devices newType;

        switch (device)
        {
            case InputDevice d when device is XInputController:
                newType = E_Devices.Xbox;
                break;

            case InputDevice d when device is DualShockGamepad:
                newType = E_Devices.DualShock;
                break;

            case InputDevice d when device is SwitchProControllerHID:
                newType = E_Devices.Switch;
                break;

            default:
                newType = E_Devices.Keyboard;
                break;
        }

        currentDeviceType = newType;
        SetPlayer1Device(currentDeviceType);
        this.DeviceChange(newType);
    }

    public void OnDeviceLost(PlayerInput input)
    {
    }

    public void OnDeviceRegained(PlayerInput input)
    {

    }

    public void OnControlsChanged(PlayerInput input)
    {

    }

    #endregion

    #region InGame Actions

    public void ForceReadMovements()
    {
        Input.currentActionMap.Disable();
        Input.currentActionMap.Enable();
    }
    public void OnMovements(InputAction.CallbackContext context)
    {
        Owner.ReadMovementsInputs(context);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed) Owner.OnAttackInput?.Invoke();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed) Owner.OnDashInput?.Invoke();
    }

    public void OnSkill(InputAction.CallbackContext context)
    {
        if (context.performed) Owner.OnSkillInput?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed) Owner.OnInteractInput?.Invoke();
    }

    public void IG_OnPause(InputAction.CallbackContext context)
    {
        if (context.performed) this.PauseCall();
    }

    public void OnStayStatic(InputAction.CallbackContext context)
    {
        Owner.StayStaticInput(context);
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed) Owner.OnAimInput?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnSelfRevive(InputAction.CallbackContext context)
    {
        if (context.performed) Owner.OnSelfReviveInput?.Invoke();
    }

    public void OnSecondContextual(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Owner.OnSecondContextInput?.Invoke();
            this.SecondContext(Owner.PlayerIndex);
        }
    }

    #endregion

    #region UI Actions

    public void UI_OnPause(InputAction.CallbackContext context)
    {
        if (context.performed) this.PauseCall();
    }

    public void OnCancelMenu(InputAction.CallbackContext context)
    {
        if (context.performed) this.CloseMenuCall();
    }

    public void OnArrowsLeft(InputAction.CallbackContext context)
    {
        if (context.performed) this.HorizontalArrows(false, InputsID);
    }

    public void OnArrowsRight(InputAction.CallbackContext context)
    {
        if (context.performed) this.HorizontalArrows(true, InputsID);
    }

    public void OnArrowsUp(InputAction.CallbackContext context)
    {
        if (context.performed) this.VerticalArrows(true, InputsID);
    }

    public void OnArrowsDown(InputAction.CallbackContext context)
    {
        if (context.performed) this.VerticalArrows(false, InputsID);
    }

    public void OnQuitLoby(InputAction.CallbackContext context)
    {
        if (context.performed) this.QuitLobby(InputsID);
    }

    public void OnSelect(InputAction.CallbackContext context)
    {

    }

    public void OnStart(InputAction.CallbackContext context)
    {
        this.Start(context);
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.performed) this.Navigate(context.ReadValue<Vector2>(), InputsID);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {

    }

    public void OnCancel(InputAction.CallbackContext context)
    {

    }

    public void OnPoint(InputAction.CallbackContext context)
    {

    }

    public void OnClick(InputAction.CallbackContext context)
    {

    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {

    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {

    }

    public void OnRightClick(InputAction.CallbackContext context)
    {

    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {

    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {

    }

    public void OnScrollDown(InputAction.CallbackContext context)
    {

    }

    public void OnScrollUp(InputAction.CallbackContext context)
    {

    }

    public void OnScrollLeft(InputAction.CallbackContext context)
    {

    }

    public void OnScrollRight(InputAction.CallbackContext context)
    {

    }

    public void OnValidateButton(InputAction.CallbackContext context)
    {
        if (context.performed) this.ValidateButton(InputsID);
    }

    public void OnCancelButton(InputAction.CallbackContext context)
    {
        if (context.performed) this.CancelButton(InputsID);
    }

    public void OnThirdAction(InputAction.CallbackContext context)
    {
        if (context.performed) this.ThirdAction(InputsID);
    }

    public void OnFourthAction(InputAction.CallbackContext context)
    {
        if (context.performed) this.FourthAction(InputsID);
    }

    #endregion

    #region Dialogue

    public void OnShowNextLine(InputAction.CallbackContext context)
    {
        if (context.performed) this.TryNextLineCall();
    }

    public void OnSkipDialogue(InputAction.CallbackContext context)
    {
        if (context.performed) this.SkipDialogueCall();
    }

    #endregion

    protected override void OnDestroy()
    {
        base.OnDestroy();
        this.PlayerInputsDestroyed(this.InputsID);
    }

}

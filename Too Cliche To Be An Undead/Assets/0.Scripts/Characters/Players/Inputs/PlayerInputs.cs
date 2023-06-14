using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputs : MonoBehaviourEventsHandler
{
    [SerializeField] private PlayerCharacter owner;

    private void Reset()
    {
        owner = this.GetComponentInParent<PlayerCharacter>();
    }

    protected override void EventsSubscriber()
    {
    }

    protected override void EventsUnSubscriber()
    {
    }

    #region Device

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

    public void OnMovements(InputAction.CallbackContext context)
    {
        owner.ReadMovementsInputs(context);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed) owner.OnAttackInput?.Invoke();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed) owner.OnDashInput?.Invoke();
    }

    public void OnSkill(InputAction.CallbackContext context)
    {
        if (context.performed) owner.OnSkillInput?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed) owner.OnInteractInput?.Invoke();
    }

    public void IG_OnPause(InputAction.CallbackContext context)
    {
        if (context.performed) this.PauseCall();
    }

    public void OnStayStatic(InputAction.CallbackContext context)
    {
        owner.StayStaticInput(context);
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed) owner.OnAimInput?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnSelfRevive(InputAction.CallbackContext context)
    {
        if (context.performed) owner.OnSelfReviveInput?.Invoke();
    }

    public void OnSecondContextual(InputAction.CallbackContext context)
    {
        if (context.performed) owner.OnSecondContextInput?.Invoke();
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
        if (context.performed) owner.OnHorizontalArrowInput?.Invoke(false, owner.PlayerIndex);
    }

    public void OnArrowsRight(InputAction.CallbackContext context)
    {
        if (context.performed) owner.OnHorizontalArrowInput?.Invoke(true, owner.PlayerIndex);
    }

    public void OnArrowsUp(InputAction.CallbackContext context)
    {
        if (context.performed) owner.OnVerticalArrowInput?.Invoke(true, owner.PlayerIndex);
    }

    public void OnArrowsDown(InputAction.CallbackContext context)
    {
        if (context.performed) owner.OnVerticalArrowInput?.Invoke(false, owner.PlayerIndex);
    }

    public void OnQuitLoby(InputAction.CallbackContext context)
    {
        if (context.performed) owner.OnQuitLobbyInput?.Invoke();
        //owner.QuitLobby
    }

    public void OnSelect(InputAction.CallbackContext context)
    {

    }

    public void OnStart(InputAction.CallbackContext context)
    {

    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.performed) owner.OnNavigationArrowInput?.Invoke(context.ReadValue<Vector2>(), owner.PlayerIndex);
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
        if (context.performed) owner.OnValidateInput?.Invoke();
    }

    public void OnCancelButton(InputAction.CallbackContext context)
    {
        if (context.performed) owner.OnCancelInput?.Invoke();
    }

    public void OnThirdAction(InputAction.CallbackContext context)
    {
        if (context.performed) owner.OnThirdActionButton?.Invoke();
    }

    public void OnFourthAction(InputAction.CallbackContext context)
    {
        if (context.performed) owner.OnFourthActionButton?.Invoke();
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

}

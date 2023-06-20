
public class FSM_Player_Pushed : FSM_Entity_Pushed<FSM_Player_Manager>
{
    private PlayerCharacter playerOwner;

    private const string ONHIT_MODIFIER_ID = "PUSHED_";

    public FSM_Player_Manager.E_PlayerState StateName { get; private set; }

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        base.EnterState(stateManager);

        playerOwner.Weapon.AddOnHitEffect(new OnHitEffects
            (_owner: playerOwner,
            _id: ONHIT_MODIFIER_ID,
            _rangeModifier: 1,
            _damagesModifier: 1,
            _knockbackModifier: 1,
            _cameraShakeIntensityModifier: 1,
            null,
            -1,
            -1,
            true
            ));
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        base.UpdateState(stateManager);

        stateManager.OwnerWeapon.SetRotation();
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        base.ExitState(stateManager);
        playerOwner.Weapon.RemoveOnHitEffect(ONHIT_MODIFIER_ID);
        playerOwner.PlayerInputsComponent.ForceReadMovements();
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(FSM_Player_Manager.E_PlayerState.Idle);
        this.CheckDying(stateManager);
    }

    public override void Setup(FSM_Player_Manager stateManager)
    {
        owner = stateManager.Owner;
        playerOwner = owner as PlayerCharacter;
        StateName = FSM_Player_Manager.E_PlayerState.Pushed;
    }

    protected override void EventsSubscriber(FSM_Player_Manager stateManager)
    {
        base.EventsSubscriber(stateManager);
        playerOwner.OnAttackInput += playerOwner.Weapon.AskForAttack;
        playerOwner.OnSkillInput += playerOwner.GetSkillHolder.StartSkill;
        playerOwner.OnAskForStun += stateManager.SwitchToStun;
        CinematicManagerEvents.OnChangeCinematicState += stateManager.CinematicStateChange;
    }

    protected override void EventsUnsubscriber(FSM_Player_Manager stateManager)
    {
        base.EventsUnsubscriber(stateManager);
        playerOwner.OnAttackInput -= playerOwner.Weapon.AskForAttack;
        playerOwner.OnSkillInput -= playerOwner.GetSkillHolder.StartSkill;
        playerOwner.OnAskForStun -= stateManager.SwitchToStun;
        CinematicManagerEvents.OnChangeCinematicState -= stateManager.CinematicStateChange;
    }

    public override string ToString() => StateName.ToString();
}

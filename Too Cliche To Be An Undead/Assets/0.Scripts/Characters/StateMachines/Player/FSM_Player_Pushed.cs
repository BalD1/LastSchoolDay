
public class FSM_Player_Pushed : FSM_Entity_Pushed<FSM_Player, FSM_Player.E_PlayerState>
{
    private PlayerCharacter playerOwner;

    private const string ONHIT_MODIFIER_ID = "PUSHED_";

    public override void EnterState(FSM_Player stateManager)
    {
        base.EnterState(stateManager);

        //playerOwner.Weapon.AddOnHitEffect(new OnHitEffects
        //    (_owner: playerOwner,
        //    _id: ONHIT_MODIFIER_ID,
        //    _rangeModifier: 1,
        //    _damagesModifier: 1,
        //    _knockbackModifier: 1,
        //    _cameraShakeIntensityModifier: 1,
        //    null,
        //    -1,
        //    -1,
        //    true
        //    ));
    }

    public override void UpdateState(FSM_Player stateManager)
    {
        base.UpdateState(stateManager);

        stateManager.OwnerWeapon.SetRotation();
    }

    public override void FixedUpdateState(FSM_Player stateManager)
    {
    }

    public override void ExitState(FSM_Player stateManager)
    {
        base.ExitState(stateManager);
        //playerOwner.Weapon.RemoveOnHitEffect(ONHIT_MODIFIER_ID);
        //playerOwner.PlayerInputsComponent.ForceReadMovements();
    }

    public override void Conditions(FSM_Player stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(FSM_Player.E_PlayerState.Idle);
        this.CheckDying(stateManager);
    }

    public override void Setup(FSM_Player stateManager)
    {
        owner = stateManager.Owner;
        playerOwner = owner as PlayerCharacter;
        StateKey = FSM_Player.E_PlayerState.Pushed;
    }

    protected override void EventsSubscriber(FSM_Player stateManager)
    {
        base.EventsSubscriber(stateManager);
        //playerOwner.OnAttackInput += playerOwner.Weapon.AskForAttack;
        //playerOwner.OnSkillInput += playerOwner.GetSkillHolder.StartSkill;
        playerOwner.OnAskForStun += stateManager.SwitchToStun;
        CinematicManagerEvents.OnChangeCinematicState += stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue += stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue += stateManager.DialogueEnded;
    }

    protected override void EventsUnsubscriber(FSM_Player stateManager)
    {
        base.EventsUnsubscriber(stateManager);
        //playerOwner.OnAttackInput -= playerOwner.Weapon.AskForAttack;
        //playerOwner.OnSkillInput -= playerOwner.GetSkillHolder.StartSkill;
        playerOwner.OnAskForStun -= stateManager.SwitchToStun;
        CinematicManagerEvents.OnChangeCinematicState -= stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue -= stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue -= stateManager.DialogueEnded;
    }

    public override string ToString() => StateKey.ToString();
}

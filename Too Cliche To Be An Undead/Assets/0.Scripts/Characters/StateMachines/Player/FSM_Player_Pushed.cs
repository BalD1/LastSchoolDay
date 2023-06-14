
public class FSM_Player_Pushed : FSM_Entity_Pushed<FSM_Player_Manager>
{
    private PlayerCharacter playerOwner;

    private const string ONHIT_MODIFIER_ID = "PUSHED_";

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
        //playerOwner.Movements();
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        base.ExitState(stateManager);
        (owner as PlayerCharacter).ForceUpdateMovementsInput();

        playerOwner.Weapon.RemoveOnHitEffect(ONHIT_MODIFIER_ID);
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(stateManager.IdleState);
    }

    public override void Setup(FSM_Player_Manager stateManager)
    {
        owner = stateManager.Owner;
        playerOwner = owner as PlayerCharacter;
    }

    protected override void EventsSubscriber()
    {
        base.EventsSubscriber();
        playerOwner.OnAttackInput += playerOwner.Weapon.AskForAttack;
        playerOwner.OnSkillInput += playerOwner.GetSkillHolder.StartSkill;
    }

    protected override void EventsUnsubscriber()
    {
        base.EventsUnsubscriber();
        playerOwner.OnAttackInput -= playerOwner.Weapon.AskForAttack;
        playerOwner.OnSkillInput -= playerOwner.GetSkillHolder.StartSkill;
    }

    public override string ToString() => "Pushed";
}

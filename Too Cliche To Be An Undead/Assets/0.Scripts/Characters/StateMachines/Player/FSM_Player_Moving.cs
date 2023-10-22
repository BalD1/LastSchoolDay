using UnityEngine;

public class FSM_Player_Moving : FSM_Base<FSM_Player_Manager, FSM_Player_Manager.E_PlayerState>
{
    private PlayerCharacter owner;

    public FSM_Player_Manager.E_PlayerState StateName { get; private set; }

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        base.EnterState(stateManager);
        owner.canBePushed = true;
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        stateManager.OwnerWeapon.SetRotation();
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
        owner.PlayerMotor?.Movements();
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        base.ExitState(stateManager);
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        // Si la velocité du personnage est à 0, on le passe en Idle
        if (owner.PlayerMotor.Velocity.Equals(Vector2.zero))
            stateManager.SwitchState(FSM_Player_Manager.E_PlayerState.Idle);

        if (stateManager.OwnerWeapon.isAttacking)
            stateManager.SwitchState(FSM_Player_Manager.E_PlayerState.Attacking);

        this.CheckDying(stateManager);
    }

    protected override void EventsSubscriber(FSM_Player_Manager stateManager)
    {
        owner.OnAttackInput += owner.Weapon.AskForAttack;

        owner.OnSkillInput += owner.GetSkillHolder.StartSkill;
        owner.OnAskForSkill += stateManager.SwitchToSkill;

        owner.OnDashInput += stateManager.DashConditions;
        owner.OnAskForPush += stateManager.PushConditions;
        owner.OnAskForStun += stateManager.SwitchToStun;
        CinematicManagerEvents.OnChangeCinematicState += stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue += stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue += stateManager.DialogueEnded;
    }

    protected override void EventsUnsubscriber(FSM_Player_Manager stateManager)
    {
        owner.OnAttackInput -= owner.Weapon.AskForAttack;

        owner.OnSkillInput -= owner.GetSkillHolder.StartSkill;
        owner.OnAskForSkill -= stateManager.SwitchToSkill;

        owner.OnDashInput -= stateManager.DashConditions;
        owner.OnAskForPush -= stateManager.PushConditions;
        owner.OnAskForStun -= stateManager.SwitchToStun;
        CinematicManagerEvents.OnChangeCinematicState -= stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue -= stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue -= stateManager.DialogueEnded;
    }

    public override void Setup(FSM_Player_Manager stateManager)
    {
        owner = stateManager.Owner;
        StateName = FSM_Player_Manager.E_PlayerState.Moving;
    }

    public override string ToString() => this.StateName.ToString();
}

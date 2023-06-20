using UnityEngine;

public class FSM_Player_Idle : FSM_Entity_Idle<FSM_Player_Manager>
{
    private PlayerCharacter playerOwner;

    public FSM_Player_Manager.E_PlayerState StateName { get; private set; }

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        base.EnterState(stateManager);
        playerOwner.SetAllVelocity(Vector2.zero);

        playerOwner.canBePushed = true;
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        stateManager.OwnerWeapon.SetRotation();

        playerOwner.AnimationController.FlipSkeletonOnMouseOrGamepad();
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        base.ExitState(stateManager);
    }

    public override void Setup(FSM_Player_Manager stateManager)
    {
        owner = stateManager.Owner;
        playerOwner = owner as PlayerCharacter;
        StateName = FSM_Player_Manager.E_PlayerState.Idle;
    }

    protected override void EventsSubscriber(FSM_Player_Manager stateManager)
    {
        base.EventsSubscriber(stateManager);
        playerOwner.OnAttackInput += playerOwner.Weapon.AskForAttack;

        playerOwner.OnSkillInput += playerOwner.GetSkillHolder.StartSkill;
        playerOwner.OnAskForSkill += stateManager.SwitchToSkill;

        playerOwner.OnDashInput += stateManager.DashConditions;
        playerOwner.OnAskForPush += stateManager.PushConditions;
        playerOwner.OnAskForStun += stateManager.SwitchToStun;
        CinematicManagerEvents.OnChangeCinematicState += stateManager.CinematicStateChange;
    }

    protected override void EventsUnsubscriber(FSM_Player_Manager stateManager)
    {
        base.EventsUnsubscriber(stateManager);
        playerOwner.OnAttackInput -= playerOwner.Weapon.AskForAttack;

        playerOwner.OnSkillInput -= playerOwner.GetSkillHolder.StartSkill;
        playerOwner.OnAskForSkill -= stateManager.SwitchToSkill;

        playerOwner.OnDashInput -= stateManager.DashConditions;
        playerOwner.OnAskForPush -= stateManager.PushConditions;
        playerOwner.OnAskForStun -= stateManager.SwitchToStun;
        CinematicManagerEvents.OnChangeCinematicState -= stateManager.CinematicStateChange;
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        // Si la vélocité du personnage n'est pas à 0, on le passe en Moving
        
        if (playerOwner.GetRb.velocity != Vector2.zero ||
            playerOwner.Velocity != Vector2.zero)
        {
            stateManager.SwitchState(FSM_Player_Manager.E_PlayerState.Moving);
        }

        if (stateManager.OwnerWeapon.isAttacking)
            stateManager.SwitchState(FSM_Player_Manager.E_PlayerState.Attacking);

        this.CheckDying(stateManager);
    }

    public override string ToString() => StateName.ToString();
}

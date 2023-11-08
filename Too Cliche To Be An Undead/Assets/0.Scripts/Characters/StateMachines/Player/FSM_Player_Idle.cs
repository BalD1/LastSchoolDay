using UnityEngine;

public class FSM_Player_Idle : FSM_Entity_Idle<FSM_Player, FSM_Player.E_PlayerState>
{
    private PlayerCharacter playerOwner;

    public FSM_Player.E_PlayerState StateName { get; private set; }

    public override void EnterState(FSM_Player stateManager)
    {
        base.EnterState(stateManager);
        
        //playerOwner.canBePushed = true;
    }

    public override void UpdateState(FSM_Player stateManager)
    {
        stateManager.OwnerWeapon.SetRotation();

    }

    public override void ExitState(FSM_Player stateManager)
    {
        base.ExitState(stateManager);
    }

    public override void Setup(FSM_Player stateManager)
    {
        owner = stateManager.Owner;
        playerOwner = owner as PlayerCharacter;
        StateName = FSM_Player.E_PlayerState.Idle;
    }

    protected override void EventsSubscriber(FSM_Player stateManager)
    {
        base.EventsSubscriber(stateManager);
        //playerOwner.OnAttackInput += playerOwner.Weapon.AskForAttack;

        //playerOwner.OnSkillInput += playerOwner.GetSkillHolder.StartSkill;
        //playerOwner.OnAskForSkill += stateManager.SwitchToSkill;

        //playerOwner.OnDashInput += stateManager.DashConditions;
        //playerOwner.OnAskForPush += stateManager.PushConditions;
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
        //playerOwner.OnAskForSkill -= stateManager.SwitchToSkill;

        //playerOwner.OnDashInput -= stateManager.DashConditions;
        //playerOwner.OnAskForPush -= stateManager.PushConditions;
        playerOwner.OnAskForStun -= stateManager.SwitchToStun;
        CinematicManagerEvents.OnChangeCinematicState -= stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue -= stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue -= stateManager.DialogueEnded;
    }

    public override void Conditions(FSM_Player stateManager)
    {
        // Si la vélocité du personnage n'est pas à 0, on le passe en Moving
        
        //if (playerOwner.GetRb.velocity != Vector2.zero ||
            //playerOwner.PlayerMotor.Velocity != Vector2.zero)
        //{
            //stateManager.SwitchState(FSM_Player_Manager.E_PlayerState.Moving);
        //}

        if (stateManager.OwnerWeapon.isAttacking)
            stateManager.SwitchState(FSM_Player.E_PlayerState.Attacking);

        this.CheckDying(stateManager);
    }

    public override string ToString() => StateName.ToString();
}

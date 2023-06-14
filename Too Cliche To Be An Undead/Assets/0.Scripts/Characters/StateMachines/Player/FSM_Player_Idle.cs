using UnityEngine;

public class FSM_Player_Idle : FSM_Entity_Idle<FSM_Player_Manager>
{
    private PlayerCharacter playerOwner;

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
    }

    protected override void EventsSubscriber()
    {
        base.EventsSubscriber();
        playerOwner.OnAttackInput += playerOwner.Weapon.AskForAttack;
        playerOwner.OnSkillInput += playerOwner.GetSkillHolder.StartSkill;
        playerOwner.OnDashInput += playerOwner.StartDash;
    }

    protected override void EventsUnsubscriber()
    {
        base.EventsUnsubscriber();
        playerOwner.OnAttackInput -= playerOwner.Weapon.AskForAttack;
        playerOwner.OnSkillInput -= playerOwner.GetSkillHolder.StartSkill;
        playerOwner.OnDashInput -= playerOwner.StartDash;
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        // Si la vélocité du personnage n'est pas à 0, on le passe en Moving
        
        if (playerOwner.GetRb.velocity != Vector2.zero ||
            playerOwner.Velocity != Vector2.zero)
        {
            stateManager.SwitchState(stateManager.MovingState);
        }

        if (stateManager.OwnerWeapon.isAttacking)
            stateManager.SwitchState(stateManager.AttackingState);

        if (playerOwner.isDashing) stateManager.SwitchState(stateManager.DashingState);
        
    }
}

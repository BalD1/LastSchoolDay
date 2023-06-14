using UnityEngine;

public class FSM_Player_Moving : FSM_Base<FSM_Player_Manager>
{
    private PlayerCharacter owner;

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
        owner.Movements();
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        base.ExitState(stateManager);
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        // Si la velocité du personnage est à 0, on le passe en Idle
        if (owner.Velocity.Equals(Vector2.zero))
            stateManager.SwitchState(stateManager.IdleState);

        if (stateManager.OwnerWeapon.isAttacking)
            stateManager.SwitchState(stateManager.AttackingState);

        if (owner.isDashing)
            stateManager.SwitchState(stateManager.DashingState);
        
    }

    protected override void EventsSubscriber()
    {
        owner.OnAttackInput += owner.Weapon.AskForAttack;
        owner.OnSkillInput += owner.GetSkillHolder.StartSkill;
        owner.OnDashInput += owner.StartDash;
    }

    protected override void EventsUnsubscriber()
    {
        owner.OnAttackInput -= owner.Weapon.AskForAttack;
        owner.OnSkillInput -= owner.GetSkillHolder.StartSkill;
        owner.OnDashInput -= owner.StartDash;
    }

    public override void Setup(FSM_Player_Manager stateManager)
    {
        owner = stateManager.Owner;
    }

    public override string ToString() => "Moving";
}

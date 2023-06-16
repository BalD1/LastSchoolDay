using UnityEngine;

public class FSM_Player_Attacking : FSM_Base<FSM_Player_Manager>
{
    public PlayerCharacter owner;
    private FSM_Player_Manager stateManager;

    public FSM_Player_Manager.E_PlayerState StateName { get; private set; }

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        base.EnterState(stateManager);

        Vector2 mouseDir = stateManager.Owner.Weapon.GetGeneralDirectionOfMouseOrGamepad();

        owner.canBePushed = true;

        PlayerAnimationController ownerAnims = owner.AnimationController;

        if (ownerAnims.animationsData == null) return;

        switch (owner.Weapon.GetGeneralDirectionOfMouseOrGamepad())
        {
            case Vector2 v when v == Vector2.up:
                ownerAnims.FlipSkeleton(false);
                ownerAnims.SetAnimation(ownerAnims.animationsData.AttackAnim_up, false);
                break;

            case Vector2 v when v == Vector2.down:
                ownerAnims.FlipSkeleton(true);
                ownerAnims.SetAnimation(ownerAnims.animationsData.AttackAnim_down, false);
                break;

            case Vector2 v when v == Vector2.left:
                ownerAnims.FlipSkeleton(false);
                ownerAnims.SetAnimation(ownerAnims.animationsData.AttackAnim_side, false);
                break;

            case Vector2 v when v == Vector2.right:
                ownerAnims.FlipSkeleton(true);
                ownerAnims.SetAnimation(ownerAnims.animationsData.AttackAnim_side, false);
                break;
        }
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        base.ExitState(stateManager);
        owner.CancelAttackAnimation();
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        if (owner.IsAlive() == false) return;

        if (owner.Weapon.attackEnded)
        {
            owner.Weapon.SetAttackEnded(false);
            owner.StateManager.SwitchState(FSM_Player_Manager.E_PlayerState.Idle);
        }

        this.CheckDying(stateManager);
    }

    protected override void EventsSubscriber(FSM_Player_Manager stateManager)
    {
        owner.Weapon.D_nextAttack += NextAttack;
        owner.OnAttackInput += owner.Weapon.AskForAttack;
        owner.OnDashInput += stateManager.DashConditions;
        owner.OnAskForStun += stateManager.SwitchToStun;
        owner.OnAttackEnded += OnAttackEnded;
    }

    protected override void EventsUnsubscriber(FSM_Player_Manager stateManager)
    {
        owner.Weapon.D_nextAttack -= NextAttack;
        owner.OnAttackInput -= owner.Weapon.AskForAttack;
        owner.OnDashInput -= stateManager.DashConditions;
        owner.OnAskForStun -= stateManager.SwitchToStun;
        owner.OnAttackEnded -= OnAttackEnded;
    }

    private void OnAttackEnded() => stateManager.SwitchState(FSM_Player_Manager.E_PlayerState.Idle);

    public void NextAttack(int currentAttackIndex)
    {
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_ATTACKINDEX, currentAttackIndex);
    }

    public override void Setup(FSM_Player_Manager stateManager)
    {
        owner = stateManager.Owner;
        this.stateManager = stateManager;
        StateName = FSM_Player_Manager.E_PlayerState.Attacking;
    }

    public override string ToString() => StateName.ToString();
}

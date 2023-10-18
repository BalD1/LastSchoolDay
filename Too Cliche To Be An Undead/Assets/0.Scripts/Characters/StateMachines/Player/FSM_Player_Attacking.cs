using UnityEngine;

public class FSM_Player_Attacking : FSM_PlayerState
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

        if (ownerAnims.AnimationsData == null) return;

        switch (owner.Weapon.GetGeneralDirectionOfMouseOrGamepad())
        {
            case Vector2 v when v == Vector2.up:
                ownerAnims.TryFlipSkeleton(false);
                ownerAnims.SetAnimation(ownerAnims.AnimationsData.AttackAnim_up, false);
                break;

            case Vector2 v when v == Vector2.down:
                ownerAnims.TryFlipSkeleton(true);
                ownerAnims.SetAnimation(ownerAnims.AnimationsData.AttackAnim_down, false);
                break;

            case Vector2 v when v == Vector2.left:
                ownerAnims.TryFlipSkeleton(false);
                ownerAnims.SetAnimation(ownerAnims.AnimationsData.AttackAnim_side, false);
                break;

            case Vector2 v when v == Vector2.right:
                ownerAnims.TryFlipSkeleton(true);
                ownerAnims.SetAnimation(ownerAnims.AnimationsData.AttackAnim_side, false);
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
        owner.PlayerInputsComponent.ForceReadMovements();
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
        owner.OnAttackInput += owner.Weapon.AskForAttack;
        owner.OnDashInput += stateManager.DashConditions;
        owner.OnAskForStun += stateManager.SwitchToStun;
        owner.OnAttackEnded += OnAttackEnded;
        CinematicManagerEvents.OnChangeCinematicState += stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue += stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue += stateManager.DialogueEnded;
    }

    protected override void EventsUnsubscriber(FSM_Player_Manager stateManager)
    {
        owner.OnAttackInput -= owner.Weapon.AskForAttack;
        owner.OnDashInput -= stateManager.DashConditions;
        owner.OnAskForStun -= stateManager.SwitchToStun;
        owner.OnAttackEnded -= OnAttackEnded;
        CinematicManagerEvents.OnChangeCinematicState -= stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue -= stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue -= stateManager.DialogueEnded;
    }

    private void OnAttackEnded() => stateManager.SwitchState(FSM_Player_Manager.E_PlayerState.Idle);

    public override void Setup(FSM_Player_Manager stateManager)
    {
        owner = stateManager.Owner;
        this.stateManager = stateManager;
        StateName = FSM_Player_Manager.E_PlayerState.Attacking;
    }

    public override string ToString() => StateName.ToString();
}

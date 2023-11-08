using UnityEngine;

public class FSM_Player_Attacking : FSM_Base<FSM_Player, FSM_Player.E_PlayerState>
{
    public PlayerCharacter owner;
    private FSM_Player stateManager;

    public FSM_Player.E_PlayerState StateName { get; private set; }

    public override void EnterState(FSM_Player stateManager)
    {
        base.EnterState(stateManager);

        //Vector2 mouseDir = stateManager.Owner.Weapon.GetGeneralDirectionOfMouseOrGamepad();

        ////owner.canBePushed = true;

        //PlayerAnimationController ownerAnims = owner.AnimationController;

        //if (owner.AnimationsData == null) return;

        //switch (owner.Weapon.GetGeneralDirectionOfMouseOrGamepad())
        //{
        //    case Vector2 v when v == Vector2.up:
        //        ownerAnims.TryFlipSkeleton(false);
        //        //ownerAnims.SetAnimation(owner.AnimationsData.AttackAnim_up, false);
        //        break;

        //    case Vector2 v when v == Vector2.down:
        //        ownerAnims.TryFlipSkeleton(true);
        //        //ownerAnims.SetAnimation(owner.AnimationsData.AttackAnim_down, false);
        //        break;

        //    case Vector2 v when v == Vector2.left:
        //        ownerAnims.TryFlipSkeleton(false);
        //        //ownerAnims.SetAnimation(owner.AnimationsData.AttackAnim_side, false);
        //        break;

        //    case Vector2 v when v == Vector2.right:
        //        ownerAnims.TryFlipSkeleton(true);
        //        //ownerAnims.SetAnimation(owner.AnimationsData.AttackAnim_side, false);
        //        break;
        //}
    }

    public override void UpdateState(FSM_Player stateManager)
    {
    }

    public override void FixedUpdateState(FSM_Player stateManager)
    {
    }

    public override void ExitState(FSM_Player stateManager)
    {
        base.ExitState(stateManager);
        //owner.CancelAttackAnimation();
        //owner.PlayerInputsComponent.ForceReadMovements();
    }

    public override void Conditions(FSM_Player stateManager)
    {
        //if (owner.IsAlive() == false) return;

        //if (owner.Weapon.attackEnded)
        //{
        //    owner.Weapon.SetAttackEnded(false);
        //    owner.StateManager.SwitchState(FSM_Player_Manager.E_PlayerState.Idle);
        //}

        this.CheckDying(stateManager);
    }

    protected override void EventsSubscriber(FSM_Player stateManager)
    {
        //owner.OnAttackInput += owner.Weapon.AskForAttack;
        //owner.OnDashInput += stateManager.DashConditions;
        //owner.OnAskForStun += stateManager.SwitchToStun;
        //owner.OnAttackEnded += OnAttackEnded;
        CinematicManagerEvents.OnChangeCinematicState += stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue += stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue += stateManager.DialogueEnded;
    }

    protected override void EventsUnsubscriber(FSM_Player stateManager)
    {
        //owner.OnAttackInput -= owner.Weapon.AskForAttack;
        //owner.OnDashInput -= stateManager.DashConditions;
        //owner.OnAskForStun -= stateManager.SwitchToStun;
        //owner.OnAttackEnded -= OnAttackEnded;
        CinematicManagerEvents.OnChangeCinematicState -= stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue -= stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue -= stateManager.DialogueEnded;
    }

    private void OnAttackEnded() => stateManager.SwitchState(FSM_Player.E_PlayerState.Idle);

    public override void Setup(FSM_Player stateManager)
    {
        owner = stateManager.Owner;
        this.stateManager = stateManager;
        StateName = FSM_Player.E_PlayerState.Attacking;
    }

    public override string ToString() => StateName.ToString();
}

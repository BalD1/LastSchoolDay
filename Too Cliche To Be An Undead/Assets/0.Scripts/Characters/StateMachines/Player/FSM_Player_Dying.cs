using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;

public class FSM_Player_Dying : FSM_Base<FSM_Player_Manager>
{
    public PlayerCharacter owner;
    private float dyingState_TIMER;
    public float DyingState_TIMER { get => dyingState_TIMER; }

    private bool hadSelfRevive = false;

    private bool isFake = false;

    public FSM_Player_Manager.E_PlayerState StateName { get; private set; }

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        base.EnterState(stateManager);

        if (dyingState_TIMER <= 0) dyingState_TIMER = owner.DyingState_DURATION;
        owner.canBePushed = false;
        hadSelfRevive = false;
        if (owner.selfReviveCount <= 0) hadSelfRevive = true;
        else owner.SelfReviveText.enabled = true;

        this.EnteredDying(owner);
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        if (isFake) return;

        dyingState_TIMER -= Time.deltaTime;
        owner.PlayerHUD.FillPortrait(dyingState_TIMER / owner.DyingState_DURATION);
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        base.ExitState(stateManager);
        this.ExitedDying(owner);
        owner.SelfReviveText.enabled = false;
        isFake = false;
        owner.PlayerInputsComponent.ForceReadMovements();
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        if (dyingState_TIMER <= 0) stateManager.SwitchState(FSM_Player_Manager.E_PlayerState.Dead);
    }

    protected override void EventsSubscriber(FSM_Player_Manager stateManager)
    {
        if (!isFake) owner.OnOtherInteract += Revive;
        owner.OnRevive += Revive;
        owner.OnSelfReviveInput += SelfRevive;
        CinematicManagerEvents.OnChangeCinematicState += stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue += stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue += stateManager.DialogueEnded;
    }

    protected override void EventsUnsubscriber(FSM_Player_Manager stateManager)
    {
        if (!isFake) owner.OnOtherInteract -= Revive;
        owner.OnRevive -= Revive;
        owner.OnSelfReviveInput -= SelfRevive;
        CinematicManagerEvents.OnChangeCinematicState -= stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue -= stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue -= stateManager.DialogueEnded;
    }

    private void SelfRevive()
    {
        if (owner.selfReviveCount >= 1)
        {
            owner.selfReviveCount -= 1;
            owner.AskRevive();
        }
    }

    private void Revive(GameObject interactor) => Revive();
    private void Revive()
    {
        owner.OnHeal(owner.MaxHP_M * owner.ReviveHealthPercentage, false, false, healFromDeath: true);
        owner.StateManager.SwitchState(FSM_Player_Manager.E_PlayerState.Idle);
    }

    public void SetAsFakeState()
    {
        isFake = true;
        owner.OnOtherInteract -= Revive;
        owner.OnSelfReviveInput -= SelfRevive;
    }

    public override void Setup(FSM_Player_Manager stateManager)
    {
        owner = stateManager.Owner;
        StateName = FSM_Player_Manager.E_PlayerState.Dying;
    }

    public override string ToString() => StateName.ToString();
}

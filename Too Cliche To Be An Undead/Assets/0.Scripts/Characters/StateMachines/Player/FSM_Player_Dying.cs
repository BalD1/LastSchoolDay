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

    private bool removedAlive = false;
    public bool RemovedAlive { get => removedAlive; }

    private bool isFake = false;

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        base.EnterState(stateManager);

        if (dyingState_TIMER <= 0) dyingState_TIMER = owner.DyingState_DURATION;
        owner.canBePushed = false;
        removedAlive = false;
        if (owner.selfReviveCount <= 0)
        {
            PlayersManager.Instance.RemoveAlivePlayer(owner.transform);
            removedAlive = true;
        }
        else
        {
            StringBuilder sb = new StringBuilder("Press ");

            InputDevice d = owner.Inputs.devices[0];

            if (d is XInputController) sb.Append("Y");
            else if (d is DualShockGamepad) sb.Append("TRIANGLE");
            else if (d is SwitchProControllerHID) sb.Append("X");
            else sb.Append("E");

            sb.Append(" to revive \n (");
            sb.Append(owner.selfReviveCount);
            sb.Append(" left)");

            owner.SelfReviveText.text = sb.ToString();
            owner.SelfReviveText.enabled = true;
        }
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        if (isFake) return;

        dyingState_TIMER -= Time.deltaTime;
        owner.PlayerHUD.FillPortrait(dyingState_TIMER / owner.DyingState_DURATION);
        if (dyingState_TIMER <= 0) owner.DefinitiveDeath();
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        base.ExitState(stateManager);

        owner.ForceUpdateMovementsInput();

        if (removedAlive)
            PlayersManager.Instance.AddAlivePlayer();

        owner.SelfReviveText.enabled = false;
        isFake = false;


    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
    }

    protected override void EventsSubscriber()
    {
        if (!isFake) owner.OnOtherInteract += owner.Revive;
        owner.OnRevive += Revive;
        owner.OnSelfReviveInput += SelfRevive;
    }

    protected override void EventsUnsubscriber()
    {
        if (!isFake) owner.OnOtherInteract -= owner.Revive;
        owner.OnRevive -= Revive;
        owner.OnSelfReviveInput -= SelfRevive;
    }

    private void SelfRevive()
    {
        if (owner.selfReviveCount >= 1)
        {
            owner.selfReviveCount -= 1;
            owner.Revive();
        }
    }

    private void Revive()
    {
        owner.StateManager.SwitchState<FSM_Player_Idle>(FSM_Player_Manager.E_PlayerState.Idle);
    }

    public void SetAsFakeState()
    {
        isFake = true;
        owner.OnOtherInteract -= owner.Revive;
        owner.OnSelfReviveInput -= SelfRevive;
    }

    public override void Setup(FSM_Player_Manager stateManager)
    {
        owner = stateManager.Owner;
    }

    public override string ToString() => "Dying";
}

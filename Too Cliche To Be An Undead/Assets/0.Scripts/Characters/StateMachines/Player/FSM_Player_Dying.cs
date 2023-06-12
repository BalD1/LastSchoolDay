using System.Collections;
using System.Collections.Generic;
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
        owner ??= stateManager.Owner;

        if (dyingState_TIMER <= 0) dyingState_TIMER = owner.DyingState_DURATION;

        if (!isFake) owner.OnInteract += owner.Revive;

        owner.SetAnimatorArgs("Dying", true);

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

        owner.OnRevive += Revive;
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
        owner.SetAnimatorArgs("Dying", false);
        owner.ForceUpdateMovementsInput();

        if (!isFake) owner.OnInteract -= owner.Revive;

        if (removedAlive)
            PlayersManager.Instance.AddAlivePlayer();

        owner.SelfReviveText.enabled = false;
        isFake = false;
        owner.OnRevive -= Revive;
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
    }

    private void Revive()
    {
        owner.StateManager.SwitchState<FSM_Player_Idle>(FSM_Player_Manager.E_PlayerState.Idle, true);
    }

    public void SetAsFakeState()
    {
        isFake = true;
        owner.OnInteract -= owner.Revive;
    }

    public override string ToString() => "Dying";
}

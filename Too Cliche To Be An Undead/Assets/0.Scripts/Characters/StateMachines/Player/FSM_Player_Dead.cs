using UnityEngine;

public class FSM_Player_Dead : FSM_Base<FSM_Player, FSM_Player.E_PlayerState>
{
    public PlayerCharacter owner;

    public override void EnterState(FSM_Player stateManager)
    {
        base.EnterState(stateManager);

        //if (owner.selfReviveCount > 0)
        //{
        //    owner.selfReviveCount -= 1;
        //    //owner.Heal(owner.MaxHP_M * owner.ReviveHealthPercentage, false, false, healFromDeath: true);
        //    owner.StateManager.SwitchState(FSM_Player_Manager.E_PlayerState.Idle);
        //    return;
        //}

        //owner.MinimapMarker.SetActive(false);

        this.EnteredDeath(owner);
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
        //owner.PlayerInputsComponent.ForceReadMovements();
    }

    public override void Conditions(FSM_Player stateManager)
    {
    }

    protected override void EventsSubscriber(FSM_Player stateManager)
    {
    }

    protected override void EventsUnsubscriber(FSM_Player stateManager)
    {
    }

    public override void Setup(FSM_Player stateManager)
    {
        owner = stateManager.Owner;
        StateKey = FSM_Player.E_PlayerState.Dead;
    }

    public override string ToString() => StateKey.ToString();
}

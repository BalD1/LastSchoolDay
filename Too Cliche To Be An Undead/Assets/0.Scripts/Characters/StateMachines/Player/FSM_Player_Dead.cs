using UnityEngine;

public class FSM_Player_Dead : FSM_Base<FSM_Player_Manager>
{
    public PlayerCharacter owner;

    public FSM_Player_Manager.E_PlayerState StateName { get; private set; }

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        base.EnterState(stateManager);

        if (owner.selfReviveCount > 0)
        {
            owner.selfReviveCount -= 1;
            owner.OnHeal(owner.MaxHP_M * owner.ReviveHealthPercentage, false, false, healFromDeath: true);
            owner.StateManager.SwitchState(FSM_Player_Manager.E_PlayerState.Idle);
            return;
        }

        owner.MinimapMarker.SetActive(false);

        this.EnteredDeath(owner);
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
        owner.PlayerInputsComponent.ForceReadMovements();
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
    }

    protected override void EventsSubscriber(FSM_Player_Manager stateManager)
    {
    }

    protected override void EventsUnsubscriber(FSM_Player_Manager stateManager)
    {
    }

    public override void Setup(FSM_Player_Manager stateManager)
    {
        owner = stateManager.Owner;
        StateName = FSM_Player_Manager.E_PlayerState.Dead;
    }

    public override string ToString() => StateName.ToString();
}

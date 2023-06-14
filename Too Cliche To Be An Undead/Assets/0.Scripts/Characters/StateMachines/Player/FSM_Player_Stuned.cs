
public class FSM_Player_Stuned : FSM_Entity_Stunned<FSM_Player_Manager>
{
    public override void EnterState(FSM_Player_Manager stateManager)
    {
        base.EnterState(stateManager);

        owner.canBePushed = true;
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(stateManager.IdleState);
    }

    public override void Setup(FSM_Player_Manager stateManager)
    {
        owner = stateManager.Owner;
    }

    public override string ToString() => "Stunned";
}

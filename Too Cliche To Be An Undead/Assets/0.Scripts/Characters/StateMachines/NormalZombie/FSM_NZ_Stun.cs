
public class FSM_NZ_Stun : FSM_Entity_Stunned<FSM_NZ_Manager>
{
    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        base.EnterState(stateManager);

        owner.canBePushed = true;
        (owner as NormalZombie).UnsetAttackedPlayer();
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(stateManager.ChasingState);
    }

    public override void Setup(FSM_NZ_Manager stateManager)
    {
        owner = stateManager.Owner;
    }

    public override string ToString() => "Stunned";
}


public class FSM_Boss_Stun : FSM_Entity_Stunned<FSM_Boss_Manager>
{
    public override void EnterState(FSM_Boss_Manager stateManager)
    {
        base.EnterState(stateManager);

        (owner as BossZombie).UnsetAttackedPlayer();
    }

    public override void Conditions(FSM_Boss_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(stateManager.ChasingState);
    }

    public override void Setup(FSM_Boss_Manager stateManager)
    {
        owner = stateManager.Owner;
    }

    public override string ToString() => "Stunned";
}

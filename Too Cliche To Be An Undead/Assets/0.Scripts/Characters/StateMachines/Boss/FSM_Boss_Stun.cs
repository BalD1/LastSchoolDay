
public class FSM_Boss_Stun : FSM_Entity_Stunned<FSM_Boss_Manager, FSM_Boss_Manager.E_BossState>
{
    public override void EnterState(FSM_Boss_Manager stateManager)
    {
        base.EnterState(stateManager);

        //(owner as BossZombie).UnsetAttackedPlayer();
    }

    public override void Conditions(FSM_Boss_Manager stateManager)
    {
        base.Conditions(stateManager);
        if (baseConditionChecked) stateManager.SwitchState(FSM_Boss_Manager.E_BossState.Chasing);
        //if (owner.CurrentHP <= 0)
            stateManager.SwitchState(FSM_Boss_Manager.E_BossState.Dead);
    }

    public override void Setup(FSM_Boss_Manager stateManager)
    {
        owner = stateManager.Owner;
    }

    public override string ToString() => "Stunned";
}


public static class FSM_Player_Extensions
{
    public static void CheckDying(this FSM_Base<FSM_Player_Manager> state, FSM_Player_Manager manager)
    {
        if (manager.Owner.CurrentHP <= 0) manager.SwitchState(FSM_Player_Manager.E_PlayerState.Dying);
    }
}

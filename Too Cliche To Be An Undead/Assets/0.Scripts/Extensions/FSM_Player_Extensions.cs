
public static class FSM_Player_Extensions
{
    public static void CheckDying(this FSM_Base<FSM_Player, FSM_Player.E_PlayerState> state, FSM_Player manager)
    {
        //if (manager.Owner.CurrentHP <= 0) manager.SwitchState(FSM_Player_Manager.E_PlayerState.Dying);
    }
}

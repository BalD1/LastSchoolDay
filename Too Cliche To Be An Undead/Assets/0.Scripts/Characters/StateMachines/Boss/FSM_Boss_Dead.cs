using Spine.Unity;

public class FSM_Boss_Dead : FSM_Base<FSM_Boss_Manager, FSM_Boss_Manager.E_BossState>
{
    private BossZombie owner;

    public override void EnterState(FSM_Boss_Manager stateManager)
    {
        base.EnterState(stateManager);

        GameManager.Instance.D_bossFightEnded?.Invoke();

        foreach (var item in owner.SpawnedZombies)
        {
            (item as NormalZombie).ForceKill();
        }
        owner.SpawnedZombies.Clear();

        foreach (var item in IGPlayersManager.Instance.PlayersList)
        {
            if (item.StateManager.CurrentState.StateKey == FSM_Player_Manager.E_PlayerState.Dead)
                item.AskRevive();
        }

        SoundManager.Instance.ChangeMusicMixerPitch(1);
    }

    public override void UpdateState(FSM_Boss_Manager stateManager)
    {
    }

    public override void FixedUpdateState(FSM_Boss_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Boss_Manager stateManager)
    {
        base.ExitState(stateManager);
    }

    public override void Conditions(FSM_Boss_Manager stateManager)
    {
    }

    protected override void EventsSubscriber(FSM_Boss_Manager stateManager)
    {
    }

    protected override void EventsUnsubscriber(FSM_Boss_Manager stateManager)
    {
    }

    public override void Setup(FSM_Boss_Manager stateManager)
    {
        owner = stateManager.Owner;
    }

    public override string ToString()
    {
        return "Dead";
    }
}

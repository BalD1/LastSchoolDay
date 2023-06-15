using Spine.Unity;

public class FSM_Boss_Dead : FSM_Base<FSM_Boss_Manager>
{
    BossZombie owner;

    private bool wasJumping = false;

    public override void EnterState(FSM_Boss_Manager stateManager)
    {
        base.EnterState(stateManager);

        GameManager.Instance.D_bossFightEnded?.Invoke();

        foreach (var item in owner.SpawnedZombies)
        {
            (item as NormalZombie).ForceKill();
        }
        owner.SpawnedZombies.Clear();

        foreach (var item in GameManager.Instance.playersByName)
        {
            if (item.playerScript.StateManager.ToString() == "Dying")
                item.playerScript.AskRevive();
        }
        if (GameManager.Instance.GameState == GameManager.E_GameState.GameOver)
            GameManager.Instance.CancelGameOver();

        SoundManager.Instance.ChangeMusicMixerPitch(1);
        UIManager.Instance.RemoveBossCollider(owner.hudTrigger);
    }

    private void SetAnim() => owner.animationController.SetAnimation(owner.animationData.DeathAnim, false);

    public override void UpdateState(FSM_Boss_Manager stateManager)
    {
    }

    public override void FixedUpdateState(FSM_Boss_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Boss_Manager stateManager)
    {
        base.ExitState(stateManager);
        wasJumping = false;
    }

    public override void Conditions(FSM_Boss_Manager stateManager)
    {
    }

    protected override void EventsSubscriber(FSM_Boss_Manager stateManager)
    {
        if (owner.IsJumping)
        {
            owner.OnJumpEnded += SetAnim;
            wasJumping = true;
        }
    }

    protected override void EventsUnsubscriber(FSM_Boss_Manager stateManager)
    {
        if (wasJumping) owner.OnJumpEnded -= SetAnim;
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

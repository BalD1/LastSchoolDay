using UnityEngine;

public class FSM_NZ_Idle : FSM_Entity_Idle<FSM_NZ_Manager>
{ 
    private bool canSwitchToChase;
    private float waitBeforeNewPoint;

    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        base.EnterState(stateManager);

        waitBeforeNewPoint = Random.Range((owner as NormalZombie).MinWaitBeforeNewWanderPoint, (owner as NormalZombie).MaxWaitBeforeNewWanderPoint);

        owner.canBePushed = true;
    }

    public override void UpdateState(FSM_NZ_Manager stateManager)
    {
        if (waitBeforeNewPoint > 0) waitBeforeNewPoint -= Time.deltaTime;
        base.UpdateState(stateManager);
    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
        base.ExitState(stateManager);
        canSwitchToChase = false;
        waitBeforeNewPoint = 0;
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        if (canSwitchToChase) stateManager.SwitchState(stateManager.ChasingState);

        if (waitBeforeNewPoint <= 0) stateManager.SwitchState(stateManager.WanderingState);

        base.Conditions(stateManager);
    }

    public override void Setup(FSM_NZ_Manager stateManager)
    {
        owner = stateManager.Owner;
    }

    public void SawPlayer() => canSwitchToChase = true;
}

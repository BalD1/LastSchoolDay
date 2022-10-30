using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FSM_NZ_Idle : FSM_Entity_Idle<FSM_NZ_Manager>
{ 
    private bool canSwitchToChase;
    private float waitBeforeNewPoint;

    public override void EnterState(FSM_NZ_Manager stateManager)
    {
        owner ??= stateManager.Owner;
        base.EnterState(stateManager);

        waitBeforeNewPoint = Random.Range((owner as NormalZombie).MinWaitBeforeNewWanderPoint, (owner as NormalZombie).MaxWaitBeforeNewWanderPoint);
    }

    public override void UpdateState(FSM_NZ_Manager stateManager)
    {
        if (waitBeforeNewPoint > 0) waitBeforeNewPoint -= Time.deltaTime;
        base.UpdateState(stateManager);
    }

    public override void ExitState(FSM_NZ_Manager stateManager)
    {
        canSwitchToChase = false;
        waitBeforeNewPoint = 0;

        base.ExitState(stateManager);
    }

    public override void Conditions(FSM_NZ_Manager stateManager)
    {
        if (canSwitchToChase) stateManager.SwitchState(stateManager.chasingState);

        if (waitBeforeNewPoint <= 0) stateManager.SwitchState(stateManager.wanderingState);

        base.Conditions(stateManager);
    }

    public void SawPlayer() => canSwitchToChase = true;
}

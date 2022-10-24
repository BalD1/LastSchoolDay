using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_TD_Idle : FSM_Entity_Idle<FSM_TD_Manager>
{
    public override void EnterState(FSM_TD_Manager stateManager)
    {
        owner ??= stateManager.Owner;
        base.EnterState(stateManager);
    }
}

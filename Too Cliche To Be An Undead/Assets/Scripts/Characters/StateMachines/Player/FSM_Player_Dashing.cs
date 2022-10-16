using BalDUtilities.MouseUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Dashing : FSM_Base<FSM_Player_Manager>
{
    private PlayerCharacter owner;
    private float dash_dur_TIMER;

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        owner ??= stateManager.Owner;
        dash_dur_TIMER = owner.Dash_DURATION;

        owner.GetRb.velocity = Vector2.zero;
        owner.SetVelocity(Vector2.zero);

        Vector2 mousePos = MousePosition.GetMouseWorldPosition();
        Vector2 mouseDir = (mousePos - (Vector2)owner.transform.position).normalized;

        owner.SetVelocity(mouseDir * owner.DashForce);
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        dash_dur_TIMER -= Time.deltaTime;
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
        owner.GetRb.MovePosition(owner.GetRb.position + owner.Velocity * Time.fixedDeltaTime);
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        owner.isDashing = false;
        owner.GetRb.velocity = Vector2.zero;
        owner.SetVelocity(Vector2.zero);
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        if (dash_dur_TIMER <= 0) stateManager.SwitchState(stateManager.idleState);
    }
}

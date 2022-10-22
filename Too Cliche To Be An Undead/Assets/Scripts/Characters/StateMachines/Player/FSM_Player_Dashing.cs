using BalDUtilities.MouseUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Dashing : FSM_Base<FSM_Player_Manager>
{
    private PlayerCharacter owner;
    private float max_DURATION;
    private float dash_dur_TIMER;

    private Vector2 mouseDir;

    private List<Collider2D> alreadyPushedEntities;

    private Vector2 b;

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        owner ??= stateManager.Owner;
        b = owner.transform.position;
        owner.d_EnteredTrigger += TriggerEnter;
        max_DURATION = owner.PlayerDash.DashSpeedCurve[owner.PlayerDash.DashSpeedCurve.length - 1].time;
        dash_dur_TIMER = max_DURATION;

        alreadyPushedEntities = new List<Collider2D>();

        owner.SetAllVelocity(Vector2.zero);

        Vector2 mousePos = MousePosition.GetMouseWorldPosition();
        mouseDir = (mousePos - (Vector2)owner.transform.position).normalized;

        owner.PlayerDash.OnDashStart(owner);

        owner.SetSelfVelocity(mouseDir * owner.PlayerDash.DashSpeedCurve.Evaluate(0));

        Vector2 animatorMouseDir = stateManager.Owner.Weapon.GetDirectionOfMouse();
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_HORIZONTAL, animatorMouseDir.x);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_VERTICAL, animatorMouseDir.y);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_DASHING, true);
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        dash_dur_TIMER -= Time.deltaTime;
        owner.SetSelfVelocity(mouseDir * owner.PlayerDash.DashSpeedCurve.Evaluate(-(dash_dur_TIMER - max_DURATION)));
        owner.PlayerDash.OnDashUpdate(owner);
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
        owner.GetRb.MovePosition(owner.GetRb.position + owner.Velocity * Time.fixedDeltaTime);
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        owner.isDashing = false;
        owner.SetAllVelocity(Vector2.zero);
        owner.PlayerDash.OnDashStop(owner);
        owner.d_EnteredTrigger -= TriggerEnter;
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_DASHING, false);
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        if (dash_dur_TIMER <= 0) stateManager.SwitchState(stateManager.idleState);
    }

    private void TriggerEnter(Collider2D collider)
    {
        // Check if the hit object is an entity
        Entity e =  collider.GetComponentInParent<Entity>();
        if (e == null) return;

        // Check if the entity as not already been pushed
        if (alreadyPushedEntities.Contains(collider)) return;

        alreadyPushedEntities.Add(collider);

        // lessen the PushForce depending on the remaining push time
        float remainingPushForce = owner.PlayerDash.PushForce * GetRemainingTimeByMax();

        e.Push(owner.transform.position, remainingPushForce);
    }

    public float GetRemainingTimeByMax() => dash_dur_TIMER / max_DURATION;
}

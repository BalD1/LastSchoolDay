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

    private const float remainingForceOnCollision = .9f;
    private const float remainingTimeReductionOnCollision = .95f;
    private const float remainingForceThresholdToIgnoreIfPlayer = 10;

    private float hitStopTimeBase = .10f;
    private float hitStopTimer;

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        owner ??= stateManager.Owner;
        owner.canBePushed = false;
        owner.OnEnteredBodyTrigger += TriggerEnter;
        max_DURATION = owner.PlayerDash.DashSpeedCurve[owner.PlayerDash.DashSpeedCurve.length - 1].time;
        dash_dur_TIMER = max_DURATION;

        owner.OnDashStarted?.Invoke();

        alreadyPushedEntities = new List<Collider2D>();

        owner.SetAllVelocity(Vector2.zero);

        if (owner.Inputs.currentControlScheme.Equals(PlayerCharacter.SCHEME_KEYBOARD) && GameManager.OPTION_DashToMouse)
        {
            Vector2 mousePos = MousePosition.GetMouseWorldPosition();
            mouseDir = (mousePos - (Vector2)owner.transform.position).normalized;
        }
        else mouseDir = (owner.LastDirection).normalized;

        owner.PlayerDash.OnDashStart(owner, mouseDir);

        owner.SetSelfVelocity(mouseDir * owner.PlayerDash.DashSpeedCurve.Evaluate(0));

        Vector2 animatorMouseDir = stateManager.Owner.Weapon.GetGeneralDirectionOfMouseOrGamepad();
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_HORIZONTAL, animatorMouseDir.x);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_VERTICAL, animatorMouseDir.y);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_DASHING, true);

        owner.PlayerHUD.UpdateDashThumbnailFill(1);
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        if (hitStopTimer > 0)
        {
            hitStopTimer -= Time.deltaTime;
            if (hitStopTimer <= 0)
                owner.SkeletonAnimation.AnimationState.TimeScale = 1;

            return;
        }
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
        owner.ForceUpdateMovementsInput();

        owner.AnimationController.FlipSkeleton(mouseDir.x > 0);

        owner.PlayerDash.OnDashStop(owner);

        owner.OnEnteredBodyTrigger -= TriggerEnter;

        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_DASHING, false);

        owner.StartDashTimer();
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        if (dash_dur_TIMER <= 0) stateManager.SwitchState(stateManager.movingState);
    }

    private void TriggerEnter(Collider2D collider)
    {
        // Check if the hit object is an entity
        Entity e =  collider.GetComponentInParent<Entity>();
        if (e == null) return;

        // Check if the entity as not already been pushed
        if (alreadyPushedEntities.Contains(collider)) return;

        alreadyPushedEntities.Add(collider);

        owner.GetRb.velocity *= remainingForceOnCollision;
        dash_dur_TIMER *= remainingTimeReductionOnCollision;

        // lessen the PushForce depending on the remaining push time
        float remainingPushForce = owner.PlayerDash.PushForce * GetRemainingTimeByMax();

        CameraManager.Instance.ShakeCamera(owner.PlayerDash.MaxScreenShakeIntensity * GetRemainingTimeByMax(),
                                           owner.PlayerDash.MaxScreenShakeDuration * GetRemainingTimeByMax());

        if (e is PlayerCharacter)
        {
            if (remainingPushForce <= remainingForceThresholdToIgnoreIfPlayer) return;

            remainingPushForce *= 2;
        }

        hitStopTimer = hitStopTimeBase;
        owner.SetSelfVelocity(Vector2.zero);
        owner.StartTimeStop();
        LeanTween.delayedCall(hitStopTimeBase, () => owner.StopTimeStop());

        e.Push(owner.transform.position, remainingPushForce, owner, owner);
        owner.OnDashHit(e);
    }

    public float GetRemainingTimeByMax() => dash_dur_TIMER / max_DURATION;
    public override string ToString() => "Dashing";
}

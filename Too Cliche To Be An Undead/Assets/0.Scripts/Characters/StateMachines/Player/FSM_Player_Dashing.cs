using BalDUtilities.MouseUtils;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Dashing : FSM_Base<FSM_Player, FSM_Player.E_PlayerState>
{
    private PlayerCharacter owner;
    private float max_DURATION;
    private float dash_dur_TIMER;

    public FSM_Player.E_PlayerState StateName { get; private set; }

    private Vector2 mouseDir;

    private List<Collider2D> alreadyPushedEntities;

    private const float remainingForceOnCollision = .9f;
    private const float remainingTimeReductionOnCollision = .95f;
    private const float remainingForceThresholdToIgnoreIfPlayer = 10;

    private float hitStopTimeBase = .10f;
    private float hitStopTimer;

    public override void EnterState(FSM_Player stateManager)
    {
        base.EnterState(stateManager);
        ////owner.canBePushed = false;
        //max_DURATION = owner.PlayerDash.DashSpeedCurve[owner.PlayerDash.DashSpeedCurve.length - 1].time;
        //dash_dur_TIMER = max_DURATION;

        //(owner.BodyTrigger as BoxCollider2D).size *= 2;

        //owner.OnDashStarted?.Invoke();

        //alreadyPushedEntities = new List<Collider2D>();

        //owner.PlayerMotor?.SetAllVelocity(Vector2.zero);

        //if (owner.PlayerInputsComponent.IsOnKeyboard() && GameManager.OPTION_DashToMouse)
        //{
        //    Vector2 mousePos = MousePosition.GetMouseWorldPosition();
        //    mouseDir = (mousePos - (Vector2)owner.transform.position).normalized;
        //}
        //else mouseDir = (owner.PlayerMotor.LastDirection).normalized;

        //owner.PlayerDash.OnDashStart(owner, mouseDir);

        //owner.PlayerMotor?.SetSelfVelocity(mouseDir * owner.PlayerDash.DashSpeedCurve.Evaluate(0));

        //owner.PlayerHUD.UpdateDashThumbnailFill(1);
    }

    public override void UpdateState(FSM_Player stateManager)
    {
        if (hitStopTimer > 0)
        {
            hitStopTimer -= Time.deltaTime;
            //if (hitStopTimer <= 0)
                //owner.SkeletonAnimation.AnimationState.TimeScale = 1;

            return;
        }
        dash_dur_TIMER -= Time.deltaTime;
        //owner.PlayerMotor?.SetSelfVelocity(mouseDir * owner.PlayerDash.DashSpeedCurve.Evaluate(-(dash_dur_TIMER - max_DURATION)));
        //owner.PlayerDash.OnDashUpdate(owner);
    }

    public override void FixedUpdateState(FSM_Player stateManager)
    {
        //owner.GetRb.MovePosition(owner.GetRb.position + owner.PlayerMotor.Velocity * Time.fixedDeltaTime);
    }

    public override void ExitState(FSM_Player stateManager)
    {
        base.ExitState(stateManager);

        //owner.PlayerMotor?.SetAllVelocity(Vector2.zero);

        //owner.AnimationController.TryFlipSkeleton(mouseDir.x > 0);

        //(owner.BodyTrigger as BoxCollider2D).size /= 2;

        //owner.PlayerDash.OnDashStop(owner);

        //owner.StartDashTimer();
        //owner.PlayerInputsComponent.ForceReadMovements();
    }

    public override void Conditions(FSM_Player stateManager)
    {
        if (dash_dur_TIMER <= 0) stateManager.SwitchState(FSM_Player.E_PlayerState.Moving);
        this.CheckDying(stateManager);
    }

    protected override void EventsSubscriber(FSM_Player stateManager)
    {
        //owner.OnEnteredBodyTrigger += TriggerEnter;
        owner.OnAskForStun += stateManager.SwitchToStun;
        CinematicManagerEvents.OnChangeCinematicState += stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue += stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue += stateManager.DialogueEnded;
    }

    protected override void EventsUnsubscriber(FSM_Player stateManager)
    {
        //owner.OnEnteredBodyTrigger -= TriggerEnter;
        owner.OnAskForStun -= stateManager.SwitchToStun;
        CinematicManagerEvents.OnChangeCinematicState -= stateManager.CinematicStateChange;
        DialogueManagerEvents.OnStartDialogue -= stateManager.DialogueStart;
        DialogueManagerEvents.OnEndDialogue -= stateManager.DialogueEnded;
    }

    private void TriggerEnter(Collider2D collider)
    {
        // Check if the hit object is an entity
        Entity e =  collider.GetComponentInParent<Entity>();
        if (e == null) return;

        // Check if the entity as not already been pushed
        if (alreadyPushedEntities.Contains(collider)) return;

        alreadyPushedEntities.Add(collider);

        //owner.GetRb.velocity *= remainingForceOnCollision;
        dash_dur_TIMER *= remainingTimeReductionOnCollision;

        // lessen the PushForce depending on the remaining push time
        //float remainingPushForce = owner.PlayerDash.PushForce * GetRemainingTimeByMax();

        //CameraManager.Instance.ShakeCamera(owner.PlayerDash.MaxScreenShakeIntensity * GetRemainingTimeByMax(),
                                           //owner.PlayerDash.MaxScreenShakeDuration * GetRemainingTimeByMax());

        if (e is PlayerCharacter)
        {
            //if (remainingPushForce <= remainingForceThresholdToIgnoreIfPlayer) return;

            //remainingPushForce *= 2;
        }

        hitStopTimer = hitStopTimeBase;
        //owner.PlayerMotor?.SetSelfVelocity(Vector2.zero);
        //owner.StartTimeStop();
        //LeanTween.delayedCall(hitStopTimeBase, () => owner.StopTimeStop());

        //e.AskPush(remainingPushForce, owner, owner);
        //owner.OnDashHit(e);
    }

    public float GetRemainingTimeByMax() => dash_dur_TIMER / max_DURATION;

    public override void Setup(FSM_Player stateManager)
    {
        owner = stateManager.Owner;
        StateName = FSM_Player.E_PlayerState.Dashing;
    }

    public override string ToString() => StateName.ToString();
}

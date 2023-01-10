using BalDUtilities.MouseUtils;
using Spine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FSM_Player_InSkill : FSM_Base<FSM_Player_Manager>
{
    private PlayerCharacter owner;

    private PlayerAnimationController ownerAnimationController;
    private TrackEntry ownerTrackEntry;

    private Spine.Animation idleAnim;
    private Spine.Animation walkAnim;

    private float skill_MaxTimer;
    private float skill_Timer;

    private float transition_Timer;

    private const float cooldownForCancel = 1.5f;
    private float timerForCancel;

    private bool started;

    private bool isIdle = false;
    private bool isPlayingIdle = false;

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        owner ??= stateManager.Owner;
        ownerAnimationController ??= owner.AnimationController;
        ownerTrackEntry ??= ownerAnimationController.SkeletonAnimation.AnimationState.GetCurrent(0);

        idleAnim = ownerAnimationController.animationsData.skillIdleAnim;
        walkAnim = ownerAnimationController.animationsData.skillWalkAnim;

        started = false;

        timerForCancel = cooldownForCancel;

        Vector2 mouseDir = stateManager.Owner.Weapon.GetGeneralDirectionOfMouseOrGamepad();

        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_HORIZONTAL, mouseDir.x);
        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_VERTICAL, mouseDir.y);

        owner.D_skillInput += StopSkill;

        owner.canBePushed = true;
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        if (started == false)
        {
            if (transition_Timer > 0)
            {
                transition_Timer -= Time.deltaTime;
                return;
            }
            else
            {
                started = true;

                owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_INSKILL, true);

                if (owner.Velocity == Vector2.zero)
                {
                    ownerAnimationController.SetAnimation(idleAnim, true);
                    isPlayingIdle = isIdle = true;
                }
                else
                {
                    ownerAnimationController.SetAnimation(walkAnim, true);
                    isPlayingIdle = isIdle = false;
                }

                ownerAnimationController.SetAnimation(idleAnim, true);

                owner.GetSkillHolder.Trigger.enabled = true;
                owner.GetSkill.StartSkill(owner);
            }
        }

        skill_Timer -= Time.deltaTime;

        if (timerForCancel > 0) timerForCancel -= Time.deltaTime;

        stateManager.OwnerWeapon.SetRotation(owner.GetSkill.AimAtMovements);

        owner.SkillDurationIcon.fillAmount = skill_Timer / skill_MaxTimer;

        Vector2 mousePos = MousePosition.GetMouseWorldPosition();
        Vector2 mouseDir = (mousePos - (Vector2)owner.transform.position).normalized;
        ownerAnimationController.FlipSkeleton(mouseDir.x > 0);

        CheckAnimation();

        owner.GetSkill.UpdateSkill(owner);
    }

    private void CheckAnimation()
    {
        isIdle = owner.Velocity == Vector2.zero;

        if (isIdle && !isPlayingIdle)
        {
            ownerAnimationController.SetAnimation(idleAnim, true);
            isPlayingIdle = true;
        }
        else if (!isIdle && isPlayingIdle)
        {
            ownerAnimationController.SetAnimation(walkAnim, true);
            isPlayingIdle = false;
        }
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
        if (owner.GetSkill.CanMove && started) owner.Movements();
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        owner.GetSkill.StopSkill(owner);
        owner.GetSkillHolder.Trigger.enabled = false;

        owner.ForceUpdateMovementsInput();

        owner.D_skillInput -= StopSkill;

        owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_INSKILL, false);
    }

    private void StopSkill()
    {
        if (timerForCancel > 0) return;

        this.skill_Timer = 0;
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        if (skill_Timer <= 0) stateManager.SwitchState(stateManager.idleState);
    }

    public FSM_Player_InSkill SetTimers(float _skillTimer, float _transitionTimer = -1)
    {
        skill_Timer = _skillTimer;
        skill_MaxTimer = _skillTimer;

        transition_Timer = _transitionTimer;
        if (transition_Timer <= 0)
        {
            started = true;
            owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_INSKILL, true);
            owner.GetSkillHolder.Trigger.enabled = true;
            owner.GetSkill.StartSkill(owner);
        }
        return this;
    }
    public override string ToString() => "InSkill";
}

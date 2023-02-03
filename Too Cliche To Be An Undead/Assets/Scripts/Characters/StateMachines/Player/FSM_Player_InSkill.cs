using BalDUtilities.MouseUtils;
using Spine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FSM_Player_InSkill : FSM_Base<FSM_Player_Manager>
{
    private PlayerCharacter owner;

    private PlayerAnimationController ownerAnimationController;

    private Spine.Animation idleAnim;
    private Spine.Animation walkAnim;

    private float skill_MaxTimer;
    private float skill_Timer;

    private float skill_Start_Offset;
    private float skill_Start_Timer;

    private float transition_Timer;

    private const float cooldownForCancel = 1.5f;
    private float timerForCancel;

    private bool started;
    private bool startOffsetFlag = false;

    private bool isIdle = false;
    private bool isPlayingIdle = false;
    private bool loopAnims = true;

    private Vector2 initialDirection;

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        owner ??= stateManager.Owner;
        ownerAnimationController ??= owner.AnimationController;

        idleAnim = ownerAnimationController.animationsData.skillIdleAnimSide;
        walkAnim = ownerAnimationController.animationsData.skillWalkAnimSide;

        loopAnims = owner.GetSkill.LoopAnims;

        started = false;

        timerForCancel = cooldownForCancel;

        Vector2 mouseDir = stateManager.Owner.Weapon.GetGeneralDirectionOfMouseOrGamepad();
        initialDirection = mouseDir;

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

                if (owner.GetSkill.is4D)
                {
                    switch (initialDirection)
                    {
                        case Vector2 v when v == Vector2.up:
                            ownerAnimationController.SetAnimation(ownerAnimationController.animationsData.skillIdleAnimUp, loopAnims);
                            break;

                        case Vector2 v when v == Vector2.down:
                            ownerAnimationController.SetAnimation(ownerAnimationController.animationsData.skillIdleAnimDown, loopAnims);
                            break;

                        case Vector2 v when v == Vector2.left:
                            ownerAnimationController.SetAnimation(ownerAnimationController.animationsData.skillIdleAnimSide, loopAnims);
                            break;

                        case Vector2 v when v == Vector2.right:
                            ownerAnimationController.SetAnimation(ownerAnimationController.animationsData.skillIdleAnimSide, loopAnims);
                            break;
                    }
                }
                else
                {
                    if (owner.Velocity == Vector2.zero)
                    {
                        ownerAnimationController.SetAnimation(idleAnim, loopAnims);
                        isPlayingIdle = isIdle = true;
                    }
                    else
                    {
                        ownerAnimationController.SetAnimation(walkAnim, loopAnims);
                        isPlayingIdle = isIdle = false;
                    }
                }

                if (skill_Start_Offset <= 0)
                {
                    owner.GetSkillHolder.Trigger.enabled = true;
                    owner.GetSkill.StartSkill(owner);
                }
            }
        }
        else
        {
            if (skill_Start_Offset > 0 && !startOffsetFlag)
            {
                if (skill_Start_Timer >= 0) skill_Start_Timer -= Time.deltaTime;
                else
                {
                    startOffsetFlag = true;
                    owner.GetSkillHolder.Trigger.enabled = true;
                    owner.GetSkill.StartSkill(owner);
                }
            }
        }

        skill_Timer -= Time.deltaTime;

        if (timerForCancel > 0) timerForCancel -= Time.deltaTime;

        stateManager.OwnerWeapon.SetRotation(owner.GetSkill.AimAtMovements);

        owner.SkillDurationIcon.fillAmount = skill_Timer / skill_MaxTimer;

        if (owner.GetSkill.canAim)
        {
            Vector2 mousePos = MousePosition.GetMouseWorldPosition();
            Vector2 mouseDir = (mousePos - (Vector2)owner.transform.position).normalized;
            ownerAnimationController.FlipSkeleton(mouseDir.x > 0);
        }

        CheckAnimation();

        owner.GetSkill.UpdateSkill(owner);
    }

    private void CheckAnimation()
    {
        isIdle = owner.Velocity == Vector2.zero;

        if (isIdle && !isPlayingIdle)
        {
            ownerAnimationController.SetAnimation(idleAnim, loopAnims);
            isPlayingIdle = true;
        }
        else if (!isIdle && isPlayingIdle)
        {
            ownerAnimationController.SetAnimation(walkAnim, loopAnims);
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
        UIManager.Instance.SetSkillIconState(owner.PlayerIndex, false);
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

    public FSM_Player_InSkill SetTimers(float _skillTimer, float _transitionTimer = -1, float _startOffset = -1)
    {
        skill_Timer = _skillTimer + _startOffset;
        skill_MaxTimer = _skillTimer;

        transition_Timer = _transitionTimer;
        skill_Start_Offset = skill_Start_Timer = _startOffset;
        startOffsetFlag = skill_Start_Offset <= 0;

        //if (transition_Timer <= 0)
        //{
        //    started = true;
        //    owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_INSKILL, true);
        //    owner.GetSkillHolder.Trigger.enabled = true;
        //    owner.GetSkill.StartSkill(owner);
        //}
        return this;
    }
    public override string ToString() => "InSkill";
}

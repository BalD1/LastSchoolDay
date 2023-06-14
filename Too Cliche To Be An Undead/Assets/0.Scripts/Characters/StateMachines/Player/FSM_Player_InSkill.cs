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

    public Vector2 SkillHolderPosAtStart { get; private set; }

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        base.EnterState(stateManager);

        SkillHolderPosAtStart = owner.GetSkillHolder.transform.position;

        idleAnim = ownerAnimationController.animationsData.skillIdleAnimSide;
        walkAnim = ownerAnimationController.animationsData.skillWalkAnimSide;

        loopAnims = owner.GetSkill.LoopAnims;

        started = false;

        timerForCancel = cooldownForCancel;

        Vector2 mouseDir = stateManager.Owner.Weapon.GetGeneralDirectionOfMouseOrGamepad();
        initialDirection = mouseDir;

        owner.canBePushed = true;

        owner.GetSkill.EarlyStart(owner);
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
        // if the skill hasn't started yet
        if (started == false)
        {
            // if we must wait for the transition
            if (transition_Timer > 0)
            {
                transition_Timer -= Time.deltaTime;
                return;
            }
            else
            {
                // start the skill
                started = true;

                owner.SetAnimatorArgs(PlayerCharacter.ANIMATOR_ARGS_INSKILL, true);

                float animationSpeedScale = owner.GetSkill.AnimationSpeedScale;

                // if the skill got 4D anims, set the anim depending on orientation
                if (owner.GetSkill.is4D)
                {
                    switch (initialDirection)
                    {
                        case Vector2 v when v == Vector2.up:
                            ownerAnimationController.SetAnimation(ownerAnimationController.animationsData.skillIdleAnimUp, loopAnims, animationSpeedScale);
                            break;

                        case Vector2 v when v == Vector2.down:
                            ownerAnimationController.SetAnimation(ownerAnimationController.animationsData.skillIdleAnimDown, loopAnims, animationSpeedScale);
                            break;

                        case Vector2 v when v == Vector2.left:
                            ownerAnimationController.SetAnimation(ownerAnimationController.animationsData.skillIdleAnimSide, loopAnims, animationSpeedScale);
                            break;

                        case Vector2 v when v == Vector2.right:
                            ownerAnimationController.SetAnimation(ownerAnimationController.animationsData.skillIdleAnimSide, loopAnims, animationSpeedScale);
                            break;
                    }
                }
                else
                {
                    // else, just check if player is idle or not
                    if (owner.Velocity == Vector2.zero)
                    {
                        ownerAnimationController.SetAnimation(idleAnim, loopAnims, animationSpeedScale);
                        isPlayingIdle = isIdle = true;
                    }
                    else
                    {
                        ownerAnimationController.SetAnimation(walkAnim, loopAnims, animationSpeedScale);
                        isPlayingIdle = isIdle = false;
                    }
                }

                // if there is no offset timer for the skill, call it
                if (skill_Start_Offset <= 0)
                {
                    owner.GetSkillHolder.Trigger.enabled = true;
                    owner.GetSkill.StartSkill(owner);
                }
            }
        }
        else
        {
            // if the skill has started, but there is an offset still
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
            owner.AnimationController.FlipSkeletonOnMouseOrGamepad();

        CheckAnimation();

        owner.GetSkill.UpdateSkill(owner);
    }

    private void CheckAnimation()
    {
        isIdle = owner.Velocity == Vector2.zero;

        float animationSpeedScale = owner.GetSkill.AnimationSpeedScale;

        if (isIdle && !isPlayingIdle)
        {
            ownerAnimationController.SetAnimation(idleAnim, loopAnims, animationSpeedScale);
            isPlayingIdle = true;
        }
        else if (!isIdle && isPlayingIdle)
        {
            ownerAnimationController.SetAnimation(walkAnim, loopAnims, animationSpeedScale);
            isPlayingIdle = false;
        }
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
        if (owner.GetSkill.CanMove && started) owner.Movements();
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        base.ExitState(stateManager);
        owner.GetSkill.StopSkill(owner);

        owner.SkillDurationIcon.fillAmount = 0;

        owner.GetSkillHolder.Trigger.enabled = false;

        owner.ForceUpdateMovementsInput();

        owner.OnEndSkill?.Invoke(owner.GetSkill.holdSkillAudio);
    }

    private void StopSkill()
    {
        if (timerForCancel > 0) return;

        this.skill_Timer = 0;

        owner.OnEndSkill?.Invoke(owner.GetSkill.holdSkillAudio);
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
        if (skill_Timer <= 0) stateManager.SwitchState(stateManager.IdleState);
    }

    protected override void EventsSubscriber()
    {
        owner.OnSkillInput += StopSkill;
    }

    protected override void EventsUnsubscriber()
    {
        owner.OnSkillInput -= StopSkill;
    }

    public FSM_Player_InSkill SetTimers(float _skillTimer, float _transitionTimer = -1, float _startOffset = -1)
    {
        skill_Timer = _skillTimer + _startOffset;
        skill_MaxTimer = _skillTimer;

        transition_Timer = _transitionTimer;
        skill_Start_Offset = skill_Start_Timer = _startOffset;
        startOffsetFlag = skill_Start_Offset <= 0;

        return this;
    }

    public override void Setup(FSM_Player_Manager stateManager)
    {
        owner = stateManager.Owner;
        ownerAnimationController = owner.AnimationController;
    }

    public override string ToString() => "InSkill";
}

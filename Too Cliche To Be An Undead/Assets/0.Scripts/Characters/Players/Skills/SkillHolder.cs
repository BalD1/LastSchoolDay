using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHolder : MonoBehaviour
{
    [SerializeField] private PlayerCharacter owner;
    [SerializeField] private SCRPT_Skill skill;
    [SerializeField] private Animator animator;
    [SerializeField] private CircleCollider2D trigger;

    public SCRPT_Skill Skill { get => skill; }
    public Animator GetAnimator { get => animator; }
    public CircleCollider2D Trigger { get => trigger; }

    public delegate void D_EnteredTrigger(Entity entity);
    public delegate void D_ExitedTrigger(Entity entity);

    public D_EnteredTrigger D_enteredTrigger;
    public D_ExitedTrigger D_exitedTrigger;

    private List<Collider2D> collidersInTrigger = new List<Collider2D>();

#if UNITY_EDITOR
    [SerializeField] public bool debugMode;
#endif

    private float timer;

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            float fillAmount = timer / owner.MaxSkillCD_M;
            owner.PlayerHUD.UpdateSkillThumbnailFill(fillAmount);
        }
    }

    public void StartSkill()
    {
        if (Skill.IsInUse || timer > 0) return;

        PlayerAnimationController animController = owner.AnimationController;
        AnimationReferenceAsset transitionAnim = animController.animationsData.skillTransitionAnim;

        float transitionDuration = -1;
        float startOffset = -1;

        startOffset = skill.StartOffset;

        if (transitionAnim != null)
        {
            animController.SetAnimation(transitionAnim, false);
            transitionDuration = transitionAnim.Animation.Duration;
            transitionDuration *= skill.TransitionDurationMultiplier;
        }

        owner.StateManager.SwitchState(owner.StateManager.inSkillState.SetTimers(skill.Duration, transitionDuration, startOffset));

        owner.PlayerHUD.UpdateSkillThumbnailFill(1);
    }

    public void StartTimer() => timer = owner.MaxSkillCD_M;
    public void StartTimer(float t)
    {
        timer = t;
        Debug.Log(timer);
    }

    public void PlayAnimation(string id) => animator.Play(id);

    public void AnimationEnded() => this.transform.localPosition = Vector2.zero;

    public void ChangeSkill(SCRPT_Skill newSkill)
    {
        this.skill = newSkill;
        this.Skill.ResetSkill();
        timer = 0;
        owner.PlayerHUD.SetSkillThumbnail(newSkill.Thumbnail);

        owner.PlayerHUD.UpdateSkillThumbnailFill(0, false);

        owner.ResetSkillAnimator();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        collidersInTrigger.Add(collision);

        Entity e = collision.GetComponentInParent<Entity>();

        if (e != null)
            D_enteredTrigger?.Invoke(e);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collidersInTrigger.Remove(collision);

        Entity e = collision.GetComponentInParent<Entity>();

        if (e != null)
        D_exitedTrigger?.Invoke(e);
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (debugMode)
        {
            Gizmos.DrawWireSphere(this.transform.position, skill.Range);
        }
#endif
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHolder : MonoBehaviour
{
    [SerializeField] private PlayerCharacter owner;
    [SerializeField] private SCRPT_Skill skill;
    [SerializeField] private Animator animator;

    public SCRPT_Skill Skill { get => skill; }
    public Animator GetAnimator { get => animator; }

#if UNITY_EDITOR
    [SerializeField] public bool debugMode;
#endif

    private float timer;

    private void Awake()
    {
        owner.SetSkillThumbnail(skill.Thumbnail);
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            owner.UpdateSkillThumbnailFill(-((timer / skill.Cooldown) - 1));
        }
    }

    public void UseSkill()
    {
        if (timer > 0) return;

        skill.Use(owner);
        owner.StateManager.SwitchState(owner.StateManager.inSkillState.SetTimer(skill.Duration));
        owner.UpdateSkillThumbnailFill(-((timer / skill.Cooldown) - 1));
    }

    public void StartTimer(float t) => timer = t;

    public void PlayAnimation(string id) => animator.Play(id);

    public void AnimationEnded() => this.transform.localPosition = Vector2.zero;

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

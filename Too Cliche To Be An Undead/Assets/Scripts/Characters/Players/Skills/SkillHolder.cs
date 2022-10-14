using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHolder : MonoBehaviour
{
    [SerializeField] private PlayerCharacter owner;
    [SerializeField] private SCRPT_Skill skill;
    [SerializeField] private Animator animator;
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
        if (Input.GetMouseButtonDown(1) && timer <= 0) UseSkill();
        else if (timer > 0)
        {
            timer -= Time.deltaTime;
            owner.UpdateSkillThumbnailFill(-((timer / skill.Cooldown) - 1));
        }
    }

    private void UseSkill()
    {
        skill.Use(owner);
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

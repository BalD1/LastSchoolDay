using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable/Entity/Skills/ProtectionZone")]
public class SCRPT_Skill_ProtectionZone : SCRPT_Skill
{
    public List<PlayerCharacter> playersInRange = new List<PlayerCharacter>();

    public override void StartSkill(PlayerCharacter owner)
    {
        isInUse = true;

        owner.D_startSkill?.Invoke(owner.GetSkill.holdSkillAudio);

        owner.SetInvincibility(true);

        owner.SkillTutoAnimator.SetTrigger(skillTutoAnimatorName);

        owner.GetSkillHolder.GetComponent<SpriteRenderer>().sortingLayerName = layerName.ToString();
        owner.GetSkillHolder.GetAnimator.Play(animationToPlay);
        owner.OffsetSkillHolder(offset);

        owner.OnEnteredBodyTrigger += EnteredTrigger;
        owner.OnExitedBodyTrigger += ExitedTrigger;
    }

    public override void UpdateSkill(PlayerCharacter owner) 
    {

    }

    public override void StopSkill(PlayerCharacter owner)
    {
        isInUse = false;
        owner.SetInvincibility(false);
        owner.SetTimedInvincibility(1.5f);

        owner.GetSkillHolder.GetAnimator.SetTrigger("EndSkill");
        owner.GetSkillHolder.AnimationEnded();
        owner.GetSkillHolder.StartTimer();

        owner.SkillTutoAnimator.SetTrigger("finish");

        owner.OnEnteredBodyTrigger -= EnteredTrigger;
        owner.OnExitedBodyTrigger -= ExitedTrigger;

        if (playersInRange.Count <= 0) return;

        foreach (var item in playersInRange)    
        {
            item.SetInvincibility(false);
            item.SetTimedInvincibility(.5f);
        }

        playersInRange.Clear();
    }

    private void EnteredTrigger(Collider2D collider)
    {
        if (GameManager.IsInLayerMask(collider.gameObject, entitiesToAffect) == false) return;

        PlayerCharacter target = collider.GetComponentInParent<PlayerCharacter>();

        if (target == null) return;

        playersInRange.Add(target);
        target?.SetInvincibility(true);
    }

    private void ExitedTrigger(Collider2D collider)
    {
        if (GameManager.IsInLayerMask(collider.gameObject, entitiesToAffect) == false) return;

        PlayerCharacter target = collider.GetComponentInParent<PlayerCharacter>();

        if (target == null) return;

        playersInRange.Remove(target);
        target?.SetInvincibility(false);
        target?.SetTimedInvincibility(1);
    }

    public override void EarlyStart(PlayerCharacter owner)
    {
        throw new System.NotImplementedException();
    }
}

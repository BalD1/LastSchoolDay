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
        owner.SetInvincibility(true);
        owner.GetSkillHolder.GetComponent<SpriteRenderer>().sortingLayerName = layerName.ToString();
        owner.GetSkillHolder.GetAnimator.Play(animationToPlay);
        owner.OffsetSkillHolder(offset);

        owner.d_EnteredTrigger += EnteredTrigger;
        owner.d_ExitedTrigger += ExitedTrigger;
    }

    public override void UpdateSkill(PlayerCharacter owner) 
    {

    }

    public override void StopSkill(PlayerCharacter owner)
    {
        isInUse = false; isInUse = false;
        owner.SetInvincibility(false);
        owner.GetSkillHolder.GetAnimator.SetTrigger("EndSkill");
        owner.GetSkillHolder.AnimationEnded();
        owner.GetSkillHolder.StartTimer(cooldown);

        owner.d_EnteredTrigger -= EnteredTrigger;
        owner.d_ExitedTrigger -= ExitedTrigger;

        if (playersInRange.Count <= 0) return;

        foreach (var item in playersInRange)    
        {
            item.SetInvincibility(false);
        }

        playersInRange.Clear();
    }

    private void EnteredTrigger(Collider2D collider)
    {
        PlayerCharacter target = collider.GetComponentInParent<PlayerCharacter>();

        if (target == null) return;

        playersInRange.Add(target);
        target?.SetInvincibility(true);
    }

    private void ExitedTrigger(Collider2D collider)
    {
        PlayerCharacter target = collider.GetComponentInParent<PlayerCharacter>();

        if (target == null) return;

        playersInRange.Remove(target);
        target?.SetInvincibility(false);
    }
}

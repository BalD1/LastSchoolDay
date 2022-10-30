using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable/Entity/Skills/Flamethrower")]
public class SCRPT_Skill_Flamethrower : SCRPT_Skill
{
    private const string tickDamages_ID = "SKILL_FT";

    private List<Entity> entitiesInTrigger = new List<Entity>();

    private TickDamages tickDamages;

    public override void StartSkill(PlayerCharacter owner)
    {
        isInUse = true; 
        owner.GetSkillHolder.D_enteredTrigger += EnteredTrigger;
        owner.GetSkillHolder.D_exitedTrigger += ExitedTrigger;

        owner.GetSkillHolder.GetAnimator.Play(animationToPlay);

        isInUse = true;

        finalDamages = damages;
        foreach (var modif in owner.StatsModifiers)
        {
            if (modif.StatType.Equals(StatsModifier.E_StatType.Damages)) finalDamages += modif.Modifier;
        }
    }

    public override void UpdateSkill(PlayerCharacter owner)
    {
        owner.OffsetSkillHolder(offset);
        owner.RotateSkillHolder();

        foreach (var item in entitiesInTrigger)
        {
            item.AddTickDamages(tickDamages_ID, finalDamages, .5f, 2f);
        }
    }

    public override void StopSkill(PlayerCharacter owner)
    {
        owner.GetSkillHolder.GetAnimator.SetTrigger("EndSkill");
        owner.GetSkillHolder.AnimationEnded();
        owner.GetSkillHolder.StartTimer(cooldown);
        isInUse = false;
    }

    public void EnteredTrigger(Entity entity)
    {
        entitiesInTrigger.Add(entity);
    }

    public void ExitedTrigger(Entity entity)
    {
        entitiesInTrigger.Remove(entity);
    }
}

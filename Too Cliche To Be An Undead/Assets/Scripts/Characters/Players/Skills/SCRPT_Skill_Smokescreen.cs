using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable/Entity/Skills/Smokescreen")]
public class SCRPT_Skill_Smokescreen : SCRPT_Skill
{
    [SerializeField] private float stunDuration;

    public override void StartSkill(PlayerCharacter owner)
    {
        isInUse = true;
        owner.OffsetSkillHolder(offset);
        owner.GetSkillHolder.GetComponent<SpriteRenderer>().sortingLayerName = layerName.ToString();

        if (particles != null)
            Instantiate(particles, owner.GetSkillHolder.transform.position, Quaternion.identity);

        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(owner.GetSkillHolder.transform.position, range, entitiesToAffect);

        Entity currentEntity;

        foreach (var item in hitTargets)
        {
            currentEntity = item.GetComponentInParent<Entity>();

            bool isFromSameTeam = currentEntity.GetStats.Team.Equals(owner.GetStats.Team);
            if (isFromSameTeam) continue;

            currentEntity.Stun(stunDuration);
        }
    }

    public override void UpdateSkill(PlayerCharacter owner) { }

    public override void StopSkill(PlayerCharacter owner)
    {
        isInUse = false;
        owner.GetSkillHolder.StartTimer();
    }
}

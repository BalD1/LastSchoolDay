using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable/Entity/Skills/Smokescreen")]
public class SCRPT_Skill_Smokescreen : SCRPT_Skill
{
    [SerializeField] private float stunDuration;


    [SerializeField] private bool addPoison;
    [SerializeField] private SCRPT_PoisonData poisonData;

    private const string poisonID = "SKILL_PS";

    public override void EarlyStart(PlayerCharacter owner)
    {
        //owner.OnEarlySkillStart?.Invoke();
    }

    public override void StartSkill(PlayerCharacter owner)
    {
        isInUse = true;

        //owner.OnSkillStart?.Invoke(owner.GetSkill.holdSkillAudio);

        //owner.OffsetSkillHolder(offset);
        //Vector2 skillPos = owner.GetSkillHolder.transform.position;

        //owner.GetSkillHolder.GetComponent<SpriteRenderer>().sortingLayerName = layerName.ToString();

        //owner.SkillTutoAnimator.SetTrigger(skillTutoAnimatorName);

        //if (particles != null)
        //    Instantiate(particles, skillPos, Quaternion.identity);

        //Collider2D[] hitTargets = Physics2D.OverlapCircleAll(skillPos, range, entitiesToAffect);

        Entity currentEntity;

        //foreach (var item in hitTargets)
        //{
        //    currentEntity = item.GetComponentInParent<Entity>();

        //    //bool isFromSameTeam = currentEntity.GetStats.Team.Equals(owner.GetStats.Team);
        //    //if (isFromSameTeam) continue;

        //    //currentEntity.Stun(stunDuration, false, true);

        //    //if (addPoison == false) continue;

        //    //float finalDamages = owner.MaxDamages_M * poisonData.OwnerDamagesMultiplier;

        //    //currentEntity.AddTickDamages(poisonID, finalDamages, poisonData.PoisonTimeBetweenTicks, poisonData.PoisonDuration, owner, true, poisonData.PoisonCritChances);
        //}
    }

    public override void UpdateSkill(PlayerCharacter owner) { }

    public override void StopSkill(PlayerCharacter owner)
    {
        isInUse = false;
        //owner.GetSkillHolder.StartCooldown();

        //owner.SkillTutoAnimator.SetTrigger("finish");
    }
}

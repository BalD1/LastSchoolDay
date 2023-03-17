using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable/Entity/Skills/Flamethrower")]
public class SCRPT_Skill_Flamethrower : SCRPT_Skill
{
    private const string inTriggerTickDamages_ID = "SKILL_FT";

    [SerializeField] private int skeletonIdx;
    [SerializeField] private string skeletonBoneToFollowName;

    [SerializeField] private Vector2 armsOffset;

    private List<Entity> entitiesInTrigger = new List<Entity>();

    [SerializeField] private float tickDamagesMultiplier = .5f;
    private float tickDamages;

    private GameObject playingParticles;

    public override void StartSkill(PlayerCharacter owner)
    {
        isInUse = true;

        owner.D_startSkill?.Invoke(owner.GetSkill.holdSkillAudio);

        owner.GetSkillHolder.D_enteredTrigger += EnteredTrigger;
        owner.GetSkillHolder.D_exitedTrigger += ExitedTrigger;

        owner.GetSkillHolder.GetComponent<SpriteRenderer>().sortingLayerName = layerName.ToString();
        owner.GetSkillHolder.GetAnimator.Play(animationToPlay);

        owner.OffsetChild(offset);
        owner.SetArmsState(true, armsOffset, skeletonIdx, skeletonBoneToFollowName);

        playingParticles = Instantiate(particles, owner.GetSkillHolder.transform.GetChild(0));

        owner.SkillTutoAnimator.SetTrigger(skillTutoAnimatorName);

        owner.D_aimInput += owner.Weapon.SetAimGoal;

        finalDamages = owner.MaxDamages_M * damagesPercentageModifier;
        tickDamages = finalDamages * tickDamagesMultiplier;

    }

    public override void UpdateSkill(PlayerCharacter owner)
    {
        owner.Weapon.RotateOnAim();

        Quaternion weaponRot = owner.Weapon.transform.rotation;

        Vector3 newRot = weaponRot.eulerAngles;
        newRot.z -= 90f;
        weaponRot.eulerAngles = newRot;

        owner.GetSkillHolder.transform.rotation = weaponRot;

        //owner.RotateSkillHolder();
        owner.RotateArms();
    }

    public override void StopSkill(PlayerCharacter owner)
    {
        owner.GetSkillHolder.GetAnimator.SetTrigger("EndSkill");
        owner.GetSkillHolder.AnimationEnded();
        owner.GetSkillHolder.StartTimer();
        isInUse = false;

        owner.D_aimInput -= owner.Weapon.SetAimGoal;

        owner.GetSkillHolder.D_enteredTrigger -= EnteredTrigger;
        owner.GetSkillHolder.D_exitedTrigger -= ExitedTrigger;

        owner.SetArmsState(false, Vector2.zero);

        owner.SkillTutoAnimator.SetTrigger("finish");

        Destroy(playingParticles);
    }

    public void EnteredTrigger(Entity entity)
    {
        if (GameManager.IsInLayerMask(entity.gameObject, entitiesToAffect) == false) return;

        if (entity as EnemyBase != null)
            entitiesInTrigger.Add(entity);

        TickDamages appliedTickDamages = entity.GetAppliedTickDamages(inTriggerTickDamages_ID);

        if (appliedTickDamages == null)
            entity.AddTickDamages(inTriggerTickDamages_ID, finalDamages, .5f, 3f, true);
        else
        {
            appliedTickDamages.ResetTimer();
            appliedTickDamages.ModifyDamages(finalDamages);
        }
    }

    public void ExitedTrigger(Entity entity)
    {
        if (GameManager.IsInLayerMask(entity.gameObject, entitiesToAffect) == false) return;

        if (entity as EnemyBase != null)
            entitiesInTrigger.Remove(entity);

        TickDamages appliedTickDamages = entity.GetAppliedTickDamages(inTriggerTickDamages_ID);

        if (appliedTickDamages != null)
        {
            appliedTickDamages.ResetTimer();
            appliedTickDamages.ModifyDamages(tickDamages);
        }
    }
}

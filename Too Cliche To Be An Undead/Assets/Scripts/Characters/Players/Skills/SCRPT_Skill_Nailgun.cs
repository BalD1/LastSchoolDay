using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable/Entity/Skills/Nailgun")]
public class SCRPT_Skill_Nailgun : SCRPT_Skill
{
    [SerializeField] private GameObject projectile;

    [SerializeField] private int skeletonIdx;
    [SerializeField] private string skeletonBoneToFollowName;

    [SerializeField] private float critModifier;

    [SerializeField] private float rotationMinRandom, rotationMaxRandom;

    [SerializeField] private Vector2 armsOffset;

    [SerializeField] private float fire_COOLDOWN;
    private float fire_TIMER;

    private int finalCrit;

    private Transform skillHolderTransform;

    private PlayerCharacter _owner;

    public override void StartSkill(PlayerCharacter owner)
    {
        _owner = owner;
        isInUse = true;
        owner.GetSkillHolder.GetComponent<SpriteRenderer>().sortingLayerName = layerName.ToString();
        owner.GetSkillHolder.GetAnimator.Play(animationToPlay);
        owner.OffsetSkillHolder(offset);

        owner.OffsetChild(offset);
        owner.SetArmsState(true, armsOffset, skeletonIdx, skeletonBoneToFollowName);

        owner.SkillTutoAnimator.SetTrigger(skillTutoAnimatorName);

        finalDamages = _owner.maxDamages_M * damagesPercentageModifier;
        finalCrit = (int)(_owner.maxCritChances_M * critModifier);

        fire_TIMER = 0;

        owner.D_aimInput += owner.Weapon.SetAimGoal;

        skillHolderTransform = owner.GetSkillHolder.transform;
    }

    public override void UpdateSkill(PlayerCharacter owner) 
    {
        if (fire_TIMER > 0) fire_TIMER -= Time.deltaTime;

        Fire();

        owner.Weapon.RotateOnAim();
        owner.RotateSkillHolder();
        owner.RotateArms();
    }

    public override void StopSkill(PlayerCharacter owner)
    {
        isInUse = false;

        owner.D_aimInput -= owner.Weapon.SetAimGoal;

        owner.SetArmsState(false, Vector2.zero, skeletonIdx);

        owner.GetSkillHolder.GetAnimator.SetTrigger("EndSkill");
        owner.GetSkillHolder.AnimationEnded();
        owner.GetSkillHolder.StartTimer();

        owner.SkillTutoAnimator.SetTrigger("finish");
    }

    private void Fire()
    {
        if (fire_TIMER > 0) return;

        fire_TIMER = fire_COOLDOWN;

        Quaternion q = _owner.Weapon.transform.rotation;
        Vector3 v = q.eulerAngles;
        v.z += 90f;

        v.x += Random.Range(rotationMinRandom, rotationMaxRandom);
        v.y += Random.Range(rotationMinRandom, rotationMaxRandom);

        q.eulerAngles = v;

        Vector2 aimTargetPosition = _owner.Weapon.IndicatorHolder.transform.GetChild(0).position;
        Vector2 aimerPosition = _owner.PivotOffset.transform.position;

        Vector2 dir = (aimTargetPosition - aimerPosition);

        float rand = Random.Range(rotationMinRandom, rotationMaxRandom);

        dir.x += rand;
        dir.y += rand;

        dir.Normalize();

        ProjectileBase proj = Instantiate(projectile, skillHolderTransform.position, q).GetComponent<ProjectileBase>();
        proj.Fire(dir, finalDamages, finalCrit, _owner.GetStats.Team);
    }
}

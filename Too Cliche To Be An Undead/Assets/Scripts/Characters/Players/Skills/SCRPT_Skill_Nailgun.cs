using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable/Entity/Skills/Nailgun")]
public class SCRPT_Skill_Nailgun : SCRPT_Skill
{
    [SerializeField] private GameObject projectile;

    [SerializeField] private float critModifier;

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
    }

    public override void StopSkill(PlayerCharacter owner)
    {
        isInUse = false;

        owner.D_aimInput -= owner.Weapon.SetAimGoal;

        owner.GetSkillHolder.GetAnimator.SetTrigger("EndSkill");
        owner.GetSkillHolder.AnimationEnded();
        owner.GetSkillHolder.StartTimer(cooldown);
    }

    private void Fire()
    {
        if (fire_TIMER > 0) return;

        fire_TIMER = fire_COOLDOWN;

        Quaternion q = _owner.Weapon.transform.rotation;
        Vector3 v = q.eulerAngles;
        v.z += 90f;
        q.eulerAngles = v;

        // Vector2 dir = _owner.Weapon.GetPreciseDirectionOfMouseOrGamepad().normalized;
         Vector2 dir = (_owner.Weapon.IndicatorHolder.transform.GetChild(0).position - _owner.transform.position).normalized;


        ProjectileBase proj = Instantiate(projectile, skillHolderTransform.position, q).GetComponent<ProjectileBase>();
        proj.Fire(dir, finalDamages, finalCrit, _owner.GetStats.Team);
    }
}

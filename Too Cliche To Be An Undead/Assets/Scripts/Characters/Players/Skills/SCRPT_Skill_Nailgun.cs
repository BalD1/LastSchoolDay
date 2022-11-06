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

    private Transform skillHolderTransform;

    private PlayerCharacter _owner;

    private bool inputPressed = false;

    public override void StartSkill(PlayerCharacter owner)
    {
        _owner = owner;
        isInUse = true;
        owner.GetSkillHolder.GetComponent<SpriteRenderer>().sortingLayerName = layerName.ToString();
        owner.GetSkillHolder.GetAnimator.Play(animationToPlay);
        owner.OffsetSkillHolder(offset);

        owner.D_startHoldAttackInput += PressInput;
        owner.D_endHoldAttackInput += ReleaseInput;

        fire_TIMER = 0;

        skillHolderTransform = owner.GetSkillHolder.transform;
    }

    public override void UpdateSkill(PlayerCharacter owner) 
    {
        if (fire_TIMER > 0) fire_TIMER -= Time.deltaTime;

        Fire();
    }

    public override void StopSkill(PlayerCharacter owner)
    {
        isInUse = false;
        owner.GetSkillHolder.GetAnimator.SetTrigger("EndSkill");
        owner.GetSkillHolder.AnimationEnded();
        owner.GetSkillHolder.StartTimer(cooldown);

        owner.D_startHoldAttackInput -= PressInput;
        owner.D_endHoldAttackInput -= ReleaseInput;
    }

    private void PressInput() => inputPressed = true;
    private void ReleaseInput() => inputPressed = false;

    private void Fire()
    {
        if (fire_TIMER > 0 || !inputPressed) return;

        fire_TIMER = fire_COOLDOWN;

        float finalDamages = _owner.GetStats.BaseDamages(_owner.StatsModifiers) * damages;
        int finalCrit = (int)(_owner.GetStats.CritChances(_owner.StatsModifiers) * critModifier);

        Quaternion q = _owner.Weapon.GetRotationOnMouseOrGamepad();
        Vector3 v = q.eulerAngles;
        v.z += 90f;
        q.eulerAngles = v;

        Vector2 dir = _owner.Weapon.GetPreciseDirectionOfMouseOrGamepad().normalized;

        ProjectileBase proj = Instantiate(projectile, skillHolderTransform.position, q).GetComponent<ProjectileBase>();
        proj.Fire(dir, finalDamages, finalCrit, _owner.GetStats.Team);
    }
}

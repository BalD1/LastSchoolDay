using UnityEngine;

[CreateAssetMenu(fileName = "AttackAmp", menuName = "Scriptable/Entity/Skills/AttackAmp")]
public class SCRPT_AttackAmp : SCRPT_Skill
{
    private PlayerCharacter owner;

    private bool canBeUsed = true;

    [SerializeField] private GameObject onAnimEndedEffect;

    [field: SerializeField] public float RangeModifier { get; private set; }
    [field: SerializeField] public float DamagesModifier { get; private set; }
    [field: SerializeField] public float KnockbackModifier { get; private set; }
    [field: SerializeField] public float CamerashakeIntensityModifier { get; private set; }

    [field: SerializeField] public int HitCounts { get; private set; }
    [field: SerializeField] public float Lifetime { get; private set; }

    [field: SerializeField] public string EffectID { get; private set; }

    public override void EarlyStart(PlayerCharacter owner)
    {
        owner.D_earlySkillStart?.Invoke();
    }

    public override void StartSkill(PlayerCharacter owner)
    {
        isInUse = true;

        this.owner = owner;

        owner.D_startSkill?.Invoke(owner.GetSkill.holdSkillAudio);

        owner.SkillTutoAnimator.SetTrigger(skillTutoAnimatorName);

        owner.GetSkillHolder.GetComponent<SpriteRenderer>().sortingLayerName = layerName.ToString();
        owner.GetSkillHolder.GetAnimator.Play(animationToPlay);
        owner.OffsetSkillHolder(offset);

        TextPopup.Create("Attaque +", owner.transform.position + (Vector3)owner.GetHealthPopupOffset);

        particles?.Create(owner.transform);
    }

    public override void UpdateSkill(PlayerCharacter owner)
    {
    }

    public override void StopSkill(PlayerCharacter owner)
    {
        OnHitEffects onHitEffects = new OnHitEffects(owner, EffectID, RangeModifier, DamagesModifier, KnockbackModifier, CamerashakeIntensityModifier, null, HitCounts, Lifetime, true);

        this.owner = owner;

        owner.Weapon.AddOnHitEffect(onHitEffects);

        if (onAnimEndedEffect != null)
        {
            GameObject effect = onAnimEndedEffect.Create(owner.transform);

            float animDuration = effect.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length;

            Destroy(effect, animDuration + .1f);
        }

        owner.D_successfulAttack += StartSkillTimerOnHit;
    }

    private void StartSkillTimerOnHit(bool lastAttack)
    {
        owner.GetSkillHolder.StartTimer();
        owner.D_successfulAttack -= StartSkillTimerOnHit;
        isInUse = false;
    }
}
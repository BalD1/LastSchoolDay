using UnityEngine;
using Spine.Unity;

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

    [SerializeField] private AudioClip voiceAttackClipOverride;

    [SerializeField] private Material flashMaterial;
    private Material baseMaterial;

    private LTDescr colorTween;

    public override void EarlyStart(PlayerCharacter owner)
    {
        owner.D_earlySkillStart?.Invoke();
    }

    public override void StartSkill(PlayerCharacter owner)
    {
        isInUse = true;

        this.owner = owner;

        owner.OnOverrideNextVoiceAttackAudio?.Invoke(voiceAttackClipOverride);
        owner.D_startSkill?.Invoke(owner.GetSkill.holdSkillAudio);

        owner.SkillTutoAnimator.SetTrigger(skillTutoAnimatorName);

        owner.GetSkillHolder.GetComponent<SpriteRenderer>().sortingLayerName = layerName.ToString();
        owner.GetSkillHolder.GetAnimator.Play(animationToPlay);
        owner.OffsetSkillHolder(offset);

        TextPopup.Create("Super Strike", owner.transform.position + (Vector3)owner.GetHealthPopupOffset);

        particles?.Create(owner.transform);

        Renderer skeletonRenderer = owner.SkeletonAnimation.GetComponent<Renderer>();
        owner.AnimationController.JasonMaterialOverride.gameObject.SetActive(true);
        skeletonRenderer.material.SetFloat("_FillPhase", 0);
        colorTween = LeanTween.value(0, 1, .25f).setOnUpdate(
            (float val) =>
            {
                skeletonRenderer.material.SetFloat("_FillPhase", val);
            }).setOnComplete(() =>
            {
                LeanTween.value(1, 0, .25f).setOnUpdate(
                (float val) =>
                {
                    skeletonRenderer.material.SetFloat("_FillPhase", val);
                }).setOnComplete(() =>
                {
                    owner.AnimationController.JasonMaterialOverride.gameObject.SetActive(false);
                });
            });
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

        owner.Weapon.performHitStop = true;
        owner.D_onAttack += StartSkillTimerOnHit;
    }

    private void StartSkillTimerOnHit(bool lastAttack)
    {
        owner.GetSkillHolder.StartTimer();
        owner.D_onAttack -= StartSkillTimerOnHit;
        isInUse = false;
    }
}
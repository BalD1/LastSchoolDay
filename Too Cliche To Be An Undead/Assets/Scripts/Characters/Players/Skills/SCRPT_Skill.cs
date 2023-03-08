using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SCRPT_Skill : ScriptableObject
{
    [SerializeField] protected E_SortlingLayers layerName;

    [SerializeField] protected bool aimAtMovements = true;

    [SerializeField] protected string skillTutoAnimatorName;

    public bool AimAtMovements { get => aimAtMovements; }

    public enum E_SortlingLayers
    {
        Background,
        Default,
        Foreground
    }

    [SerializeField] protected float cooldown;
    [SerializeField] protected float offset;
    [SerializeField] protected float range;
    [SerializeField] protected float duration;
    [SerializeField] protected float transitionDurationMultiplier = 1;
    [SerializeField] protected float startOffset = 0;

    [SerializeField] protected float damagesPercentageModifier;
    protected float finalDamages;

    [SerializeField] protected bool canMove;
    [field: SerializeField] public bool canAim = true;
    [SerializeField] protected bool loopAnims = true;

    [field: SerializeField] public bool is4D;

    [SerializeField] protected LayerMask entitiesToAffect;

    [SerializeField] protected string animationToPlay;
    [SerializeField] protected Sprite thumbnail;

    [SerializeField] protected GameObject particles;

    [field: SerializeField] public bool holdSkillAudio { get; private set; }

    protected bool isInUse;
    public bool IsInUse { get => isInUse; }

    public Sprite Thumbnail { get => thumbnail; }
    public float Cooldown { get => cooldown; }
    public float Range { get => range; }
    public float Duration { get => duration; }
    public float TransitionDurationMultiplier { get => transitionDurationMultiplier; }
    public float StartOffset { get => startOffset; }

    public bool CanMove { get => canMove; }
    public bool LoopAnims { get => loopAnims; }

    public string AnimationToPlay { get => animationToPlay; }

    public abstract void StartSkill(PlayerCharacter owner);
    public abstract void UpdateSkill(PlayerCharacter owner);
    public abstract void StopSkill(PlayerCharacter owner);

    public void ResetSkill() => isInUse = false;
}

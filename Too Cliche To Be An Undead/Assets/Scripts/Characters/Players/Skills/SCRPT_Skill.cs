using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SCRPT_Skill : ScriptableObject
{
    [SerializeField] protected float cooldown;
    [SerializeField] protected float offset;
    [SerializeField] protected float range;
    [SerializeField] protected float duration;

    [SerializeField] protected bool canMove;

    [SerializeField] protected LayerMask entitiesToAffect;

    [SerializeField] protected string animationToPlay;
    [SerializeField] protected Sprite thumbnail;

    public Sprite Thumbnail { get => thumbnail; }
    public float Cooldown { get => cooldown; }
    public float Range { get => range; }
    public float Duration { get => duration; }

    public bool CanMove { get => canMove; }

    public abstract void Use(PlayerCharacter owner);
}

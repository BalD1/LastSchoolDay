using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] protected float skillRange;
    public float SkillRange { get => skillRange; }

    [SerializeField] protected GameObject owner;
    public GameObject Owner { get => owner; }

    [SerializeField] protected Animator animator;
    public Animator Animator { get => animator; }

    public abstract void Setup(GameObject _owner);

#if UNITY_EDITOR
    [SerializeField] protected bool debugMode;
#endif

    public abstract void Use();
}

/*
 * 
 * 
 * 
 * 
 * 
 * 
 */

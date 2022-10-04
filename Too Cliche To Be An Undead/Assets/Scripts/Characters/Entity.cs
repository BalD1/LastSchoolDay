using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Rigidbody2D rb;
    public Rigidbody2D GetRb { get => rb; }

    [SerializeField] protected SpriteRenderer sprite;
    public SpriteRenderer GetSprite { get => sprite; }

    [SerializeField] protected Material hitMaterial;
    public Material GetHitMaterial { get => hitMaterial; }

    [SerializeField] protected Animator animator;
    public Animator GetAnimator { get => animator; }

    [SerializeField] protected SCRPT_EntityStats stats;
    public SCRPT_EntityStats GetStats { get => stats; }



    protected virtual void Start()
    {

    }
    
    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {

    }

    public bool RollCrit() => Random.Range(0, 100) >= GetStats.CritChances ? true : false;
}

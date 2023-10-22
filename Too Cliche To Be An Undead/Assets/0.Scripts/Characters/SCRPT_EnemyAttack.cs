using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SCRPT_EnemyAttack : ScriptableObject
{
    [SerializeField] protected bool canMove;

    [SerializeField] protected LayerMask entitiesToAffect;

    [field: SerializeField] public SO_SpineAnimationData AnticipAnim { get; private set; }
    [field: SerializeField] public SO_SpineAnimationData AttackAnim { get; private set; }

    [field: SerializeField] public float AttackDistance { get; private set; }
    [field: SerializeField] public float AttackDuration { get; private set; }
    [field: SerializeField] public float MinDurationBeforeAttack { get; private set; }
    [field: SerializeField] public float MaxDurationBeforeAttack { get; private set; }
    [field: SerializeField] public float telegraphRotationOffset { get; private set; }
    [field: SerializeField] public bool DamageOnTrigger { get; private set; }
    [field: SerializeField] public bool DamageOnCollision { get; private set; }
    [field: SerializeField] public Sprite telegraphSprite { get; private set; }
    [field: SerializeField] public Vector2 attackOffset { get; private set; }
    [field: SerializeField] public Vector2 telegraphVectorSize { get; private set; }

    [field: SerializeField] public bool SetTelegraphOnStart { get; private set; }


    public abstract void OnStart(EnemyBase owner);
    public abstract void OnUpdate(EnemyBase owner);
    public abstract void OnExit(EnemyBase owner);
}

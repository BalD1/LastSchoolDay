using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SO_EnemyAttack : ScriptableObject
{
    [SerializeField] protected bool canMove;

    [SerializeField] protected LayerMask entitiesToAffect;

    [field: SerializeField] public SO_SpineAnimationData AnticipAnim { get; private set; }
    [field: SerializeField] public AudioSource AnticipAudio { get; private set; }
    [field: SerializeField] public SO_SpineAnimationData AttackAnim { get; private set; }
    [field: SerializeField] public AudioSource AttackAudio { get; private set; }

    [field: SerializeField] public float TriggerDistance { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }
    [field: SerializeField] public float AttackDuration { get; private set; }
    [field: SerializeField] public float MinDurationBeforeAttack { get; private set; }
    [field: SerializeField] public float MaxDurationBeforeAttack { get; private set; }
    [field: SerializeField] public float TelegraphRotationOffset { get; private set; }
    [field: SerializeField] public bool DamageOnTrigger { get; private set; }
    [field: SerializeField] public bool DamageOnCollision { get; private set; }
    [field: SerializeField] public Sprite TelegraphSprite { get; private set; }
    [field: SerializeField] public Vector2 AttackOffset { get; private set; }
    [field: SerializeField] public Vector2 TelegraphVectorSize { get; private set; }

    [field: SerializeField] public bool SetTelegraphOnStart { get; private set; }


    public abstract void OnStart(EnemyBase owner);
    public abstract void OnUpdate(EnemyBase owner);
    public abstract void OnExit(EnemyBase owner);
}

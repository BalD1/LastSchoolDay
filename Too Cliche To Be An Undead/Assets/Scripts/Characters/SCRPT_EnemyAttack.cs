using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SCRPT_EnemyAttack : ScriptableObject
{
    [SerializeField] protected bool canMove;

    [SerializeField] protected LayerMask entitiesToAffect;

    public abstract void OnStart(EnemyBase owner);
    public abstract void OnUpdate(EnemyBase owner);
    public abstract void OnExit(EnemyBase owner);
}

using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SO_WeaponData;

[System.Serializable]
public class PlayerWeaponBase : IWeapon
{
    [SerializeField, ReadOnly] private SO_WeaponData weaponData;

    protected Collider2D[] hitEntities;

    private WeaponHandler handler;
    private IComponentHolder owner;
    private StatsHandler ownerStats;
    private PlayerMotor ownerMotor;
    private AimDirection ownerAim;

    private Queue<S_AttackData> attacksDataQueue;
    private S_AttackData? queuedAttack;
    [SerializeField, ReadOnly] private S_AttackData currentAttack;

    public event Action<S_DirectionalAnimationData> OnStartAttack;
    public event Action OnNextAttackAvailable;
    public event Action OnAttackEnded;
    public event Action OnBreakCombo;

    [SerializeField, ReadOnly] private float nextAttackTimer;
    [SerializeField, ReadOnly] private float breakComboTimer;
    [SerializeField, ReadOnly] private float attackTimer;

    public PlayerWeaponBase(SO_WeaponData weaponData, IComponentHolder owner, WeaponHandler handler)
    {
        Setup(weaponData, owner, handler);
    }

    public void Setup(SO_WeaponData weaponData, IComponentHolder owner, WeaponHandler handler)
    {
        this.weaponData = weaponData;
        this.owner = owner;

        owner.HolderTryGetComponent(IComponentHolder.E_Component.StatsHandler, out ownerStats);
        owner.HolderTryGetComponent(IComponentHolder.E_Component.Motor, out ownerMotor);
        owner.HolderTryGetComponent(IComponentHolder.E_Component.Aimer, out ownerAim);

        attacksDataQueue = new Queue<S_AttackData>(weaponData.AttackData);
        this.handler = handler;
    }

    public void Update()
    {
        HandleNextAttackTimer();
        HandleComboTimer();
        HandleAttackDurationTimer();
    }

    private void HandleNextAttackTimer()
    {
        if (nextAttackTimer <= 0) return;
        nextAttackTimer -= Time.deltaTime;
        if (nextAttackTimer > 0) return;

        if (queuedAttack != null) AskAttack();
        else OnNextAttackAvailable?.Invoke();
    }

    private void HandleAttackDurationTimer()
    {
        if (attackTimer <= 0) return;
        attackTimer -= Time.deltaTime;
        if (attackTimer > 0) return;
        OnAttackEnded?.Invoke();
    }

    private void HandleComboTimer()
    {
        if (breakComboTimer <= 0) return;
        breakComboTimer -= Time.deltaTime;
        if (breakComboTimer > 0) return;
        attacksDataQueue = new Queue<S_AttackData>(weaponData.AttackData);
        queuedAttack = null;
        OnBreakCombo?.Invoke();
    }

    public bool AskAttack()
    {
        if (nextAttackTimer > 0)
        {
            if (queuedAttack != null) return false;
            if (attacksDataQueue.Count == 0) return false;
            if (!currentAttack.Equals(weaponData.AttackData.Last()))
                QueueNextAttack();
            return false;
        }
        StartAttack();
        return true;
    }

    public void StartAttack()
    {
        SetupCurrentAttack();

        nextAttackTimer = currentAttack.NextAttackTimer;
        breakComboTimer = currentAttack.BreakComboTimer;
        attackTimer = currentAttack.AttackDuration;

        OnStartAttack?.Invoke(currentAttack.Animations);
        HandleMomentum();

        ownerStats.TryGetFinalStat(IStatContainer.E_StatType.AttackRange, out float statRange);
        statRange *= currentAttack.RangeModifier;
        hitEntities = Physics2D.OverlapCircleAll(handler.transform.position, statRange, weaponData.DamageableLayer);
        currentAttack.Effect?.Create().PlayAt(ownerAim.transform.position, ownerAim.transform.rotation);

        ownerStats.TryGetFinalStat(IStatContainer.E_StatType.BaseDamages, out float ownerDamages);
        ownerDamages *= currentAttack.DamagesMultiplier;

        ownerStats.TryGetFinalStat(IStatContainer.E_StatType.CritChances, out float ownerCritChances);

        foreach (var item in hitEntities)
        {
            INewDamageable damageable = item.GetComponent<INewDamageable>();
            if (damageable == null) return;
            DamageTarget(damageable, ownerDamages, ownerCritChances);
        }
    }

    private void DamageTarget(INewDamageable target, float ownerDamages, float ownerCritChances)
    {
        float finalDamages = ownerDamages;
        bool isCrit = RandomExtensions.PercentageChance(ownerCritChances);
        if (isCrit) finalDamages *= GameManager.CRIT_MULTIPLIER;

        INewDamageable.DamagesData damagesData = new INewDamageable.DamagesData(ownerStats.GetTeam(), weaponData.DamagesType, finalDamages, isCrit, owner as Entity);
        target.InflictDamages(damagesData);
    }

    private void HandleMomentum()
    {
        float onAttackMovementSpeed = currentAttack.AttackMovementForce;
        ownerStats.TryGetFinalStat(IStatContainer.E_StatType.Speed, out float ownerSpeed);
        if (ownerMotor.Velocity != Vector2.zero) onAttackMovementSpeed += ownerSpeed * currentAttack.AttackMovementMomentum;
        Vector2 attackDirection = ownerAim.GetPreciseAimDirection();
        ownerMotor.Body.AddForce(onAttackMovementSpeed * attackDirection, ForceMode2D.Impulse);
    }

    private void SetupCurrentAttack()
    {
        if (queuedAttack != null)
        {
            currentAttack = queuedAttack.Value;
            queuedAttack = null;
            return;
        }
        if (attacksDataQueue.Count == 0) return;

        currentAttack = attacksDataQueue.Dequeue();
    }

    public void QueueNextAttack()
    {
        queuedAttack = attacksDataQueue.Dequeue();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Boss_Attacking : FSM_Base<FSM_Boss_Manager>
{
    private BossZombie owner;

    private bool attack_flag;

    private float waitBeforeAttack_TIMER;
    private float attack_TIMER;

    public override void EnterState(FSM_Boss_Manager stateManager)
    {
        
        owner ??= stateManager.Owner;

        if (owner.Attack.DamageOnCollision) owner.d_EnteredTrigger += OnTrigger;

        owner.StartMaterialFlash("_Attacking", .1f);

        TextPopup.Create("!", owner.transform).transform.localPosition += (Vector3)owner.GetHealthPopupOffset;

        float durationBeforeAttack = Random.Range(owner.MinDurationBeforeAttack, owner.MaxDurationBeforeAttack);

        LeanTween.scale(owner.GetSprite.gameObject, owner.MaxScaleOnAttack, durationBeforeAttack / 2).setEase(owner.InType).setOnComplete(
            () =>
            {
                owner.SetAttackedPlayer(owner.CurrentPlayerTarget);
                LeanTween.scale(owner.GetSprite.gameObject, Vector2.one, durationBeforeAttack / 2).setEase(owner.OutType);
            });

        waitBeforeAttack_TIMER = durationBeforeAttack;
        attack_TIMER = owner.CurrentAttack.MaxDurationBeforeAttack;
        attack_flag = false;

        SCRPT_EnemyAttack enemyAttack = owner.Attack;

        Vector2 dir = (owner.PivotOffset.transform.position - owner.CurrentPlayerTarget.PivotOffset.transform.position).normalized;
        float lookAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        lookAngle += enemyAttack.telegraphRotationOffset;
        Quaternion telegraphRotation = Quaternion.AngleAxis(lookAngle, Vector3.forward);

        owner.AttackDirection = -dir;

        Vector2 telegraphSize = enemyAttack.telegraphVectorSize != Vector2.zero ? enemyAttack.telegraphVectorSize : new Vector2(enemyAttack.AttackDistance, enemyAttack.AttackDistance);

        owner.attackTelegraph.Setup(telegraphSize, owner.Attack.attackOffset, telegraphRotation, owner.Attack.telegraphSprite, durationBeforeAttack);

        owner.canBePushed = true;
        
    }

    public override void UpdateState(FSM_Boss_Manager stateManager)
    {
        if (waitBeforeAttack_TIMER > 0) waitBeforeAttack_TIMER -= Time.deltaTime;
        else if (waitBeforeAttack_TIMER <= 0 && !attack_flag)
        {
            owner.Attack.OnStart(owner);
            attack_flag = true;
        }

        if (attack_flag && attack_TIMER > 0) attack_TIMER -= Time.deltaTime;
    }

    public override void FixedUpdateState(FSM_Boss_Manager stateManager)
    {

    }

    public override void ExitState(FSM_Boss_Manager stateManager)
    {
        owner.Attack.OnExit(owner);
        if (owner.Attack.DamageOnCollision) owner.d_EnteredTrigger -= OnTrigger;

        owner.UnsetAttackedPlayer();
    }

    public override void Conditions(FSM_Boss_Manager stateManager)
    {
        //if (attack_TIMER <= 0) stateManager.SwitchState(stateManager.chasingState);
    }

    private void OnTrigger(Collider2D collider)
    {
        if (!owner.attackStarted) return;
        if (collider == null) return;
        PlayerCharacter p = collider.transform.GetComponentInParent<PlayerCharacter>();
        if (p == null) return;

        p.OnTakeDamages(owner.maxDamages_M, owner.RollCrit());
    }

    public override string ToString() => "Attacking";
}

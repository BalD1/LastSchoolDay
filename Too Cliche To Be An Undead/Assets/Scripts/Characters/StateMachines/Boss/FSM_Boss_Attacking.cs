using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Boss_Attacking : FSM_Base<FSM_Boss_Manager>
{
    private BossZombie owner;

    private bool attack_flag;

    private float waitBeforeAttack_TIMER;
    private float attack_TIMER;

    private bool switchToRecover = false;

    public override void EnterState(FSM_Boss_Manager stateManager)
    {
        owner ??= stateManager.Owner;

        switchToRecover = false;

        owner.attacksPatern.D_paterneEnded += SwitchToRecover;

        SetupNextAttack();
    }

    public override void UpdateState(FSM_Boss_Manager stateManager)
    {
        if (waitBeforeAttack_TIMER > 0) waitBeforeAttack_TIMER -= Time.deltaTime;
        else if (waitBeforeAttack_TIMER <= 0 && !attack_flag)
        {
            owner.CurrentAttack.attack.OnStart(owner);
            attack_flag = true;
        }

        if (attack_flag && attack_TIMER > 0) attack_TIMER -= Time.deltaTime;
        else if (attack_flag && attack_TIMER <= 0)
        {
            owner.attacksPatern.GetNextAttack();

            if (switchToRecover) return;

            attack_flag = false;
            SetupNextAttack();
        }
    }

    public override void FixedUpdateState(FSM_Boss_Manager stateManager)
    {

    }

    public override void ExitState(FSM_Boss_Manager stateManager)
    {
        owner.CurrentAttack.attack.OnExit(owner);
        if (owner.CurrentAttack.attack.DamageOnCollision) owner.d_EnteredTrigger -= OnTrigger;

        owner.attacksPatern.D_paterneEnded -= SwitchToRecover;
        switchToRecover = false;

        owner.attacksPatern.StartNewPatern();
        owner.UnsetAttackedPlayer();
    }

    public override void Conditions(FSM_Boss_Manager stateManager)
    {
        if (switchToRecover)
        {
            float recoveringTime = owner.attacksPatern.currentPatern.recoverTime;
            stateManager.SwitchState(stateManager.recoveringState.SetTimer(recoveringTime));
        }
    }

    public void SwitchToRecover()
    {
        switchToRecover = true;
    }

    private void SetupNextAttack()
    {
        SCRPT_EnemyAttack enemyAttack = owner.CurrentAttack.attack;

        if (enemyAttack.DamageOnCollision) owner.d_EnteredTrigger += OnTrigger;

        owner.StartMaterialFlash("_Attacking", .1f);

        TextPopup.Create("!", owner.transform).transform.localPosition += (Vector3)owner.GetHealthPopupOffset;

        float durationBeforeAttack = Random.Range(enemyAttack.MinDurationBeforeAttack, enemyAttack.MaxDurationBeforeAttack);

        LeanTween.scale(owner.GetSprite.gameObject, owner.MaxScaleOnAttack, durationBeforeAttack / 2).setEase(owner.InType).setOnComplete(
            () =>
            {
                owner.SetAttackedPlayer(owner.CurrentPlayerTarget);
                LeanTween.scale(owner.GetSprite.gameObject, Vector2.one, durationBeforeAttack / 2).setEase(owner.OutType);
            });

        waitBeforeAttack_TIMER = durationBeforeAttack;
        attack_TIMER = enemyAttack.MaxDurationBeforeAttack + owner.CurrentAttack.timeBeforeNext;
        attack_flag = false;

        Vector2 dir = (owner.PivotOffset.transform.position - owner.CurrentPlayerTarget.PivotOffset.transform.position).normalized;
        float lookAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        lookAngle += enemyAttack.telegraphRotationOffset;
        Quaternion telegraphRotation = Quaternion.AngleAxis(lookAngle, Vector3.forward);

        owner.AttackDirection = -dir;

        Vector2 telegraphSize = enemyAttack.telegraphVectorSize != Vector2.zero ? enemyAttack.telegraphVectorSize : new Vector2(enemyAttack.AttackDistance, enemyAttack.AttackDistance);

        owner.attackTelegraph.Setup(telegraphSize, enemyAttack.attackOffset, telegraphRotation, enemyAttack.telegraphSprite, durationBeforeAttack);

        owner.canBePushed = true;
    }

    private void OnTrigger(Collider2D collider)
    {
        if (!owner.attackStarted) return;
        if (collider == null) return;

        if (collider.transform.parent == null) return;

        PlayerCharacter p = collider.transform.parent.GetComponent<PlayerCharacter>();
        if (p == null) return;

        p.OnTakeDamages(owner.maxDamages_M, owner.RollCrit());
    }

    public override string ToString() => "Attacking";
}

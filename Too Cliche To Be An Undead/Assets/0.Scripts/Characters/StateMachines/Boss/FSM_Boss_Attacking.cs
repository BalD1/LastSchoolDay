using UnityEngine;
using static FSM_Boss_Manager;

public class FSM_Boss_Attacking : FSM_Base<FSM_Boss_Manager, E_BossState>
{
    private BossZombie owner;

    private bool attack_flag;

    private float waitBeforeAttack_TIMER;
    private float attack_TIMER;

    private bool switchToRecover = false;

    public override void EnterState(FSM_Boss_Manager stateManager)
    {
        base.EnterState(stateManager);

        switchToRecover = false;

        SetupNextAttack();
    }

    public override void UpdateState(FSM_Boss_Manager stateManager)
    {
        if (owner.HitStop_TIMER > 0) return;

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
        base.ExitState(stateManager);
        owner.CurrentAttack.attack.OnExit(owner);

        switchToRecover = false;

        owner.attacksPatern.StartNewPatern();
        owner.UnsetAttackedPlayer();
    }

    public override void Conditions(FSM_Boss_Manager stateManager)
    {
        if (switchToRecover)
        {
            float recoveringTime = owner.attacksPatern.currentPatern.recoverTime;
            stateManager.SwitchState<FSM_Boss_Recovering>(FSM_Boss_Manager.E_BossState.Recovering).SetTimer(recoveringTime);
        }
        if (owner.CurrentHP <= 0)
            stateManager.SwitchState(FSM_Boss_Manager.E_BossState.Dead);
    }

    protected override void EventsSubscriber(FSM_Boss_Manager stateManager)
    {
        owner.attacksPatern.D_paterneEnded += SwitchToRecover;
    }

    protected override void EventsUnsubscriber(FSM_Boss_Manager stateManager)
    {
        if (owner.CurrentAttack.attack.DamageOnTrigger) owner.OnEnteredBodyTrigger -= OnTrigger;
        if (owner.CurrentAttack.attack.DamageOnCollision)
        {
            owner.d_EnteredCollider -= OnCollision;
            owner.D_entityEnteredCollider -= OnEntityCollision;
        }

        owner.attacksPatern.D_paterneEnded -= SwitchToRecover;
    }

    public void SwitchToRecover()
    {
        switchToRecover = true;
    }

    private void SetupNextAttack()
    {
        owner.OnEnteredBodyTrigger -= OnTrigger;
        owner.d_EnteredCollider -= OnCollision;
        owner.D_entityEnteredCollider -= OnEntityCollision;

        if (!owner.IsAlive()) return;

        SCRPT_EnemyAttack enemyAttack = owner.CurrentAttack.attack;

        // if the attack uses enemy's collisions trigger
        if (enemyAttack.DamageOnTrigger) owner.OnEnteredBodyTrigger += OnTrigger;
        if (owner.CurrentAttack.attack.DamageOnCollision)
        {
            owner.d_EnteredCollider += OnCollision;
            owner.D_entityEnteredCollider += OnEntityCollision;
        }

        // Create a text feedback
        TextPopup.Create("!", owner.transform).transform.localPosition = (Vector3)owner.GetHealthPopupOffset;

        // how long should the enemy wait before attacking ?
        float durationBeforeAttack = Random.Range(enemyAttack.MinDurationBeforeAttack, enemyAttack.MaxDurationBeforeAttack);

        if (durationBeforeAttack > 0)
        {
        // Set the anticipation anim
            owner.animationController.SetAnimation(owner.AnimationData.AttackAnticipAnim, true);

            // Second feedback using the enemy scale
            LeanTween.scale(owner.SkeletonAnimation.gameObject, owner.MaxScaleOnAttack, durationBeforeAttack / 2).setEase(owner.InType).setOnComplete(
                () =>
                {
                    owner.SetAttackedPlayer(owner.CurrentPlayerTarget);
                    LeanTween.scale(owner.SkeletonAnimation.gameObject, Vector2.one, durationBeforeAttack / 2).setEase(owner.OutType);
                });
        }

        waitBeforeAttack_TIMER = durationBeforeAttack;
        attack_TIMER = enemyAttack.MaxDurationBeforeAttack + enemyAttack.AttackDuration + owner.CurrentAttack.timeBeforeNext;
        attack_flag = false;

        // should we set the telegraph now or wait for the attack OnStart ?
        if (enemyAttack.SetTelegraphOnStart == false)
        {
            Vector2 dir = (owner.PivotOffset.transform.position - owner.CurrentPlayerTarget.PivotOffset.transform.position).normalized;
            float lookAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            lookAngle += enemyAttack.telegraphRotationOffset;
            Quaternion telegraphRotation = Quaternion.AngleAxis(lookAngle, Vector3.forward);

            owner.AttackDirection = -dir;

            Vector2 telegraphSize = enemyAttack.telegraphVectorSize != Vector2.zero ? enemyAttack.telegraphVectorSize : new Vector2(enemyAttack.AttackDistance, enemyAttack.AttackDistance);

            owner.attackTelegraph.Setup(telegraphSize, enemyAttack.attackOffset, telegraphRotation, enemyAttack.telegraphSprite, durationBeforeAttack);
        }

        owner.canBePushed = true;
    }

    private void OnCollision(Collision2D collision)
    {
        if (!owner.attackStarted) return;
        if (collision == null) return;

        if (collision.transform.parent == null) return;

        PlayerCharacter p = collision.gameObject.GetComponent<PlayerCharacter>();
        if (p == null) return;

        owner.OnHitEntity(p);
    }
    private void OnEntityCollision(Entity entity)
    {
        if (!owner.attackStarted) return;
        if (entity == null) return;

        owner.OnHitEntity(entity);
    }
    private void OnTrigger(Collider2D collider)
    {
        if (!owner.attackStarted) return;
        if (collider == null) return;

        if (collider.transform.parent == null) return;


        PlayerCharacter p = collider.transform.parent.GetComponent<PlayerCharacter>();
        if (p == null) return;

        owner.OnHitEntity(p);
    }

    public override void Setup(FSM_Boss_Manager stateManager)
    {
        owner = stateManager.Owner;
    }

    public override string ToString() => "Attacking";
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombie : EnemyBase
{
    [SerializeField] private FSM_NZ_Manager stateManager;

    [field: SerializeField] public EnemyVision Vision { get; private set; }
    [field: SerializeField] public bool isIdle = false;

    [field:  SerializeField] public bool allowWander { get; private set; }

    [field: SerializeField] public bool tutorialZombie { get; private set; }

    [SerializeField] private float attack_DURATION = .3f;
    public float Attack_DURATION { get => attack_DURATION; }

    public bool attackStarted;

    public Vector2 AttackDirection { get; set; }

    public const int maxDistanceFromPlayer = 15;

    [field: SerializeField] public float timeOfDeath;

    public static NormalZombie Create(Vector2 pos, bool seeAtStart)
    {
        NormalZombie res = Instantiate(GameAssets.Instance.GetRandomZombie, pos, Quaternion.identity).GetComponent<NormalZombie>();

        res.isIdle = !seeAtStart;
        res.Vision.targetPlayerAtStart = seeAtStart;

        if (!seeAtStart) res.ResetTarget();

        return res;
    }

    protected override void Start()
    {
        base.Start();
        Pathfinding?.StartUpdatePath();

        if (tutorialZombie || isIdle) return;

        SpawnersManager.Instance.AddZombie();

        this.Vision.TargetClosestPlayer();

        d_OnDeath += SpawnersManager.Instance.RemoveZombie;
    }

    protected override void Update()
    {
        base.Update();

        if (isIdle) return;

        if (Vector2.Distance(this.transform.position, GameManager.Player1Ref.transform.position) > maxDistanceFromPlayer) ForceKill();
    }

    public void ForceKill()
    {
        if (isIdle) return;

        attackedPlayer?.RemoveAttacker(this);
        this.gameObject.SetActive(false);
        SpawnersManager.Instance.TeleportZombie(this);
        ResetAll();

    }
    public override void OnDeath(bool forceDeath = false)
    {
        base.OnDeath(forceDeath);

        if (tutorialZombie)
        {
            Destroy(this.gameObject);
            return;
        }
        SpawnersManager.Instance.ZombiesPool.Enqueue(this);
        timeOfDeath = Time.timeSinceLevelLoad;

        if (isIdle)
        {
            isIdle = false;
            d_OnDeath += SpawnersManager.Instance.RemoveZombie;
        }
        this.gameObject.SetActive(false);
        ResetAll();
    }

    private void ResetAll()
    {
        StopAllCoroutines();
        this.RemoveModifiersAll();
        this.RemoveAllTickDamages();
        this.ResetStats();
        this.ResetTarget();
    }

    public void Reenable(Vector2 pos, bool addToSpawner = true)
    {
        this.transform.position = pos;
        this.ResetStats();
        this.stateManager.SwitchState(stateManager.chasingState);

        if (addToSpawner) SpawnersManager.Instance.AddZombie();

        this.Vision.TargetClosestPlayer();
        this.sprite.material.SetInt("_Hit", 0);
        this.gameObject.SetActive(true);
    }

    public override void Stun(float duration, bool resetAttackTimer = false)
    {
        stateManager.SwitchState(stateManager.stunnedState.SetDuration(duration, resetAttackTimer));
        this.attackTelegraph.CancelTelegraph();
    }

    public override Vector2 Push(Vector2 pusherPosition, float pusherForce, Entity originalPusher)
    {
        if (!canBePushed) return Vector2.zero;

        Vector2 v = base.Push(pusherPosition, pusherForce, originalPusher);

        if (v.magnitude <= Vector2.zero.magnitude) return Vector2.zero;

        stateManager.SwitchState(stateManager.pushedState.SetForce(v, originalPusher));

        return v;
    }

    public override void SetAttackedPlayer(PlayerCharacter target)
    {
        base.SetAttackedPlayer(target);

        Vector2 targetPosition = this.CurrentPlayerTarget == null ? this.storedTargetPosition : this.CurrentPlayerTarget.transform.position;
        //AttackDirection = (targetPosition - (Vector2)this.transform.position).normalized;
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, Attack.AttackDistance);
    }

}

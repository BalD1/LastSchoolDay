using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombie : EnemyBase
{
    [SerializeField] private FSM_NZ_Manager stateManager;

    [SerializeField] private float attack_DURATION = .3f;
    public float Attack_DURATION { get => attack_DURATION; }

    public bool attackStarted;

    public Vector2 AttackDirection { get; private set; }

    public static NormalZombie Create(Vector2 pos)
    {
        GameObject gO = Instantiate(GameAssets.Instance.NormalZombiePF, pos, Quaternion.identity);

        NormalZombie nZ = gO.GetComponent<NormalZombie>();

        return nZ;
    }

    protected override void Start()
    {
        base.Start();
        Pathfinding?.StartUpdatePath();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void OnDeath(bool forceDeath = false)
    {
        base.OnDeath(forceDeath);
        Destroy(this.gameObject);
    }

    public override void Stun(float duration, bool resetAttackTimer = false)
    {
        stateManager.SwitchState(stateManager.stunnedState.SetDuration(duration, resetAttackTimer));
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
        AttackDirection = (targetPosition - (Vector2)this.transform.position).normalized;
    }

}

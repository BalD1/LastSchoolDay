using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BalDUtilities.MouseUtils;
using BalDUtilities.VectorUtils;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] protected PlayerCharacter owner;
    public PlayerCharacter Owner { get => owner; }

    [SerializeField] private GameObject indicatorHolder;
    public GameObject IndicatorHolder { get => indicatorHolder; }

    [SerializeField] private float slerpSpeed = 10f;
    public float SlerpSpeed { get => slerpSpeed; }

    [SerializeField] private float onHitKnockback = 2f;

    [SerializeField] private float attacksComboResetTime = .5f;
    private float attacksComboReset_TIMER;

    public float attackDuration = .2f;
    private float attack_TIMER;

    [SerializeField] private float afterCombo_COOLDOWN = .3f;
    private float afterCombo_TIMER;

    [SerializeField] private int maxAttacksCombo = 3;

#if UNITY_EDITOR
    [SerializeField] protected bool debugMode;
#endif

    [SerializeField] protected LayerMask damageablesLayer;

    [SerializeField] public ParticleSystem slashParticles;

    [SerializeField] protected ParticleSystem hitParticles;

    [SerializeField] protected PlayersManager.GamepadShakeData normalHitGamepadShake;
    [SerializeField] protected PlayersManager.GamepadShakeData bigHitGamepadShake;

    private int attackCount;

    protected Collider2D[] hitEntities;

    private float lookAngle;

    public bool isAttacking;

    public bool prepareNextAttack;
    public bool inputStored;

    public delegate void NextAttack(int attackIdx);
    public NextAttack D_nextAttack;

    private Vector2 aimGoal;

    protected virtual void Awake()
    {
        owner ??= this.transform.GetComponentInParent<PlayerCharacter>();
    }

    private void Update()
    {
        if (attacksComboReset_TIMER > 0)
        {
            attacksComboReset_TIMER -= Time.deltaTime;
            if (attacksComboReset_TIMER <= 0) attackCount = 0;
        }

        if (attack_TIMER > 0)
        {
            attack_TIMER -= Time.deltaTime;
            if (attack_TIMER <= 0)
            {
                isAttacking = false;
                if (attackCount >= maxAttacksCombo)
                {
                    attackCount = 0;
                    inputStored = false;
                    owner.StateManager.SwitchState(owner.StateManager.idleState);
                    afterCombo_TIMER = afterCombo_COOLDOWN;
                    return;
                }

                if (inputStored)
                {
                    inputStored = false;

                    PlayerAnimationController ownerAnims = owner.AnimationController;

                    AskForAttack();
                    switch (owner.Weapon.GetGeneralDirectionOfMouseOrGamepad())
                    {
                        case Vector2 v when v == Vector2.up:
                            ownerAnims.FlipSkeleton(false);
                            ownerAnims.SetAnimation(ownerAnims.animationsData.AttackAnim_up, false);
                            break;

                        case Vector2 v when v == Vector2.down:
                            ownerAnims.FlipSkeleton(true);
                            ownerAnims.SetAnimation(ownerAnims.animationsData.AttackAnim_down, false);
                            break;

                        case Vector2 v when v == Vector2.left:
                            ownerAnims.FlipSkeleton(false);
                            ownerAnims.SetAnimation(ownerAnims.animationsData.AttackAnim_side, false);
                            break;

                        case Vector2 v when v == Vector2.right:
                            ownerAnims.FlipSkeleton(true);
                            ownerAnims.SetAnimation(ownerAnims.animationsData.AttackAnim_side, false);
                            break;
                    }
                }
                else owner.StateManager.SwitchState(owner.StateManager.idleState);
            }
        }

        if (afterCombo_TIMER > 0) afterCombo_TIMER -= Time.deltaTime;
    }

    #region Rotations

    /// <summary>
    /// Check if we should aim at movements. If yes, rotate depending on gamepad or mouse.
    /// If not, Rotate on mouse.
    /// </summary>
    /// <param name="aimAtMovements"></param>
    public void SetRotation(bool aimAtMovements = true)
    {
        if (aimAtMovements) this.transform.rotation = GetRotationOnMouseOrGamepad();
        else RotateOnMouse();

        indicatorHolder.transform.rotation = this.transform.rotation;
    }

    /// <summary>
    /// Returns quaternion depending on if the user is on keyboard or gamepad
    /// </summary>
    /// <returns></returns>
    public Quaternion GetRotationOnMouseOrGamepad()
    {
        if (owner.Inputs.currentControlScheme == null) return Quaternion.identity;
        if (owner.Inputs.currentControlScheme.Equals(PlayerCharacter.SCHEME_KEYBOARD))
        {
            return GetRotationOnMouse();
        }
        else
        {
            Vector2 c = owner.LastDirection.normalized;
            lookAngle = Mathf.Atan2(c.y, c.x) * Mathf.Rad2Deg;
            return Quaternion.AngleAxis(lookAngle + 180, Vector3.forward);
        }
    }

    /// <summary>
    /// Rotates depending on mouse position. Uses <see cref="GetRotationOnMouse"/>
    /// </summary>
    public void RotateOnMouse()
    {
        if (owner.Inputs.currentControlScheme.Equals(PlayerCharacter.SCHEME_KEYBOARD))
            this.transform.rotation = GetRotationOnMouse();
    }

    /// <summary>
    /// Returns a quaternion depending on mouse position
    /// </summary>
    /// <returns></returns>
    public Quaternion GetRotationOnMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5f;

        Vector3 selfPosByCam = Camera.main.WorldToScreenPoint(Owner.transform.position);

        mousePos.x -= selfPosByCam.x;
        mousePos.y -= selfPosByCam.y;

        lookAngle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(lookAngle + 180, Vector3.forward);
    }

    /// <summary>
    /// Auto aims toward target
    /// </summary>
    /// <param name="target"></param>
    public void SetRotationTowardTarget(Transform target)
    {
        if (target == null) return;
        if (owner.Inputs.currentControlScheme.Equals(PlayerCharacter.SCHEME_KEYBOARD)) return;

        Vector2 dir = (target.position - this.transform.position).normalized;
        lookAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(lookAngle + 180, Vector3.forward);
        owner.LastDirection = dir;
    }

    public void SetAimGoal(Vector2 goal)
    {
        if (VectorMaths.Vector2ApproximatlyEquals(goal, Vector2.zero, .05f)) return;

        aimGoal = goal;
    }

    /// <summary>
    /// Use this if we should rotate depending on aim instead of movements
    /// </summary>
    public void RotateOnAim()
    {
        if (owner.Inputs.currentControlScheme.Equals(PlayerCharacter.SCHEME_GAMEPAD))
        {
            lookAngle = Mathf.Atan2(aimGoal.y, aimGoal.x) * Mathf.Rad2Deg;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.AngleAxis(lookAngle + 180, Vector3.forward), Time.deltaTime * slerpSpeed);
        }
        else RotateOnMouse();
    }

    public Vector2 GetPreciseDirectionOfMouseOrGamepad()
    {
        if (owner.Inputs.currentControlScheme.Equals(PlayerCharacter.SCHEME_KEYBOARD))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 5f;

            Vector3 selfPosByCam = Camera.main.WorldToScreenPoint(Owner.transform.position);

            mousePos.x -= selfPosByCam.x;
            mousePos.y -= selfPosByCam.y;

            return mousePos;
        }
        else return owner.LastDirection.normalized;
    }

    public Vector2 GetGeneralDirectionOfMouseOrGamepad()
    {
        float rot = this.transform.rotation.eulerAngles.z;

        if (rot > 45 && rot <= 135) return Vector2.down;
        else if (rot > 135 && rot <= 225) return Vector2.right;
        else if (rot > 225 && rot <= 315) return Vector2.up;
        else return Vector2.left;
    }

    #endregion

    public virtual void AskForAttack()
    {
        if (afterCombo_TIMER > 0) return;
        if (isAttacking)
        {
            inputStored = true;
            return;
        }

        StartWeaponAttack(attackCount >= maxAttacksCombo - 1);
    }

    public virtual void StartWeaponAttack(bool isLastAttack) 
    {
        SetRotation();

        attackCount++;
        attacksComboReset_TIMER = attacksComboResetTime;
        attack_TIMER = attackDuration;
        isAttacking = true;
    }

    public void SuccessfulHit(Vector3 hitPosition, Entity e, bool addKnockback, float speedModifier)
    {
        if (e == null) return;

        Vector3 effectObjectPos = slashParticles.gameObject.transform.position;
        float dist = Vector2.Distance(effectObjectPos, hitPosition) / 2;
        Vector2 dir = (hitPosition - effectObjectPos).normalized;

        Instantiate(hitParticles, effectObjectPos + (Vector3)(dist * dir), Quaternion.identity);

        float finalKnockback = onHitKnockback - e.GetStats.Weight + speedModifier;
        if (finalKnockback > 0 && addKnockback)
        {
            e.Stun(.1f, true);

            Vector2 dirToPlayer = (e.transform.position - owner.transform.position).normalized;
            e.GetRb.AddForce(finalKnockback * dirToPlayer, ForceMode2D.Impulse);
        }
    }

    public void ResetAttack()
    {
        slashParticles.Stop();
        isAttacking = false;
        prepareNextAttack = false;
        inputStored = false;
    }
}

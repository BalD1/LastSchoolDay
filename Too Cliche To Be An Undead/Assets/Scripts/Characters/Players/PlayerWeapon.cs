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

    [SerializeField] private float onHitKnockback = 2f;

#if UNITY_EDITOR
    [SerializeField] protected bool debugMode;
#endif

    [SerializeField] protected LayerMask damageablesLayer;

    [SerializeField] protected Animator effectAnimator;
    public Animator EffectAnimator { get => effectAnimator; }

    [SerializeField] protected ParticleSystem hitParticles;

    protected Collider2D[] hitEntities;

    private float lookAngle;

    public bool isAttacking;
    public bool attackEnded;

    public bool prepareNextAttack;
    public bool inputStored;

    public delegate void NextAttack(int attackIdx);
    public NextAttack D_nextAttack;

    private Vector2 aimGoal;

    protected virtual void Awake()
    {
        owner ??= this.transform.GetComponentInParent<PlayerCharacter>();
        effectAnimator ??= this.transform.GetComponentInChildren<Animator>();
    }

    public void FollowMouse(bool aimAtMovements = true)
    {
        if (aimAtMovements) this.transform.rotation = GetRotationOnMouseOrGamepad();
        else  RotateOnMouse();

        indicatorHolder.transform.rotation = this.transform.rotation;
    }

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

    public void RotateOnMouse()
    {
        if (owner.Inputs.currentControlScheme.Equals(PlayerCharacter.SCHEME_KEYBOARD))
            this.transform.rotation = GetRotationOnMouse();
    }

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

    public void SetRotationTowardTarget(Transform target)
    {
        Vector2 dir = (target.position - this.transform.position).normalized;
        lookAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(lookAngle + 180, Vector3.forward);
    }

    public void SetAimGoal(Vector2 goal)
    {
        if (VectorMaths.Vector2ApproximatlyEquals(goal, Vector2.zero, .05f)) return;

        aimGoal = goal;
    }

    public void RotateOnAim()
    {
        if (owner.Inputs.currentControlScheme.Equals(PlayerCharacter.SCHEME_GAMEPAD))
        {
            //cus.x -= owner.transform.position.x;
            //cus.y -= owner.transform.position.y;

            lookAngle = Mathf.Atan2(aimGoal.y, aimGoal.x) * Mathf.Rad2Deg;
            //this.transform.rotation = Quaternion.AngleAxis(lookAngle + 180, Vector3.forward);
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

    public virtual void StartWeaponAttack(bool isLastAttack) { }

    public void SuccessfulHit(Vector3 hitPosition, Entity e, bool addKnockback)
    {
        Vector3 effectObjectPos = effectAnimator.gameObject.transform.position;
        float dist = Vector2.Distance(effectObjectPos, hitPosition) / 2;
        Vector2 dir = (hitPosition - effectObjectPos).normalized;

        Instantiate(hitParticles, effectObjectPos + (Vector3)(dist * dir), Quaternion.identity);

        float finalKnockback = onHitKnockback - e.GetStats.Weight;
        if (finalKnockback > 0 && addKnockback)
        {
            e.Stun(.1f, true);

            Vector2 dirToPlayer = (e.transform.position - owner.transform.position).normalized;
            e.GetRb.AddForce(finalKnockback * dirToPlayer, ForceMode2D.Impulse);
        }
    }

    public Vector2 GetGeneralDirectionOfMouseOrGamepad()
    {
        float rot = this.transform.rotation.eulerAngles.z;

        if (rot > 45 && rot <= 135) return Vector2.down;
        else if (rot > 135 && rot <= 225) return Vector2.right;
        else if (rot > 225 && rot <= 315) return Vector2.up;
        else return Vector2.left;
    }

    public void ResetAttack()
    {
        isAttacking = false;
        attackEnded = false;
        prepareNextAttack = false;
        inputStored = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BalDUtilities.MouseUtils;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] protected PlayerCharacter owner;
    public PlayerCharacter Owner { get => owner; }

    [SerializeField] private GameObject indicatorHolder;

#if UNITY_EDITOR
    [SerializeField] protected bool debugMode;
#endif

    [SerializeField] protected LayerMask damageablesLayer;

    [SerializeField] protected Animator effectAnimator;
    public Animator EffectAnimator { get => effectAnimator; }

    protected Collider2D[] hitEntities;

    private float lookAngle;

    public bool isAttacking;
    public bool attackEnded;

    public bool prepareNextAttack;
    public bool inputStored;

    public delegate void NextAttack(int attackIdx);
    public NextAttack D_nextAttack;

    protected virtual void Awake()
    {
        owner ??= this.transform.GetComponentInParent<PlayerCharacter>();
        effectAnimator ??= this.transform.GetComponentInChildren<Animator>();
    }

    public void FollowMouse()
    {
        this.transform.rotation = GetRotationOnMouseOrGamepad();
        indicatorHolder.transform.rotation = this.transform.rotation;
    }

    public Quaternion GetRotationOnMouseOrGamepad()
    {
        if (owner.Inputs.currentControlScheme.Equals(PlayerCharacter.SCHEME_KEYBOARD))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 5f;

            Vector3 selfPosByCam = Camera.main.WorldToScreenPoint(Owner.transform.position);

            mousePos.x -= selfPosByCam.x;
            mousePos.y -= selfPosByCam.y;

            lookAngle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            return Quaternion.AngleAxis(lookAngle + 180, Vector3.forward);
        }
        else
        {
            Vector2 c = owner.LastDirection.normalized;
            lookAngle = Mathf.Atan2(c.y, c.x) * Mathf.Rad2Deg;
            return Quaternion.AngleAxis(lookAngle + 180, Vector3.forward);
        }
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

    public void SuccessfulHit(Vector3 hitPosition)
    {
        float dist = Vector2.Distance(this.transform.position, hitPosition) / 2;
        Vector2 dir = (hitPosition - this.transform.position).normalized;

        //Instantiate(hitParticles, this.transform.position + (dir * dist), Quaternion.identity);

        // Screen shake
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

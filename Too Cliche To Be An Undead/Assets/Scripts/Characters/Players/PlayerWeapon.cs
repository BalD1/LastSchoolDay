using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BalDUtilities.MouseUtils;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private PlayerCharacter owner;
    public PlayerCharacter Owner { get => owner; }

    [SerializeField] private float maxRange = 2f;
    [SerializeField] private float lastAttackDamagesMultiplier = 1.5f;

#if UNITY_EDITOR
    [SerializeField] private bool debugMode;
#endif

    [SerializeField] private LayerMask damageablesLayer;

    [SerializeField] private Animator effectAnimator;

    private Collider2D[] hitEntities;

    private Vector2 initialPosition;
    private Vector2 targetPosition;

    private float lookAngle;

    public bool isAttacking;
    public bool attackEnded;

    public bool prepareNextAttack;
    public bool inputStored;

    public delegate void NextAttack(int attackIdx);
    public NextAttack D_nextAttack;

    public void FollowMouse()
    {
        targetPosition.x = MousePosition.GetMouseWorldPosition().x - owner.transform.position.x;
        targetPosition.y = MousePosition.GetMouseWorldPosition().y - owner.transform.position.y;
        targetPosition = Vector2.ClampMagnitude(targetPosition, maxRange);

        this.transform.position = owner.transform.position + (Vector3)targetPosition;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5f;

        Vector3 selfPosByCam = Camera.main.WorldToScreenPoint(Owner.transform.position);

        mousePos.x -= selfPosByCam.x;
        mousePos.y -= selfPosByCam.y;

        lookAngle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(lookAngle - 90, Vector3.forward);
    }

    public void DamageEnemiesInRange(bool isLastAttack)
    {
        hitEntities = Physics2D.OverlapCircleAll(this.transform.position, owner.GetStats.AttackRange, damageablesLayer);
        foreach (var item in hitEntities)
        {
            var damageable = item.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                float damages = owner.GetStats.BaseDamages;

                if (isLastAttack) damages *= lastAttackDamagesMultiplier;

                if (damageable.OnTakeDamages(damages, owner.GetStats.Team, owner.RollCrit()) == false)
                    continue;

                float dist = Vector2.Distance(this.transform.position, item.transform.position) / 2;
                Vector2 dir = (item.transform.position - this.transform.position).normalized;

                // Instantiate(hitParticles, this.transform.position + (dir * dist), Quaternion.identity);

                // Screen shake
            }
        }
    }

    public Vector2 GetDirectionOfMouse()
    {
        float rot = this.transform.rotation.eulerAngles.z;

        if (rot > 45 && rot <= 135) return Vector2.left;           //left
        else if (rot > 135 && rot <= 225) return Vector2.down;     //down
        else if (rot > 225 && rot <= 315) return Vector2.right;    //right
        else return Vector2.up;                                    //up
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!debugMode || owner == null) return;

        Gizmos.DrawWireSphere(this.transform.position, owner.GetStats.AttackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(owner.transform.position, maxRange);
#endif
    }
}

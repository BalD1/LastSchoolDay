using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BalDUtilities.MouseUtils;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private PlayerCharacter owner;
    [SerializeField] private float maxRange = 2f;

#if UNITY_EDITOR
    [SerializeField] private bool debugMode;
#endif

    [SerializeField] private LayerMask damageablesLayer;

    private Collider2D[] hitEntities;

    private Vector2 initialPosition;
    private Vector2 targetPosition;

    private float lookAngle;

    private void Update()
    {
        FollowMouse();
    }

    public void FollowMouse()
    {
        targetPosition.x = MousePosition.GetMouseWorldPosition().x - owner.transform.position.x;
        targetPosition.y = MousePosition.GetMouseWorldPosition().y - owner.transform.position.y;
        targetPosition = Vector2.ClampMagnitude(targetPosition, maxRange);

        this.transform.position = owner.transform.position + (Vector3)targetPosition;

        if (targetPosition.x < 0 && !owner.IsFacingLeft()) owner.Flip(false);
        else if (targetPosition.x > 0 && owner.IsFacingLeft()) owner.Flip(true);
    }

    public void DamageEnemiesInRange()
    {
        hitEntities = Physics2D.OverlapCircleAll(this.transform.position, owner.GetStats.AttackRange, damageablesLayer);
        foreach (var item in hitEntities)
        {
            var damageable = item.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                if (damageable.OnTakeDamages(owner.GetStats.BaseDamages, owner.GetStats.Team, owner.RollCrit()) == false)
                    continue;

                float dist = Vector2.Distance(this.transform.position, item.transform.position) / 2;
                Vector2 dir = (item.transform.position - this.transform.position).normalized;
                // Instantiate(hitParticles, this.transform.position + (dir * dist), Quaternion.identity);

                // Screen shake
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!debugMode || owner == null) return;

        Gizmos.DrawWireSphere(this.transform.position, owner.GetStats.AttackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(owner.transform.position, maxRange);
    }
}

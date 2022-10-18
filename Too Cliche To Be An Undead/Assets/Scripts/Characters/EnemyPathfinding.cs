using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEditorInternal.VR;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private EnemyBase owner;

    [SerializeField] private Seeker seeker;

    [SerializeField] private float nextWaypointDistance = 3f;
    [SerializeField] private float updatePathCooldown = .5f;

    [SerializeField] private float targetRandomOffset = 0f;

    [SerializeField] private bool chaoticTargetPosition = false;

#if UNITY_EDITOR
    [SerializeField] private bool debugMode = false; 
#endif

    private bool fleeFromTarget = false;
    private bool reachedEndOfPath = false;

    private Path path;

    private int currentWaypoint = 0;

    public void StartUpdatePath() => InvokeRepeating(nameof(TryUpdatePath), 0f, updatePathCooldown);
    public void StopUpdatepath() => CancelInvoke();

    public void TryUpdatePath()
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;

        if (seeker.IsDone())
        {
            UpdatePath();
        }
    }
    public void UpdatePath()
    {
        if (fleeFromTarget)
        {
            Vector2 direction = (Vector2)this.transform.position - owner.CurrentPositionTarget;
            SetPath((Vector2)this.transform.position + direction);
        }
        else SetPath(owner.CurrentPositionTarget);
    }

    private void SetPath(Vector2 targetPosition)
    {
        if (chaoticTargetPosition)
        {
            targetPosition.x += Random.Range(-targetRandomOffset, targetRandomOffset);
            targetPosition.y += Random.Range(-targetRandomOffset, targetRandomOffset);
        }

        seeker.StartPath(owner.GetRb.position, targetPosition, OnPathComplete);
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public Vector2 CheckWayPoint()
    {
        if (path == null) return Vector2.zero;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return Vector2.zero;
        }
        else reachedEndOfPath = false;

        Vector2 dir = (((Vector2)path.vectorPath[currentWaypoint] - owner.GetRb.position)).normalized;

        float dist = Vector2.Distance(owner.GetRb.position, path.vectorPath[currentWaypoint]);

        if (dist < nextWaypointDistance) currentWaypoint++;

        return dir;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!debugMode) return;

        Gizmos.DrawWireSphere(owner.transform.position, nextWaypointDistance); 
#endif
    }

}

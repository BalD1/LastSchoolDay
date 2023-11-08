using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
public class BasePathfinding : MonoBehaviourEventsHandler
{
    [field: SerializeField] public IPathfindingTarget Target { get; private set; }

    [SerializeField] private IComponentHolder owner;
    [SerializeField] private Seeker seeker;

    [SerializeField] private float nextWaypointDistance = 3f;
    [SerializeField] private float updatePathCooldown = .5f;
    private float updatePathTimer;

    [SerializeField] private float targetRandomOffset = 0f;

    [SerializeField] private bool chaoticTargetPosition = false;

    [SerializeField] private bool predictTargetFuturePosition = true;
    [SerializeField] private float futureTimeToPredict = .25f;
    private float baseFutureTimeToPredict;

    [SerializeField] private LayerMask predictRaycastMask;

    private bool fleeFromTarget = false;
    private Path path;
    private int currentWaypoint = 0;

    protected override void EventsSubscriber()
    {
    }

    protected override void EventsUnSubscriber()
    {
    }

    private void Start()
    {
        baseFutureTimeToPredict = futureTimeToPredict;
    }

    private void Update()
    {
        TryUpdatePath();
    }

    private void TryUpdatePath()
    {
        if (updatePathTimer > 0)
        {
            updatePathTimer -= Time.deltaTime;
            return;
        }
        if (Target == null) return;

        updatePathTimer = updatePathCooldown;
        UpdatePath();
    }

    public void UpdatePath()
    {
        if (fleeFromTarget)
        {
            Vector2 direction = (Vector2)this.transform.position - Target.GetPosition();
            SetPath((Vector2)this.transform.position + direction);
        }
        else SetPath(Target.GetPosition());
    }

    private void SetPath(Vector2 targetPosition)
    {
        if (predictTargetFuturePosition) targetPosition = PredictFuturePosition(targetPosition);

        if (chaoticTargetPosition)
        {
            targetPosition.x += Random.Range(-targetRandomOffset, targetRandomOffset);
            targetPosition.y += Random.Range(-targetRandomOffset, targetRandomOffset);
        }

        seeker.StartPath(this.transform.position, targetPosition, OnPathComplete);
    }

    private Vector2 PredictFuturePosition(Vector2 targetPosition)
    {
        Vector2 targetVelocity = Target.GetVelocity();

        // do not predict if the target is headed toward self
        if (targetVelocity.x > 0 ^ (targetPosition.x - this.transform.position.x > 0)) return targetPosition;
        if (targetVelocity.y > 0 ^ (targetPosition.y - this.transform.position.y > 0)) return targetPosition;

        Vector2 predictedPosition = targetPosition + targetVelocity * Target.GetSpeed() * futureTimeToPredict;

        RaycastHit2D raycast = Physics2D.Raycast(targetPosition, predictedPosition, futureTimeToPredict, predictRaycastMask);
        if (raycast.collider == null) return predictedPosition;
        return targetPosition;
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

        if (currentWaypoint >= path.vectorPath.Count) return Vector2.zero;

        Vector2 dir = (path.vectorPath[currentWaypoint] - this.transform.position).normalized;

        float dist = Vector2.Distance(this.transform.position, path.vectorPath[currentWaypoint]);

        if (dist < nextWaypointDistance) currentWaypoint++;

        return dir;
    }

#if UNITY_EDITOR
    [SerializeField] private bool debugMode = false;
#endif

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!debugMode) return;

        Gizmos.DrawWireSphere(this.transform.position, nextWaypointDistance);
#endif
    }
}

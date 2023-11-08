using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private EnemyBase owner;

    [SerializeField] private Seeker seeker;

    [SerializeField] private float nextWaypointDistance = 3f;
    [SerializeField] private float updatePathCooldown = .5f;

    [SerializeField] private float targetRandomOffset = 0f;
    [SerializeField] private float targetRandomOffsetAfterAttack = 0f;

    [SerializeField] private bool chaoticTargetPosition = false;

    [SerializeField] private bool predictTargetFuturePosition = true;
    [SerializeField] private float futureTimeToPredict = .25f;
    private float baseFutureTimeToPredict;

    [SerializeField] private LayerMask predictRaycastMask;

#if UNITY_EDITOR
    [SerializeField] private bool debugMode = false; 
#endif

    private bool fleeFromTarget = false;

    private Path path;

    private int currentWaypoint = 0;

//    public void StartUpdatePath() => InvokeRepeating(nameof(TryUpdatePath), 0f, updatePathCooldown);
//    public void StopUpdatepath() => CancelInvoke();

//    private void Start()
//    {
//        baseFutureTimeToPredict = futureTimeToPredict;

//        //owner.D_receivedStampModifier += ModifyPredictOnSpeedModifier;
//    }

//    public void ModifyPredictOnSpeedModifier()
//    {
//        float step = ZombiesScalingManager.Instance.TotalSpeedAddition;
//        float maxTargetPosPredict = ZombiesScalingManager.Instance.MaxTargetPositionPredictTime;

//        futureTimeToPredict += (baseFutureTimeToPredict - maxTargetPosPredict) * step;

//        futureTimeToPredict = Mathf.Clamp(futureTimeToPredict, maxTargetPosPredict, baseFutureTimeToPredict);
//    }

//    public void TryUpdatePath()
//    {
//        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;

//        //if (!owner.IsAlive()) return;
//        if (!seeker.IsDone()) return;
//        //if (owner.CurrentPositionTarget == null || owner.CurrentPlayerTarget == null) return;

//        UpdatePath();
//    }
//    public void UpdatePath()
//    {
//        if (owner.CurrentPositionTarget == null) return;

//        if (fleeFromTarget)
//        {
//            Vector2 direction = (Vector2)this.transform.position - owner.CurrentPositionTarget;
//            SetPath((Vector2)this.transform.position + direction);
//        }
//        else SetPath(owner.CurrentPositionTarget);
//    }

//    private void SetPath(Vector2 targetPosition)
//    {
//        if (predictTargetFuturePosition) PredictFuturePosition(ref targetPosition);

//        if (chaoticTargetPosition)
//        {
//            //float offset = owner.Attack_TIMER > 0 ? targetRandomOffsetAfterAttack : targetRandomOffset;
//            //targetPosition.x += Random.Range(-offset, offset);
//            //targetPosition.y += Random.Range(-offset, offset);
//        }

//        seeker.StartPath(this.transform.position, targetPosition, OnPathComplete);
//    }

//    private void PredictFuturePosition(ref Vector2 targetPosition)
//    {
//        PlayerCharacter target = owner.CurrentPlayerTarget;

//        Vector2 playerVelocity = target.PlayerMotor == null ? Vector2.zero : target.PlayerMotor.Velocity;

//        // do not predict if the target is headed toward self
//        if (playerVelocity.x > 0 ^ (targetPosition.x - this.transform.position.x > 0)) return;
//        if (playerVelocity.y > 0 ^ (targetPosition.y - this.transform.position.y > 0)) return;

//        //Vector2 predictedPosition = targetPosition + playerVelocity * target.MaxSpeed_M * futureTimeToPredict;

//        //RaycastHit2D raycast = Physics2D.Raycast(targetPosition, predictedPosition, futureTimeToPredict, predictRaycastMask);
//        //if (raycast.collider == null) targetPosition = predictedPosition;
//    }

//    private void OnPathComplete(Path p)
//    {
//        if (!p.error)
//        {
//            path = p;
//            currentWaypoint = 0;
//        }
//    }

//    public Vector2 CheckWayPoint()
//    {
//        if (path == null) return Vector2.zero;

//        if (currentWaypoint >= path.vectorPath.Count) return Vector2.zero;

//        Vector2 dir = (path.vectorPath[currentWaypoint] - this.transform.position).normalized;

//        float dist = Vector2.Distance(this.transform.position, path.vectorPath[currentWaypoint]);

//        if (dist < nextWaypointDistance) currentWaypoint++;

//        return dir;
//    }

//    private void OnDrawGizmos()
//    {
//#if UNITY_EDITOR
//        if (!debugMode) return;

//        Gizmos.DrawWireSphere(this.transform.position, nextWaypointDistance); 
//#endif
//    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] private EnemyBase owner;

    [SerializeField] private float detectionRange;

    [SerializeField] private CircleCollider2D detectionTrigger;

    [SerializeField] private LayerMask detectionMask;

    [SerializeField] private float targetUpdateRate = 2;
    private float targetUpdate_TIMER;

    private List<Transform> targets;

    [field: SerializeField] public bool isActive { get; private set; }

#if UNITY_EDITOR
    [SerializeField] private bool debugMode;
#endif

    private Vector3 dir;

    private void Start()
    {
        //targets = new List<Transform>();
        PlayerCharacter closerTarget = null;
        float closerDistance = float.MaxValue;
        foreach (var item in GameManager.Instance.playersByName)
        {
            //targets.Add(item.playerScript.gameObject.transform);
            float currentDist = Vector2.Distance(owner.transform.position, item.playerScript.transform.position);
            if(currentDist < closerDistance)
            {
                closerDistance = currentDist;
                closerTarget = item.playerScript;
            }
        }

        owner.SetTarget(closerTarget);
    }

    /*
    private void Update()
    {
        if (targetUpdate_TIMER > 0) targetUpdate_TIMER = Time.time;
        else UpdateTarget();
    }

    private void UpdateTarget()
    {
        targetUpdate_TIMER = targetUpdateRate;

        foreach (var item in collection)
        {

        }
    }
    */

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isActive) return;

        if (collision.CompareTag("VisionTarget"))
        {
            PlayerCharacter player = collision.GetComponentInParent<PlayerCharacter>();
            if (player == null) return;

            if (owner.DetectedPlayers.Contains(player) == false)
            {
                dir = (player.transform.position - owner.transform.position);
                RaycastHit2D hit = Physics2D.Raycast(owner.transform.position, dir, detectionRange, detectionMask);
#if UNITY_EDITOR
                if (debugMode)
                    Debug.DrawRay(owner.transform.position, dir, Color.red);
#endif
                if (hit)
                {
                    if (hit.collider.CompareTag("VisionTarget")) owner.AddDetectedPlayer(player);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isActive) return;

        if (collision.CompareTag("VisionTarget"))
        {
            PlayerCharacter player = collision.GetComponentInParent<PlayerCharacter>();
            if (owner.DetectedPlayers.Contains(player))
            {
                owner.RemoveDetectedPlayer(player);
            }
        }
    }

    public void SetVisionState(bool active)
    {
        isActive = active;
        if (!isActive)
        {
            foreach (var item in owner.DetectedPlayers)
            {
                owner.RemoveDetectedPlayer(item);
            }
        }
    }

    private void OnValidate()
    {
        if (detectionTrigger != null)
            detectionTrigger.radius = detectionRange;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!debugMode) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(this.transform.position, detectionRange);
#endif
    }
}

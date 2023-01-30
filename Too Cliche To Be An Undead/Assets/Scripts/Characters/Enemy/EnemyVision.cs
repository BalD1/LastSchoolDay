using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] private EnemyBase owner;

    [SerializeField] private float targetUpdateRate = 2;
    private float targetUpdate_TIMER;

    private List<Transform> targets;

    public bool targetPlayerAtStart = true;

    private void Start()
    {
        targets = new List<Transform>();

        if (targetPlayerAtStart) TargetClosestPlayer();
    }

    public void TargetClosestPlayer()
    {
        PlayerCharacter closerTarget = null;
        float closerDistance = float.MaxValue;
        foreach (var item in GameManager.Instance.playersByName)
        {
            targets.Add(item.playerScript.gameObject.transform);
            owner.AddDetectedPlayer(item.playerScript);
            float currentDist = Vector2.Distance(owner.transform.position, item.playerScript.transform.position);
            if (currentDist < closerDistance)
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
}

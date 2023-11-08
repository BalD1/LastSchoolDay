using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] private EnemyBase owner;

    [SerializeField] private bool updateTarget = false;

    [SerializeField] private LayerMask wallsLayer;

    [SerializeField] private float targetUpdateRate = 2;
    private float targetUpdate_TIMER;

    public bool targetPlayerAtStart = true;

    private void Start()
    {
        if (targetPlayerAtStart) TargetClosestPlayer();
    }

    public void TargetClosestPlayer()
    {
        PlayerCharacter closerTarget = null;
        float closerDistance = float.MaxValue;

        foreach (var item in IGPlayersManager.Instance.PlayersList)
        {
            //if (item.IsAlive() == false) continue;

            float currentDist = (owner.transform.position - item.transform.position).sqrMagnitude;
            if (currentDist < closerDistance)
            {
                closerDistance = currentDist;
                closerTarget = item;
            }
        }

        //owner.SetTarget(closerTarget);
    }

    
    private void Update()
    {
        if (!updateTarget) return;

        if (targetUpdate_TIMER > 0) targetUpdate_TIMER = Time.time;
        else UpdateTarget();
    }

    private void UpdateTarget()
    {
        targetUpdate_TIMER = targetUpdateRate;

    }
    
}

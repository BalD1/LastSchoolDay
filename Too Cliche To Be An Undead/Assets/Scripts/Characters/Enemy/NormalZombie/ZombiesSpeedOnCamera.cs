using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesSpeedOnCamera : MonoBehaviour
{
    [SerializeField] private NormalZombie owner;

    private float visibilityTimer;

    private bool visibilityFlag;

    private const int waitTimerForVisibility = 1;

    private void OnBecameInvisible()
    {
        if (owner.isIdle) return;

        owner.speedMultiplierWhenOutsideOfCamera = 5;
        owner.isVisible = false;
        visibilityFlag = false;
    }

    private void OnBecameVisible()
    {
        if (owner.isIdle)
        {
            if (owner.CurrentPlayerTarget != null) return;

            owner.Vision.TargetClosestPlayer();
            return;
        }
        owner.speedMultiplierWhenOutsideOfCamera = 1;

        visibilityFlag = true;
        visibilityTimer = waitTimerForVisibility;
    }

    private void Update()
    {
        if (visibilityFlag)
        {
            if (visibilityTimer > 0)
            {
                visibilityTimer -= Time.deltaTime;
                return;
            }

            visibilityFlag = false;
            owner.isVisible = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesSpeedOnCamera : MonoBehaviour
{
    [SerializeField] private NormalZombie owner;

    private void OnBecameInvisible()
    {
        if (owner.isIdle) return;

        owner.speedMultiplierWhenOutsideOfCamera = 5;
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
    }
}

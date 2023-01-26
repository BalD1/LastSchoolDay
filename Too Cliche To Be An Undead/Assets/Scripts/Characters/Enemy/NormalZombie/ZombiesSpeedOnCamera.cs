using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesSpeedOnCamera : MonoBehaviour
{
    [SerializeField] private NormalZombie owner;

    private void OnBecameInvisible()
    {
        owner.speedMultiplierWhenOutsideOfCamera = 5;
    }

    private void OnBecameVisible()
    {
        owner.speedMultiplierWhenOutsideOfCamera = 1;
    }
}

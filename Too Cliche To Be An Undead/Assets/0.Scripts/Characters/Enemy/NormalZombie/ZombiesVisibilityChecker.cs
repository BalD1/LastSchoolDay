using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesVisibilityChecker : MonoBehaviour
{
    [SerializeField] private BaseZombie owner;

    private float visibilityTimer;

    private bool wasVisibleOnce = false;
    private bool isVisible = false;

    private const int invisibilityTimerBeforeKill = 10;

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        Vector3 viewPos = mainCam.WorldToViewportPoint(this.transform.position);
        if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
        {
            wasVisibleOnce = true;
            isVisible = true;

            //if (owner.isIdle)
            //{
            //    if (owner.CurrentPlayerTarget == null)
            //        owner.Vision.TargetClosestPlayer();

            //    Destroy(this);
            //    return;
            //}

            return;
        }

        if (isVisible)
        {
            isVisible = false;
            visibilityTimer = invisibilityTimerBeforeKill;
        }

        if (!wasVisibleOnce) return;

        //if (visibilityTimer > 0) visibilityTimer -= Time.deltaTime;
        //else owner.ForceKill();
    }
}

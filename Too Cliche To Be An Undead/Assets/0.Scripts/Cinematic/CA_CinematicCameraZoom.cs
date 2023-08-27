using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CA_CinematicCameraZoom : CA_CinematicAction
{
    [SerializeField] private float zoomAmount;
    [SerializeField] private float zoomSpeed;

    public CA_CinematicCameraZoom(float zoomAmount, float zoomSpeed)
    {
        this.zoomAmount = zoomAmount;
        this.zoomSpeed = zoomSpeed;
    }

    public override void Execute()
    {
        CameraManager.Instance.ZoomCamera(zoomAmount, zoomSpeed, () => ActionEnded(this));
    }

}

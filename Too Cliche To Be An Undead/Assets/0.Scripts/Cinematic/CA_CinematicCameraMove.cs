using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CA_CinematicCameraMove : CA_CinematicAction
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private bool teleport = false;
    [SerializeField] private float travelTime = 2;

    private Camera cam;

    public CA_CinematicCameraMove(Vector2 _targetPosition, float _travelTime)
        => Setup(Camera.main, _targetPosition, _travelTime);
    public CA_CinematicCameraMove(Vector2 _targetPosition, bool _teleport = true)
        => Setup(Camera.main, _targetPosition, _teleport);

    public CA_CinematicCameraMove(Camera _cam, Vector2 _targetPosition, bool _teleport = true)
        => Setup(_cam, _targetPosition, _teleport);
    public CA_CinematicCameraMove(Camera _cam, Vector2 _targetPosition, float _travelTime)
        => Setup(_cam, _targetPosition, _travelTime);

    public CA_CinematicCameraMove(Transform _targetTransform, float _travelTime)
        => Setup(Camera.main, _targetTransform, _travelTime);
    public CA_CinematicCameraMove(Transform _targetTransform, bool _teleport = true)
        => Setup(Camera.main, _targetTransform, _teleport);

    public CA_CinematicCameraMove(Camera _cam, Transform _targetTransform, float _travelTime)
        => Setup(_cam, _targetTransform, _travelTime);
    public CA_CinematicCameraMove(Camera _cam, Transform _targetTransform, bool _teleport = true)
        => Setup(_cam, _targetTransform, _teleport);

    public void Setup(Camera _cam, Vector2 _targetPosition, bool _teleport = true)
    {
        this.cam = _cam;
        this.targetPosition = _targetPosition;
        this.teleport = _teleport;
    }
    public void Setup(Camera _cam, Vector2 _targetPosition, float _travelTime)
    {
        this.cam = _cam;
        this.targetPosition = _targetPosition;
        this.teleport = false;
        this.travelTime = _travelTime;
    }
    public void Setup(Camera _cam, Transform _targetTransform, bool _teleport = true)
    {
        this.cam = _cam;
        this.targetTransform = _targetTransform;
        this.teleport = _teleport;
    }
    public void Setup(Camera _cam, Transform _targetTransform, float _travelTime)
    {
        this.cam = _cam;
        this.targetTransform = _targetTransform;
        this.teleport = false;
        this.travelTime = _travelTime;
    }

    public void SetTeleport(bool _teleport) => this.teleport = _teleport;

    public override void Execute()
    {
        if (teleport) travelTime = 0;

        if (targetTransform != null) targetPosition = targetTransform.position;

        if (cam == Camera.main && CameraManager.ST_InstanceExists())
        {
            CameraManager.Instance.MoveCamera(targetPosition, () => ActionEnded(this), teleport ? 0 : travelTime);
            return;
        }
        LeanTween.move(cam.gameObject, targetPosition, travelTime).setEase(leanType)
                                                                  .setOnComplete(() => this.ActionEnded(this));
    }
}

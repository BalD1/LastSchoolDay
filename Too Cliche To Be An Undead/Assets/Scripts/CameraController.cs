using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private List<Transform> targets;

    [SerializeField] private float smoothTime = .5f;
    [SerializeField] private float minZoom = 40f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private float zoomLimiter = 50f;

    [SerializeField] private Vector3 offset;


    private Vector3 centerPoint;
    private Vector3 targetPosition;
    private Vector3 velocity;

    void Start()
    {
        if (GameManager.Instance.playersByName.Count > 0)
        {
            for (int i = 0; i < GameManager.Instance.playersByName.Count; i++)
            {
                targets.Add(GameManager.Instance.playersByName[i].playerScript.transform);
            }
        }
        else
        {
            for (int i = 0; i < DataKeeper.Instance.playersDataKeep.Count ; i++)
            {
                targets.Add(DataKeeper.Instance.playersDataKeep[i].playerInput.transform);
            }
        }
    }

    private void LateUpdate()
    {
        if (targets.Count == 0) return;

        Movements();
        Zoom();
    }

    private void Movements()
    {
        centerPoint = GetCenterPoint();

        targetPosition = centerPoint + offset;

        this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPosition, ref velocity, smoothTime);
    }

    private Vector3 GetCenterPoint()
    {
        if (targets.Count == 1) return targets[0].position;

        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);

        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }

    private void Zoom()
    {
        float newFOV = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newFOV, Time.deltaTime);
    }

    private float GetGreatestDistance()
    {
        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.size.x;
    }

    public void RemovePlayerFromList(Transform player) => targets.Remove(player);

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimDirection : MonoBehaviour
{
    [SerializeField] private PlayerInputs ownerInputs;
    [SerializeField] private PlayerCharacter owner;
    [SerializeField] private Transform indicatorHolder;

    private float lookAngle;

    public Quaternion RotationTowardsMovements()
    {
        Vector2 lastDirection = owner.PlayerMotor.LastDirection.normalized;
        lookAngle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(lookAngle + 180, Vector3.forward);
    }

    public Quaternion RotationTowardsMouse()
    {
        return Quaternion.identity;
    }
}

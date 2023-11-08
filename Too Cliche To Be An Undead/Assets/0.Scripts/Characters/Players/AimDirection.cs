using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimDirection : MonoBehaviourEventsHandler
{
    [SerializeField] private PlayerCharacter owner;
    [SerializeField] private Transform indicatorHolder;
    [SerializeField] private float slerpSpeed = 15f;
    [SerializeField] private float rotationOffset = 180;

    private bool isUsingGamepad;

    private PlayerInputHandler ownerInputs;
    private PlayerMotor playerMotor;

    private float lookAngle;

    protected override void Awake()
    {
        owner.HolderTryGetComponent(IComponentHolder.E_Component.Motor, out playerMotor);
        if (owner.HolderTryGetComponent(IComponentHolder.E_Component.PlayerInputsComponent, out ownerInputs) == IComponentHolder.E_Result.Success)
            ownerInputs.OnDeviceChange += CheckIfIsOnKeyboard;
        base.Awake();
    }

    protected override void EventsSubscriber()
    {
        owner.OnComponentModified += CheckNewPlayerInputs;
    }

    protected override void EventsUnSubscriber()
    {
        if (ownerInputs != null)
            ownerInputs.OnDeviceChange -= CheckIfIsOnKeyboard;
        owner.OnComponentModified -= CheckNewPlayerInputs;
    }

    private void CheckNewPlayerInputs(ComponentChangeEventArgs args)
    {
        if (args.ComponentType != IComponentHolder.E_Component.PlayerInputsComponent) return;
        if (ownerInputs != null) ownerInputs.OnDeviceChange -= CheckIfIsOnKeyboard;
        ownerInputs = args.NewComponent as PlayerInputHandler;
        ownerInputs.OnDeviceChange += CheckIfIsOnKeyboard;
        CheckIfIsOnKeyboard(ownerInputs.currentDeviceType);
    }

    private void Update()
    {
        SetAim();
    }

    private void CheckIfIsOnKeyboard(PlayerInputsManager.E_Devices currentDevice)
    {
        isUsingGamepad = currentDevice != PlayerInputsManager.E_Devices.Keyboard;
    }

    private void SetAim()
    {
        this.transform.rotation = isUsingGamepad ?
                                  RotationTowardsMovements(true) :
                                  RotationTowardsMouse();
    }

    public Quaternion RotationTowardsMovements(bool lerpRotation)
    {
        Vector2 lastDirection = playerMotor.LastDirection.normalized;
        lookAngle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg;
        Quaternion angle = Quaternion.AngleAxis(lookAngle + 180, Vector3.forward);

        if (!lerpRotation) return angle;
        return Quaternion.Slerp(this.transform.rotation, angle, Time.deltaTime * slerpSpeed);
    }

    public Quaternion RotationTowardsMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5f;

        Vector3 selfPosByCam = Camera.main.WorldToScreenPoint(owner.transform.position);

        mousePos.x -= selfPosByCam.x;
        mousePos.y -= selfPosByCam.y;

        lookAngle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(lookAngle + rotationOffset, Vector3.forward);
    }

    public void RotateTowardsTarget(Transform target)
    {
        if (target == null) return;
        if (!isUsingGamepad) return;

        Vector2 dir = (target.position - this.transform.position).normalized;
        lookAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(lookAngle + 180, Vector3.forward);
        playerMotor.ForceSetLasDirection(dir);
    }

    public Vector2 GetPreciseAimDirection()
    {
        if (!isUsingGamepad)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 5f;

            Vector3 selfPosByCam = Camera.main.WorldToScreenPoint(owner.transform.position);

            mousePos.x -= selfPosByCam.x;
            mousePos.y -= selfPosByCam.y;

            return mousePos;
        }
        else return playerMotor.LastDirection.normalized;
    }

    public Vector2 GetGeneralAimDirection()
    {
        float rot = this.transform.rotation.eulerAngles.z;

        if (rot > 45 && rot <= 135) return Vector2.down;
        else if (rot > 135 && rot <= 225) return Vector2.right;
        else if (rot > 225 && rot <= 315) return Vector2.up;
        else return Vector2.left;
    }
    public GameManager.E_Direction GetGeneralAimDirectionEnum()
    {
        float rot = this.transform.rotation.eulerAngles.z;

        if (rot > 45 && rot <= 135) return GameManager.E_Direction.Down;
        else if (rot > 135 && rot <= 225) return GameManager.E_Direction.Right;
        else if (rot > 225 && rot <= 315) return GameManager.E_Direction.Up;
        else return GameManager.E_Direction.Left;
    }
}

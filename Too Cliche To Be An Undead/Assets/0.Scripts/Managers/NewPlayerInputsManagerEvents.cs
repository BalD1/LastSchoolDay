using System;
using UnityEngine.InputSystem;

public static class NewPlayerInputsManagerEvents
{
    public static event Action OnEndedChangingIndexes;
    public static void EndedChangingIndexes(this NewPlayerInputsManager manager) => OnEndedChangingIndexes?.Invoke();

    public static event Action<InputDevice> OnDeviceDisconnected;
    public static void DeviceDisconnected(this NewPlayerInputsManager manager, InputDevice device)
        => OnDeviceDisconnected?.Invoke(device);

    public static event Action<InputDevice> OnDeviceReconnected;
    public static void DeviceReconnected(this NewPlayerInputsManager manager, InputDevice device)
        => OnDeviceReconnected?.Invoke(device);

    public static event Action<InputDevice> OnDeviceAdded;
    public static void DeviceAdded(this NewPlayerInputsManager manager, InputDevice device)
        => OnDeviceAdded?.Invoke(device);
}

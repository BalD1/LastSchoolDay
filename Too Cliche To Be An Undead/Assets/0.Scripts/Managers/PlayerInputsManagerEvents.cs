
using System;
using UnityEngine.InputSystem;

public static class PlayerInputsManagerEvents
{
    public static event Action OnEndedChangingIndexes;
    public static void EndedChangingIndexes(this PlayerInputsManager manager) => OnEndedChangingIndexes?.Invoke();

    public static event Action<InputDevice> OnDeviceDisconnected;
    public static void DeviceDisconnected(this PlayerInputsManager manager, InputDevice device) 
        => OnDeviceDisconnected?.Invoke(device);

    public static event Action<InputDevice> OnDeviceReconnected;
    public static void DeviceReconnected(this PlayerInputsManager manager, InputDevice device)
        => OnDeviceReconnected?.Invoke(device);

    public static event Action<InputDevice> OnDeviceAdded;
    public static void DeviceAdded(this  PlayerInputsManager manager, InputDevice device)
        => OnDeviceAdded?.Invoke(device);
}


using System;

public static class HUBDoorEventHandler
{
    public static event Action OnInteractedWithDoor;
    public static void InteractedWithDoor(this HUBDoor door) => OnInteractedWithDoor?.Invoke();
}

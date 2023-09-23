using System;
using System.Collections;
using UnityEngine;

public static class EndCinematicEvents
{
    public static event Action OnDoorOpened;
    public static void DoorOpened(this EndCinematic endCinematic)
        => OnDoorOpened?.Invoke();
}

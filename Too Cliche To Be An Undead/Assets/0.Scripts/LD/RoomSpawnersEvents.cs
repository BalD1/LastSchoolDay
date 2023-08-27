using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoomSpawnersEvents
{
    public static event Action OnEnteredRoomSpawner;
    public static void EnteredRoomSpawner(this RoomSpawners roomSpawner)
        => OnEnteredRoomSpawner?.Invoke();
}

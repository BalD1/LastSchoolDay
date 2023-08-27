using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GymnasiumCinematicEvents
{
    public static event Action OnGymnasiumCinematicStarted;
    public static void GymnasiumCinematicStarted(this GymnasiumCinematic cinematic)
        => OnGymnasiumCinematicStarted?.Invoke();
}

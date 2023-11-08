using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ZombiesScalingManagerEvents
{
    public static event Action<SO_StatModifierData[]> OnSendModifiers;
    public static void SendModifiers(this ZombiesScalingManager manager, SO_StatModifierData[] modifiers)
        => OnSendModifiers?.Invoke(modifiers);
}

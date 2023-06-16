
using System;

public static class PlayerInputsManagerEvents
{
    public static event Action OnEndedChangingIndexes;
    public static void EndedChangingIndexes(this PlayerInputsManager manager) => OnEndedChangingIndexes?.Invoke();
}

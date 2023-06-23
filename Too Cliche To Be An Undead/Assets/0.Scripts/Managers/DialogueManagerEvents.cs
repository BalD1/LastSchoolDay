using System;

public static class DialogueManagerEvents
{
    public static event Action<bool> OnStartDialogue;
    public static void StartedDialogue(this DialogueManager manager, bool _fromCinematic) => OnStartDialogue?.Invoke(_fromCinematic);

    public static event Action<bool> OnEndDialogue;
    public static void EndedDialogue(this DialogueManager manager, bool _fromCinematic) => OnEndDialogue?.Invoke(_fromCinematic);
}

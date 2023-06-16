using System;

public static class DialogueManagerEvents
{
    public static event Action OnStartDialogue;
    public static void StartedDialogue(this DialogueManager manager) => OnStartDialogue?.Invoke();

    public static event Action OnEndDialogue;
    public static void EndedDialogue(this DialogueManager manager) => OnEndDialogue?.Invoke();
}

using System.Collections.Generic;
using UnityEngine;

public class Cinematic
{
    private Queue<CinematicAction> actionQueue;

    public Cinematic() 
    {
        actionQueue = new Queue<CinematicAction>();
    }
    public Cinematic(Queue<CinematicAction> actionQueue)
    {
        actionQueue = new Queue<CinematicAction>();
        this.actionQueue = actionQueue;
    }
    public Cinematic(params CinematicAction[] action)
    {
        actionQueue = new Queue<CinematicAction>(action);
    }

    public void AddAction(CinematicAction action)
    {
        actionQueue.Enqueue(action);
    }
    public void AddActions(List<CinematicAction> actions)
    {
        foreach (CinematicAction action in actions) actionQueue.Enqueue(action);
    }

    public void StartCinematic()
    {
        this.ChangeCinematicState(true);
        PlayNextAction(null);
    }

    private void PlayNextAction(CinematicAction last)
    {
        if (last != null) last.OnEnded -= PlayNextAction;
        if (actionQueue.Count <= 0)
        {
            EndCinematic();
            return;
        }

        CinematicAction next = actionQueue.Dequeue();
        next.OnEnded += PlayNextAction;
        next.Execute();
    }

    public void EndCinematic()
    {
        this.ChangeCinematicState(false);
    }
}

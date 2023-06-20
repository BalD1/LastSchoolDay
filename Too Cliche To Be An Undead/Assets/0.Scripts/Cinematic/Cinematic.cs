using System.Collections.Generic;
using UnityEngine;

public class Cinematic
{
    private Queue<CinematicAction> actionQueue;
    public List<PlayerCharacter> Players { get; private set; } = new List<PlayerCharacter>();

    public Cinematic() 
    {
        actionQueue = new Queue<CinematicAction>();
    }
    public Cinematic(Queue<CinematicAction> actionQueue)
    {
        actionQueue = new Queue<CinematicAction>();
        this.actionQueue = actionQueue;
        foreach (var item in actionQueue)
        {
            item.SetOwner(this);
        }
    }
    public Cinematic(params CinematicAction[] action)
    {
        actionQueue = new Queue<CinematicAction>(action);
        foreach (var item in action)
        {
            item.SetOwner(this);
        }
    }
    public Cinematic SetPlayer(PlayerCharacter _player)
    {
        Players.Add(_player);
        return this;
    }
    public Cinematic SetPlayers(List<PlayerCharacter> _players)
    {
        this.Players = _players;
        return this;
    }

    public void AddAction(CinematicAction action)
    {
        actionQueue.Enqueue(action);
        action.SetOwner(this);
    }
    public void AddActions(List<CinematicAction> actions)
    {
        foreach (CinematicAction action in actions)
        {
            actionQueue.Enqueue(action);
            action.SetOwner(this);
        }
    }

    public void StartCinematic()
    {
        this.ChangeCinematicState(true);
        PlayNextAction(null);
    }

    private void PlayNextAction(CinematicAction last)
    {
        if (last != null) last.OnActionEnded -= PlayNextAction;
        if (actionQueue.Count <= 0)
        {
            EndCinematic();
            return;
        }

        CinematicAction next = actionQueue.Dequeue();
        next.OnActionEnded += PlayNextAction;
        next.Execute();
    }

    public void EndCinematic()
    {
        this.ChangeCinematicState(false);
    }
}
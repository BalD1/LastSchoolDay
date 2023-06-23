using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cinematic
{
    private Queue<CA_CinematicAction> actionQueue;
    public List<PlayerCharacter> Players { get; private set; } = new List<PlayerCharacter>();

    public Cinematic() 
    {
        actionQueue = new Queue<CA_CinematicAction>();
    }
    public Cinematic(Queue<CA_CinematicAction> actionQueue)
    {
        actionQueue = new Queue<CA_CinematicAction>();
        this.actionQueue = actionQueue;
        foreach (var item in actionQueue)
        {
            item.SetOwner(this);
        }
    }
    public Cinematic(params CA_CinematicAction[] action)
    {
        actionQueue = new Queue<CA_CinematicAction>(action);
        foreach (var item in action)
        {
            item.SetOwner(this);
        }
    }
    public Cinematic(SO_Cinematic cinematicSO)
    {
        actionQueue = new Queue<CA_CinematicAction>();
        foreach (var item in cinematicSO.CinematicActions)
        {
            actionQueue.Enqueue(item.Action);
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

    public void AddAction(CA_CinematicAction action)
    {
        actionQueue.Enqueue(action);
        action.SetOwner(this);
    }
    public void AddActions(List<CA_CinematicAction> actions)
    {
        foreach (CA_CinematicAction action in actions)
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

    private void PlayNextAction(CA_CinematicAction last)
    {
        if (last != null) last.OnActionEnded -= PlayNextAction;
        if (actionQueue.Count <= 0)
        {
            EndCinematic();
            return;
        }

        CA_CinematicAction next = actionQueue.Dequeue();
        next.OnActionEnded += PlayNextAction;
        next.Execute();
    }

    public void EndCinematic()
    {
        this.ChangeCinematicState(false);
    }
}
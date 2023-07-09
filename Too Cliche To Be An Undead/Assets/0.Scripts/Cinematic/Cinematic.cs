using System.Collections.Generic;

public class Cinematic
{
    private Queue<CA_CinematicAction> actionQueue;
    public List<PlayerCharacter> Players { get; private set; } = new List<PlayerCharacter>();
    public Cinematic CinematicChain { get; private set; }

    private bool askedPause;
    private bool allowStateChangeAtEnd = true;

    #region Constructors

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

    #endregion

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
    public Cinematic SetPlayers(params PlayerCharacter[] _players)
    {
        this.Players.AddRange(_players);
        return this;
    }
    public Cinematic SetChainCinematic(Cinematic chain)
    {
        this.CinematicChain = chain;
        return this;
    }

    public Cinematic AddAction(CA_CinematicAction action)
    {
        actionQueue.Enqueue(action);
        action.SetOwner(this);
        return this;
    }
    public Cinematic AddActions(List<CA_CinematicAction> actions)
    {
        foreach (CA_CinematicAction action in actions)
        {
            actionQueue.Enqueue(action);
            action.SetOwner(this);
        }
        return this;
    }
    public Cinematic AddActions(params CA_CinematicAction[] actions)
    {
        foreach (CA_CinematicAction action in actions)
        {
            actionQueue.Enqueue(action);
            action.SetOwner(this);
        }
        return this;
    }
    public Cinematic AllowChangeCinematicStateAtEnd(bool allow)
    {
        allowStateChangeAtEnd = allow;
        return this;
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

        if (askedPause) return;

        CA_CinematicAction next = actionQueue.Dequeue();
        next.OnActionEnded += PlayNextAction;
        next.Execute();
    }

    public Cinematic PauseAtNextAction()
    {
        askedPause = true;
        return this;
    }
    public Cinematic Resume()
    {
        askedPause = false;
        PlayNextAction(null);
        return this;
    }

    public void EndCinematic()
    {
        if (CinematicChain != null)
        {
            CinematicChain.StartCinematic();
            return;
        }
        if (allowStateChangeAtEnd) this.ChangeCinematicState(false);
    }
}
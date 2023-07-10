using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CA_CinematicActionMultiple : CA_CinematicAction
{
    private List<CA_CinematicAction> actionsToPlay = new List<CA_CinematicAction>();

    private int remainingActions = 0;

    public CA_CinematicActionMultiple(params CA_CinematicAction[] actionsToPlay)
        => AddActions(actionsToPlay);

    public void AddActions(params CA_CinematicAction[] actions)
    {
        foreach (var item in actions)
        {
            this.actionsToPlay.Add(item);
            item.SetOwner(this.owner);
        }
        remainingActions += actions.Length;
    }

    public override CA_CinematicAction SetOwner(Cinematic owner)
    {
        this.owner = owner;
        foreach (var item in actionsToPlay)
        {
            item.SetOwner(owner);
        }
        return this;
    }

    public override void Execute()
    {
        foreach (var item in actionsToPlay)
        {
            item.OnActionEnded += ActionEnded;
            item.Execute();
        }
    }

    protected override void ActionEnded(CA_CinematicAction action)
    {
        remainingActions--;
        action.OnActionEnded -= ActionEnded;

        if (remainingActions <= 0) base.ActionEnded(this);
    }
}

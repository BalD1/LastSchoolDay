using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CA_CinematicCustomAction : CA_CinematicAction
{
    private Action action;

    [SerializeField] private float waitBeforeActionTime;
    [SerializeField] private float waitAfterActionTime;

    public CA_CinematicCustomAction() { }
    public CA_CinematicCustomAction(Action _actionToPlay)
        => Setup(_actionToPlay, 0, 0);
    public CA_CinematicCustomAction(Action _actionToPlay, float _waitBeforeActionTime, float _waitAfterActionTime)
        => Setup(_actionToPlay, _waitBeforeActionTime, _waitAfterActionTime);
    public void Setup(Action _actionToPlay, float _waitBeforeActionTime, float _waitAfterActionTime)
    {
        this.action = _actionToPlay;
        this.waitBeforeActionTime = _waitBeforeActionTime;
        this.waitAfterActionTime = _waitAfterActionTime;
    }
    public void AddListener(Action _actionToAdd) => action += _actionToAdd;

    public override void Execute()
    {
        if (action == null)
        {
            this.Log("Action was not set in cinematic. Skipping Cinematic Action.");
            this.ActionEnded(this);
            return;
        }

        if (waitBeforeActionTime > 0) LeanTween.delayedCall(waitBeforeActionTime, CallAction);
        else CallAction();
    }

    private void CallAction()
    {
        action?.Invoke();
        if (waitAfterActionTime > 0) LeanTween.delayedCall(waitAfterActionTime, ActionEnded);
        else ActionEnded();
    }

    private void ActionEnded() => this.ActionEnded(this);
}

using UnityEngine;

public class CA_CinematicWait : CA_CinematicAction
{
    [SerializeField] private float waitTime;

    public CA_CinematicWait(float _waitTime)
        => Setup(_waitTime);
    public void Setup(float _waitTime)
    {
        waitTime = _waitTime;
    }

    public override void Execute()
    {
        LeanTween.delayedCall(waitTime, ActionEnded);
    }

    private void ActionEnded() => ActionEnded(this);
}

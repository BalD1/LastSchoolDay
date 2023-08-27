using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CA_CinematicChangeMusicState : CA_CinematicAction
{
    public enum E_State
    {
        Pause,
        Resume,
        Stop
    }
    [SerializeField] private E_State behaviour;
    [SerializeField] private bool waitForFade;

    public CA_CinematicChangeMusicState(E_State behaviour, bool waitForFade)
    {
        this.behaviour = behaviour;
        this.waitForFade = waitForFade;
    }

    public override void Execute()
    {
        switch (behaviour)
        {
            case E_State.Pause:
                if (waitForFade) SoundManager.Instance.PauseMusic(() => ActionEnded(this));
                else
                {
                    SoundManager.Instance.PauseMusic();
                    ActionEnded(this);
                }
                break;
            case E_State.Resume:
                if (waitForFade) SoundManager.Instance.ResumeMusic(() => ActionEnded(this));
                else
                {
                    SoundManager.Instance.ResumeMusic();
                    ActionEnded(this);
                }
                break;
            case E_State.Stop:
                if (waitForFade) SoundManager.Instance.StopMusic(() => ActionEnded(this));
                else
                {
                    SoundManager.Instance.StopMusic();
                    ActionEnded(this);
                }
                break;
        }
    }
}

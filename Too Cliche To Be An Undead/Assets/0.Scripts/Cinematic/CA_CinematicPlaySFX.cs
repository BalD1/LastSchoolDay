using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CA_CinematicPlaySFX : CA_CinematicAction
{
    [SerializeField] private AudioClip clipToPlay;
    [SerializeField] private Vector2 position;

    public CA_CinematicPlaySFX(AudioClip clipToPlay, Vector2 position)
    {
        this.clipToPlay = clipToPlay;
        this.position = position;
    }

    public override void Execute()
    {
        AudioclipPlayer.Create(position, clipToPlay);
        ActionEnded(this);
    }
}

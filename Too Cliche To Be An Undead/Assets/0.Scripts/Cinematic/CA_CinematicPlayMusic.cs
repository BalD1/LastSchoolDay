using UnityEngine;

[System.Serializable]
public class CA_CinematicPlayMusic : CA_CinematicAction
{
    [SerializeField] private SoundManager.E_MusicClipsTags musicToPlay;

    public CA_CinematicPlayMusic(SoundManager.E_MusicClipsTags _musicToPlay)
        => Setup(_musicToPlay);

    public void Setup(SoundManager.E_MusicClipsTags _musicToPlay)
    {
        this.musicToPlay = _musicToPlay;
    }

    public override void Execute()
    {
        if (SoundManager.Instance == null)
        {
            this.Log("UIManager Instance was not set. Skipping Cinematic Action.");
            this.ActionEnded(this);
            return;
        }

        SoundManager.Instance.PlayMusic(musicToPlay);
        this.ActionEnded(this);
    }
}
